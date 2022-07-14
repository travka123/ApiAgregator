using ApiAgregator.Entities;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;

namespace ApiAgregator.Services;

public class CronTaskScheduler : IHostedService
{
    private Timer? _timer;
    private readonly ILogger<CronTaskScheduler> _logger;
    private readonly IServiceProvider _serviceProvider;

    private readonly ConcurrentDictionary<int, (CronExpression expression, Action<IServiceProvider> action)> _tasks;

    private volatile bool _stopped;
    private Semaphore _semaphore;
    private ManualResetEvent _stopEvent;

    public CronTaskScheduler(IOptions<CronTaskSchedulerOptions> options, ILogger<CronTaskScheduler> logger, 
        IServiceProvider serviceProvider)
    {
        ArgumentNullException.ThrowIfNull(options.Value.ThreadCount);

        var threadCount = options.Value.ThreadCount.Value;

        _logger = logger;
        _serviceProvider = serviceProvider;

        _stopEvent = new ManualResetEvent(false);
        _semaphore = new Semaphore(threadCount, threadCount);
        _tasks = new();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _stopEvent.Reset();
        _stopped = false;
        _timer = new Timer(OnTick, null, (60 - DateTime.Now.Second) * 1000 + 100, 60 * 1000);
        _logger.LogInformation("started");
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Dispose();
        _stopped = true;
        _stopEvent.Set();
        _logger.LogInformation("stopped");
        return Task.CompletedTask;
    }

    private void OnTick(object? _)
    {
        _logger.LogInformation($"timer processing started {DateTime.Now}");

        var waitHandles = new WaitHandle[] { _semaphore, _stopEvent };

        var time = DateTime.Now;

        foreach (var kv in _tasks)
        {
            if (!kv.Value.expression.Match(time))
                continue;

            bool semaphoreAcquired = WaitHandle.WaitAny(waitHandles) == 0;

            if (_stopped)
            {
                if (semaphoreAcquired)
                {
                    _semaphore.Release();
                }

                _logger.LogWarning("timer processing aborted");

                return;
            }

            Task.Run(() =>
            {
                try
                {
                    kv.Value.action(_serviceProvider);
                }
                finally
                {
                    _semaphore.Release();
                }
            });
        }

        _logger.LogInformation($"timer processing finished {DateTime.Now}");
    }

    public void Set(int id, CronExpression expression, Action<IServiceProvider> action)
    {
        _tasks[id] = (expression, action);
        _logger.LogInformation($"task {id} set");
    }

    public void Remove(int id)
    {
        _tasks.Remove(id, out var _);
        _logger.LogInformation($"task {id} removed");
    }
}

public class CronTaskSchedulerOptions
{
    public int? ThreadCount { get; set; } 
}

