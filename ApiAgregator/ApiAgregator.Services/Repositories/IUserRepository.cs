using ApiAgregator.Entities;

namespace ApiAgregator.Services.Repositories;

public interface IUserRepository
{
    void AddUser(User user);
    void ConfirmEmail(string username, string email);
    User GetByUsername(string username);
    User GetById(int userId);
    public IEnumerable<User> GetUsers();
    IEnumerable<UserStat> GetUsersStatistics();
}