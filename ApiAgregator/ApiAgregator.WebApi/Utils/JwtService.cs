using ApiAgregator.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApiAgregator.WebApi.Utils;

public class JwtService
{
    public const string ISSUER = "ApiAgregatorServer";
    public const string AUDIENCE = "ApiAgregatorClient";
 
    private readonly SymmetricSecurityKey _key;

    public JwtService(IOptions<JwtServiceOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options.Value.Key);

        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Value.Key));
    }

    public string CreateToken(User user)
    {
        var claims = new List<Claim>();

        claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
        claims.Add(new Claim(ClaimTypes.Name, user.Username));
        claims.Add(new Claim(ClaimTypes.Role, user.IsAdmin ? "admin" : "user"));

        if (user.EmailConfirmed)
            claims.Add(new Claim(ClaimTypes.Email, user.Email));

        return CreateToken(claims);
    }

    private string CreateToken(List<Claim> claims)
    {
        var jwt = new JwtSecurityToken(
            issuer: ISSUER,
            audience: AUDIENCE,
            claims: claims,
            signingCredentials: new SigningCredentials(_key, SecurityAlgorithms.HmacSha256));
        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }

    public static TokenValidationParameters CreateTokenValidationParameters(string key)
    {
        return new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = ISSUER,
            ValidateAudience = true,
            ValidAudience = AUDIENCE,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
        };
    }
}

public class JwtServiceOptions
{
    public string? Key { get; set; }
}