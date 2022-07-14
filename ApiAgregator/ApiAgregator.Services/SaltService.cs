using System.Security.Cryptography;

namespace ApiAgregator.Services;

public class SaltService
{
    private readonly byte[] _appSalt;
    private readonly int _userSaltSize;

    public SaltService(byte[] appSalt, int userSaltSize)
    {
        _appSalt = appSalt;
        _userSaltSize = userSaltSize;
    }

    public bool Compare(byte[] saltedPassword, byte[] userSalt, byte[] password)
    {
        return saltedPassword.SequenceEqual(SaltPassword(password, userSalt));
    }

    public byte[] CreateUserSalt()
    {   
        return RandomNumberGenerator.GetBytes(_userSaltSize);
    }
    
    public byte[] SaltPassword(byte[] password, byte[] userSalt)
    {
        byte[] bytes = new byte[password.Length + _appSalt.Length + userSalt.Length];
        Buffer.BlockCopy(password, 0, bytes, 0, password.Length);
        Buffer.BlockCopy(_appSalt, 0, bytes, password.Length, _appSalt.Length);
        Buffer.BlockCopy(userSalt, 0, bytes, password.Length + _appSalt.Length, userSalt.Length);
        return SHA512.HashData(bytes);   
    }
}
