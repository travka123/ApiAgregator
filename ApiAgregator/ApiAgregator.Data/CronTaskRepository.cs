using ApiAgregator.Entities;
using ApiAgregator.Services.Repositories;
using Microsoft.Data.Sqlite;
using System.Globalization;
using System.Text.Json;

namespace ApiAgregator.Data;

public class CronTaskRepository : ICronTaskRepository
{
    private readonly DBConnection _connection;

    public CronTaskRepository(DBConnection connection)
    {
        _connection = connection;
    }

    public void AddCronTask(CronTask cronTask)
    {
        const string expression = "INSERT INTO tasks (owner_id, name, description, expression, api_name, parameters)" +
            "VALUES (@owner_id, @name, @description, @expression, @api_name, @parameters);" +
            "SELECT last_insert_rowid()";

        var sqliteCommand = new SqliteCommand(expression, _connection);

        sqliteCommand.Parameters.Add(new SqliteParameter("@owner_id", cronTask.OwnerId));
        sqliteCommand.Parameters.Add(new SqliteParameter("@name", cronTask.Name));
        sqliteCommand.Parameters.Add(new SqliteParameter("@description", cronTask.Description));
        sqliteCommand.Parameters.Add(new SqliteParameter("@expression", cronTask.Expression.ToString()));
        sqliteCommand.Parameters.Add(new SqliteParameter("@api_name", cronTask.ApiName));
        sqliteCommand.Parameters.Add(new SqliteParameter("@parameters", JsonSerializer.Serialize(cronTask.Parameters)));

        cronTask.Id = (int)(long)sqliteCommand.ExecuteScalar()!;
    }

    public bool UpdateIfOwner(CronTask cronTask)
    {
        const string expression = "UPDATE tasks SET name = @name, description = @description," +
            "expression = @expression, api_name = @api_name, parameters = @parameters " +
            "WHERE id = @id AND owner_id = @owner_id";

        var sqliteCommand = new SqliteCommand(expression, _connection);

        sqliteCommand.Parameters.Add(new SqliteParameter("@id", cronTask.Id));
        sqliteCommand.Parameters.Add(new SqliteParameter("@owner_id", cronTask.OwnerId));
        sqliteCommand.Parameters.Add(new SqliteParameter("@name", cronTask.Name));
        sqliteCommand.Parameters.Add(new SqliteParameter("@description", cronTask.Description));
        sqliteCommand.Parameters.Add(new SqliteParameter("@expression", cronTask.Expression.ToString()));
        sqliteCommand.Parameters.Add(new SqliteParameter("@api_name", cronTask.ApiName));
        sqliteCommand.Parameters.Add(new SqliteParameter("@parameters", JsonSerializer.Serialize(cronTask.Parameters)));

        return sqliteCommand.ExecuteNonQuery() != 0;
    }

    public bool DeleteIfOwner(int taskId, int ownerId)
    {
        const string expression = "DELETE FROM tasks WHERE id = @id AND owner_id = @owner_id";

        var sqliteCommand = new SqliteCommand(expression, _connection);

        sqliteCommand.Parameters.Add(new SqliteParameter("@id", taskId));
        sqliteCommand.Parameters.Add(new SqliteParameter("@owner_id", ownerId));

        return sqliteCommand.ExecuteNonQuery() != 0;
    }

    public List<CronTask> GetTasksByUserId(int userId)
    {
        var list = new List<CronTask>();

        const string expression = "SELECT * FROM tasks WHERE owner_id = @owner_id";

        var sqliteCommand = new SqliteCommand(expression, _connection);

        sqliteCommand.Parameters.Add(new SqliteParameter("@owner_id", userId));

        using (var reader = sqliteCommand.ExecuteReader())
        {
            while (reader.Read())
            {
                var parameters = JsonSerializer.Deserialize<Dictionary<string, string>>((string)reader["parameters"])!;
                DateTime? lastFire = reader["last_fire"] is string lfs ?
                    DateTime.ParseExact(lfs, "O", CultureInfo.InvariantCulture) : null;

                list.Add(new CronTask((int)(long)reader["id"], (string)reader["name"], (string)reader["description"],
                    (int)(long)reader["owner_id"], new CronExpression((string)reader["expression"]), (string)reader["api_name"],
                    lastFire, parameters));
            }
        }

        return list;
    }

    public List<CronTask> GetAllTasks()
    {
        var list = new List<CronTask>();

        const string expression = "SELECT * FROM tasks";

        var sqliteCommand = new SqliteCommand(expression, _connection);

        using (var reader = sqliteCommand.ExecuteReader())
        {
            while (reader.Read())
            {
                var parameters = JsonSerializer.Deserialize<Dictionary<string, string>>((string)reader["parameters"])!;
                DateTime? lastFire = reader["last_fire"] is string lfs ?
                    DateTime.ParseExact(lfs, "O", CultureInfo.InvariantCulture) : null;

                list.Add(new CronTask((int)(long)reader["id"], (string)reader["name"], (string)reader["description"],
                    (int)(long)reader["owner_id"], new CronExpression((string)reader["expression"]), (string)reader["api_name"],
                    lastFire, parameters));
            }
        }

        return list;
    }

    public void AddTaskCall(int taskId, bool IsSuccess)
    {
        const string expression = "INSERT INTO calls (task_id, time, error) " +
            "VALUES (@task_id, @time, @error);";

        var sqliteCommand = new SqliteCommand(expression, _connection);

        sqliteCommand.Parameters.Add(new SqliteParameter("@task_id", taskId));
        sqliteCommand.Parameters.Add(new SqliteParameter("@error", IsSuccess ? 0 : 1));
        sqliteCommand.Parameters.Add(new SqliteParameter("@time", DateTime.UtcNow.ToString("O")));

        sqliteCommand.ExecuteNonQuery();
    }

    public List<TaskCall> GetTaskCalls(int taskId)
    {
        var list = new List<TaskCall>();

        const string expression = "SELECT * FROM calls WHERE task_id = @task_id";

        var sqliteCommand = new SqliteCommand(expression, _connection);

        sqliteCommand.Parameters.Add(new SqliteParameter("@task_id", taskId));

        using (var reader = sqliteCommand.ExecuteReader())
        {
            while (reader.Read())
            {
                var time = DateTime.ParseExact((string)reader["time"], "O", CultureInfo.InvariantCulture);
                list.Add(new TaskCall((int)(long)reader["id"], (int)(long)reader["task_id"], time, (long)reader["error"] == 0));
            }
        }

        return list;
    }
}
