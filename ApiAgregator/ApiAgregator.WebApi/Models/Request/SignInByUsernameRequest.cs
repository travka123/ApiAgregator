using System.ComponentModel.DataAnnotations;

namespace ApiAgregator.WebApi.Models.Request;

public class SignInByUsernameRequest
{
    [Required]
    public string Username { get; set; } = default!;

    [Required]
    public byte[] Password { get; set; } = default!;
}
