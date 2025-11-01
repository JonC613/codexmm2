using ManualMaster.Api.Dtos;

namespace ManualMaster.Api.Services.QrCodes;

public interface IQrCodeService
{
    Task<QrCodeResponse> GenerateAsync(QrCodeGenerateRequest request, CancellationToken cancellationToken);
    Task<string?> DecodeAsync(string base64Image, CancellationToken cancellationToken);
}
