using System.ComponentModel.DataAnnotations;

namespace ApiAgregator.WebApi.Models.Request;

public class UserTaskUpdateRequest
{
    [Required]
    public string ApiName { get; set; } = default!;

    [Required]
    public string Name { get; set; } = default!;

    [Required(AllowEmptyStrings = true)]
    public string Description { get; set; } = default!;

    [Required(AllowEmptyStrings = true)]
    public string Expression { get; set; } = default!;

    [Required]
    public Dictionary<string, string> Parameters { get; set; } = default!;
}
