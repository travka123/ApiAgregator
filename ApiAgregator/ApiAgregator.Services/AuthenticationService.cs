using ApiAgregator.Entities;
using ApiAgregator.Services.Exceptions;
using ApiAgregator.Services.Repositories;

namespace ApiAgregator.Services;

public class AuthenticationService
{
    private IUserRepository _userRepository;
    private SaltService _saltService;

    public AuthenticationService(IUserRepository userRepository, SaltService saltService)
    {
        _userRepository = userRepository;
        _saltService = saltService;
    }

    public void Register(string username, string email, byte[] password)
    {
        var uSalt = _saltService.CreateUserSalt();
        var sPassword = _saltService.SaltPassword(password, uSalt);
        _userRepository.AddUser(new User(username, email, sPassword, uSalt));
    }

    public User LogInByUsername(string username, byte[] password)
    {
        var user = _userRepository.GetByUsername(username);
        if (!_saltService.Compare(user.Password, user.PasswordSalt, password))
        {
            throw new PasswordDoesNotMatchException();
        }
        return user;
    }
}