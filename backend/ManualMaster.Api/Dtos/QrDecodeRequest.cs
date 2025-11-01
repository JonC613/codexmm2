using System.ComponentModel.DataAnnotations;

namespace ManualMaster.Api.Dtos;

public class QrDecodeRequest
{
    [Required]
    public string Base64Image { get; set; } = string.Empty;
}
