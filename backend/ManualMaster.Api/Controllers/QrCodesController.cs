using ManualMaster.Api.Dtos;
using ManualMaster.Api.Services.QrCodes;
using Microsoft.AspNetCore.Mvc;

namespace ManualMaster.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QrCodesController : ControllerBase
{
    private readonly IQrCodeService _qrCodeService;

    public QrCodesController(IQrCodeService qrCodeService)
    {
        _qrCodeService = qrCodeService;
    }

    [HttpPost("generate")]
    public async Task<IActionResult> Generate([FromBody] QrCodeGenerateRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var response = await _qrCodeService.GenerateAsync(request, cancellationToken);
        return Ok(response);
    }

    [HttpPost("decode")]
    public async Task<IActionResult> Decode([FromBody] QrDecodeRequest request, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var text = await _qrCodeService.DecodeAsync(request.Base64Image, cancellationToken);
        return text is null ? BadRequest(new { message = "Unable to decode QR image." }) : Ok(new { text });
    }
}
