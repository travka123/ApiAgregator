namespace ApiAgregator.Entities;

public class CronTask
{
    public int Id { get; set; }
    public string Name { get; }
    public string Description { get; }
    public int OwnerId { get; }
    public CronExpression Expression { get; }
    public string ApiName { get; }
    public DateTime? LastFire { get; }
    public Dictionary<string, string> Parameters { get; }

    public CronTask(string name, string description, int ownerId,
        CronExpression expression, string apiName, Dictionary<string, string> parameters)
    {
        Name = name;
        Description = description;
        OwnerId = ownerId;
        Expression = expression;
        ApiName = apiName;
        LastFire = null;
        Parameters = parameters;
    }

    public CronTask(int id, string name, string description, int ownerId, 
        CronExpression expression, string apiName, DateTime? lastFire, 
        Dictionary<string, string> parameters)
    {
        Id = id;
        Name = name;
        Description = description;
        OwnerId = ownerId;
        Expression = expression;
        ApiName = apiName;
        LastFire = lastFire;
        Parameters = parameters;
    }
}
