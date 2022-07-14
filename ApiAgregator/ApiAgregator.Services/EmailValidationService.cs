using ApiAgregator.Services.Exceptions;
using ApiAgregator.Services.Repositories;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApiAgregator.Services;

public class EmailValidationService
{
    private const string ISSUER = "ApiAgregator.EmailValidationService";
    private const string AUDIENCE = "ApiAgregator.EmailValidationService.Clients";

    private readonly string _url;
    private readonly SymmetricSecurityKey _key;
    private readonly TokenValidationParameters _parameters;
    private readonly IEmailSenderService _emailSenderService;
    private readonly IUserRepository _userRepository;

    public EmailValidationService(IOptions<EmailValidationServiceOptions> options,
        IEmailSenderService emailSenderService, IUserRepository userRepository)
    {
        ArgumentNullException.ThrowIfNull(options.Value.Key);
        ArgumentNullException.ThrowIfNull(options.Value.Url);

        _url = options.Value.Url;
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Value.Key));
        _parameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = ISSUER,
            ValidateAudience = true,
            ValidAudience = AUDIENCE,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = _key
        };
        _emailSenderService = emailSenderService;
        _userRepository = userRepository;
    }

    public void RequestEmailValidation(int userId)
    {
        var user = _userRepository.GetById(userId);

        if (user.EmailConfirmed)
            throw new EmailAlreadyConfirmedException();

        var token = CreateToken(user.Username, user.Email);
        _emailSenderService.Send(user.Email, "Email check", $"Welcome to ApiAgregator, {user.Username}!\n\n" +
            "To complete the registration process, please follow the link " +
            $"{_url}/{token}");
    }

    public void ValidateEmail(string token)
    {
        string username, email;
        try
        {
            (username, email) = ValidateToken(token);
        }
        catch 
        {
            throw new EmailValidationTokenException();
        }
        _userRepository.ConfirmEmail(username, email);
    }

    private string CreateToken(string username, string email)
    {
        var jwt = new JwtSecurityToken(
            issuer: ISSUER,
            audience: AUDIENCE,
            claims: new List<Claim> { new Claim(ClaimTypes.Name, username), new Claim(ClaimTypes.Email, email) },
            signingCredentials: new SigningCredentials(_key, SecurityAlgorithms.HmacSha256));
        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }

    private (string username, string email) ValidateToken(string token)
    {
        var claimsPrincipal = new JwtSecurityTokenHandler().ValidateToken(token, _parameters, out var _);
        string username = claimsPrincipal.FindFirst(ClaimTypes.Name)!.Value;
        string email = claimsPrincipal.FindFirst(ClaimTypes.Email)!.Value;
        return (username, email);
    }
}

public class EmailValidationServiceOptions
{
    public string? Key { get; set; }
    public string? Url { get; set; }
}
