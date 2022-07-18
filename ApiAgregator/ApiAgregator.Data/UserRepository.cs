using ApiAgregator.Entities;
using ApiAgregator.Services.Exceptions;
using ApiAgregator.Services.Repositories;
using Microsoft.Data.Sqlite;

namespace ApiAgregator.Data;

public class UserRepository : IUserRepository
{
    private readonly DBConnection _connection;

    public UserRepository(DBConnection connection)
    {
        _connection = connection;
    }

    public void AddUser(User user)
    {
        const string expression = "INSERT INTO users (username, email, email_confirmed, password, password_salt, is_admin) " +
            "VALUES (@username, @email, @email_confirmed, @password, @password_salt, @is_admin);" +
            "SELECT last_insert_rowid()";

        var sqliteCommand = new SqliteCommand(expression, _connection);

        sqliteCommand.Parameters.Add(new SqliteParameter("@username", user.Username));
        sqliteCommand.Parameters.Add(new SqliteParameter("@email", user.Email));
        sqliteCommand.Parameters.Add(new SqliteParameter("@email_confirmed", user.EmailConfirmed ? 1 : 0));
        sqliteCommand.Parameters.Add(new SqliteParameter("@password", user.Password));
        sqliteCommand.Parameters.Add(new SqliteParameter("@password_salt", user.PasswordSalt));
        sqliteCommand.Parameters.Add(new SqliteParameter("@is_admin", user.IsAdmin ? 1 : 0));

        try
        {
            user.Id = (int)(long)sqliteCommand.ExecuteScalar()!;
        }
        catch (SqliteException ex) when (ex.SqliteExtendedErrorCode == 2067)
        {
            throw new UsernameExistsException();
        }
    }

    public void ConfirmEmail(string username, string email)
    {
        const string expression = "UPDATE users SET email_confirmed = 1 " +
            "WHERE username = @username AND email = @email";

        var sqliteCommand = new SqliteCommand(expression, _connection);

        sqliteCommand.Parameters.Add(new SqliteParameter("@username", username));
        sqliteCommand.Parameters.Add(new SqliteParameter("@email", email));

        if (sqliteCommand.ExecuteNonQuery() == 0)
        {
            throw new UserDoesNotExistException();
        }
    }

    public User GetById(int userId)
    {
        const string expression = "SELECT * FROM users WHERE id = @id";

        var sqliteCommand = new SqliteCommand(expression, _connection);

        sqliteCommand.Parameters.Add(new SqliteParameter("@id", userId));

        using (var reader = sqliteCommand.ExecuteReader())
        {
            if (!reader.Read())
            {
                throw new UserDoesNotExistException();
            }

            var user = new User((int)(long)reader["id"], (string)reader["username"], (string)reader["email"],
                (long)reader["email_confirmed"] == 1, (byte[])reader["password"], (byte[])reader["password_salt"],
                (long)reader["is_admin"] == 1);

            if (reader.Read())
            {
                throw new Exception();
            }

            return user;
        }
    }

    public User GetByUsername(string username)
    {
        const string expression = "SELECT * FROM users WHERE username = @username";

        var sqliteCommand = new SqliteCommand(expression, _connection);

        sqliteCommand.Parameters.Add(new SqliteParameter("@username", username));

        using (var reader = sqliteCommand.ExecuteReader())
        {
            if (!reader.Read())
            {
                throw new UserDoesNotExistException();
            }

            var user = new User((int)(long)reader["id"], (string)reader["username"], (string)reader["email"],
                (long)reader["email_confirmed"] == 1, (byte[])reader["password"], (byte[])reader["password_salt"],
                (long)reader["is_admin"] == (long)1);

            if (reader.Read())
            {
                throw new Exception();
            }

            return user;
        }
    }

    public IEnumerable<User> GetUsers()
    {
        var users = new List<User>();

        const string expression = "SELECT * FROM users";

        var sqliteCommand = new SqliteCommand(expression, _connection);

        using (var reader = sqliteCommand.ExecuteReader())
        {
            while (reader.Read())
            {
                users.Add(new User((int)(long)reader["id"], (string)reader["username"], (string)reader["email"],
                    (long)reader["email_confirmed"] == 1, (byte[])reader["password"], (byte[])reader["password_salt"],
                    (long)reader["is_admin"] == 1));
            }

            return users;
        }
    }

    public IEnumerable<UserStat> GetUsersStatistics()
    {
        var stats = new List<UserStat>();

        const string expression = "SELECT users.id, COUNT(a_tasks.id) AS total_tasks, " +
            "SUM(total_calls) AS total_calls, SUM(total_error_calls) AS total_error_calls " +
            "FROM users " +
            "LEFT JOIN ( " +
            "SELECT tasks.id, tasks.owner_id, COUNT(calls.id) AS total_calls, SUM(calls.error) AS total_error_calls " +
            "FROM tasks " +
            "INNER JOIN calls ON tasks.id = calls.task_id " +
            "GROUP BY tasks.id " +
            ") AS a_tasks ON a_tasks.owner_id = users.id " +
            "GROUP BY users.id;";

        var sqliteCommand = new SqliteCommand(expression, _connection);

        using(var reader = sqliteCommand.ExecuteReader())
        {
            while (reader.Read())
            {
                int totalCalls = reader["total_calls"] is System.DBNull ? 0 : (int)(long)reader["total_calls"];
                int totalErrorCalls = reader["total_error_calls"] is System.DBNull ? 0 : (int)(long)reader["total_error_calls"];

                stats.Add(new UserStat((int)(long)reader["id"], (int)(long)reader["total_tasks"],
                    totalCalls, totalErrorCalls));
            }
        }

        return stats;
    }
}
