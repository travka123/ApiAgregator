using ApiAgregator.Entities;

namespace ApiAgregator.Services.Repositories;

public interface ICronTaskRepository
{
    public void AddCronTask(CronTask cronTask);
    public List<CronTask> GetTasksByUserId(int userId);
    public void AddTaskCall(int taskId, bool IsSuccess);
    public List<TaskCall> GetTaskCalls(int taskId);
    bool DeleteIfOwner(int taskId, int userId);
    bool UpdateIfOwner(CronTask task);
    List<CronTask> GetAllTasks();
}
