using System.ComponentModel.DataAnnotations;

namespace ManualMaster.Api.Dtos;

public class QrCodeGenerateRequest
{
    [Required]
    public string Payload { get; set; } = string.Empty;

    public int PixelsPerModule { get; set; } = 10;
}
