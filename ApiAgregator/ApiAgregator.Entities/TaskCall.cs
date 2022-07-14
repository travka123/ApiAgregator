namespace ApiAgregator.Entities;

public class TaskCall
{
    public TaskCall(int id, int taskId, DateTime time, bool isSuccess)
    {
        Id = id;
        TaskId = taskId;
        Time = time;
        IsSuccess = isSuccess;
    }

    public int Id { get; }
    public int TaskId { get; }
    public DateTime Time { get;  }
    public bool IsSuccess { get; }
}
