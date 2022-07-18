namespace ApiAgregator.Entities;

public class User
{
    public int Id { get; set; }
    public string Username { get; }
    public string Email { get; }
    public bool EmailConfirmed { get; }
    public bool IsAdmin { get; }   
    public byte[] Password { get; }
    public byte[] PasswordSalt { get; }

    public User(string userName, string email, byte[] password, byte[] passwordSalt)
    {
        Username = userName;
        Email = email;
        EmailConfirmed = false;
        Password = password;
        PasswordSalt = passwordSalt;
    }

    public User(int id, string username, string email, bool emailConfirmed, byte[] password, byte[] passwordSalt, bool isAdmin)
    {
        Id = id;
        Username = username;
        Email = email;
        EmailConfirmed = emailConfirmed;
        Password = password;
        PasswordSalt = passwordSalt;
        IsAdmin = isAdmin;
    }
}