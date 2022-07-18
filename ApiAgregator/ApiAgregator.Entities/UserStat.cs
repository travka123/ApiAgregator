namespace ApiAgregator.Entities;

public class UserStat
{
    public UserStat(int userId, int totalTasks, int totalCalls, int totalErrorCalls)
    {
        UserId = userId;
        TotalTasks = totalTasks;
        TotalCalls = totalCalls;
        TotalErrorCalls = totalErrorCalls;
    }

    public int UserId { get; } 
    public int TotalTasks { get; } 
    public int TotalCalls { get; } 
    public int TotalErrorCalls { get; } 
}
