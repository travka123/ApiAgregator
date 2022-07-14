using ApiAgregator.BackgroundService.ExternalApis;
using ApiAgregator.Entities;
using ApiAgregator.Services.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace ApiAgregator.Services.APIs;

public class ExternalApisService
{
    private readonly ICronTaskRepository _cronTaskRepository;
    private readonly Dictionary<string, IExternalApi> _externalApis;
    private readonly CronTaskScheduler _cronTaskScheduler;

    public ExternalApisService(ICronTaskRepository cronTaskRepository, CronTaskScheduler cronTaskScheduler,
        IEnumerable<IExternalApi> externalApis)
    {
        _cronTaskRepository = cronTaskRepository;
        _externalApis = externalApis.ToDictionary(a => a.Name);
        _cronTaskScheduler = cronTaskScheduler;
    }

    public CronTask AddTask(int userId, string apiName, string taskName, string taskDescription,
        CronExpression cronExpression, Dictionary<string, string> parametrs)
    {
        if (!_externalApis.TryGetValue(apiName, out var api))
        {
            throw new ArgumentException(nameof(apiName));
        }
        if (!api.ValidateParametrs(parametrs))
        {
            throw new ArgumentException(nameof(parametrs));
        }
        var task = new CronTask(taskName, taskDescription, userId, cronExpression, apiName, parametrs);
        _cronTaskRepository.AddCronTask(task);

        var apiAction = api.CreateAction(task);

        _cronTaskScheduler.Set(task.Id, task.Expression, CreateAction(task));

        return task;
    }

    public CronTask UpdateTask(int taskId, int userId, string apiName, string taskName, string taskDescription,
        CronExpression cronExpression, Dictionary<string, string> parametrs)
    {
        if (!_externalApis.TryGetValue(apiName, out var api))
        {
            throw new ArgumentException(nameof(apiName));
        }
        if (!api.ValidateParametrs(parametrs))
        {
            throw new ArgumentException(nameof(parametrs));
        }
        var task = new CronTask(taskId, taskName, taskDescription, userId, cronExpression, apiName, null, parametrs);

        if (!_cronTaskRepository.UpdateIfOwner(task))
        {
            throw new ArgumentException(nameof(taskId));
        }

        _cronTaskScheduler.Set(task.Id, task.Expression, CreateAction(task));

        return task;
    }

    public Action<IServiceProvider> CreateAction(CronTask task)
    {
        var api = _externalApis[task.ApiName];
        var apiAction = api.CreateAction(task);
        return (serviceProvider) =>
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var cronTaskRepository = scope.ServiceProvider.GetRequiredService<ICronTaskRepository>();
                try
                {
                    apiAction(scope.ServiceProvider);
                }
                catch
                {
                    cronTaskRepository.AddTaskCall(task.Id, true);
                }
                cronTaskRepository.AddTaskCall(task.Id, false);
            }
        };
    }

    public void DeleteTasks(int taskId, int userId)
    {   
        if (_cronTaskRepository.DeleteIfOwner(taskId, userId))
        {
            _cronTaskScheduler.Remove(taskId);
        }
        else
        {
            throw new ArgumentException(nameof(taskId));
        }
    }

    public List<string> GetApis()
    {
        return _externalApis.Keys.ToList();
    }

    public void BuildForm(string apiName, IFormBuilder formBuilder)
    {
        if (!_externalApis.TryGetValue(apiName, out var api))
        {
            throw new ArgumentException(nameof(apiName));
        }
        api.BuildForm(formBuilder);
    }

    public void StartAll()
    {
        var tasks = _cronTaskRepository.GetAllTasks();
        foreach (var task in tasks)
        {
            _cronTaskScheduler.Set(task.Id, task.Expression, CreateAction(task));
        }
    }
}
