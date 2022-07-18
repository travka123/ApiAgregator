using System.ComponentModel.DataAnnotations;

namespace ApiAgregator.WebApi.Models.Request
{
    public class SignUpRequest
    {
        [Required(ErrorMessage = "must be specified")]
        [MinLength(5, ErrorMessage = "length must be >= 5")]
        [MaxLength(100, ErrorMessage = "length must be <= 100")]
        [RegularExpression("^[A-Za-z][A-Za-z0-9_]*$", ErrorMessage = "must be [A-Za-z][A-Za-z0-9_]*")]
        public string Username { get; set; } = default!;

        [Required(ErrorMessage = "must be specified")]
        [EmailAddress(ErrorMessage = "must be valid email address")]
        public string Email { get; set; } = default!;

        [Required(ErrorMessage = "must be specified")]
        public byte[] Password { get; set; } = default!;
    }
}
