using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;

namespace ApiAgregator.Data;

public class DBConnection : SqliteConnection
{
    public DBConnection(IOptions<DBConnectionOption> options) : base(options.Value.ConnectionString) 
    {
        Open();
    }
}

public class DBConnectionOption
{
    public string? ConnectionString { get; set; }
}