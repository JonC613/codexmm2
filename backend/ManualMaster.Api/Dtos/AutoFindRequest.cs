using System.ComponentModel.DataAnnotations;

namespace ManualMaster.Api.Dtos;

public class AutoFindRequest
{
    [Required]
    [MaxLength(255)]
    public string ProductName { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? ModelNumber { get; set; }
}
