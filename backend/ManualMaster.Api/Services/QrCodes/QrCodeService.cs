using ManualMaster.Api.Dtos;
using QRCoder;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace ManualMaster.Api.Services.QrCodes;

public class QrCodeService : IQrCodeService
{
    private readonly ILogger<QrCodeService> _logger;

    public QrCodeService(ILogger<QrCodeService> logger)
    {
        _logger = logger;
    }

    public Task<QrCodeResponse> GenerateAsync(QrCodeGenerateRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Payload))
        {
            throw new InvalidOperationException("QR payload cannot be empty.");
        }

        using var qrGenerator = new QRCodeGenerator();
        using var qrCodeData = qrGenerator.CreateQrCode(request.Payload, QRCodeGenerator.ECCLevel.Q);
        using var qrCode = new PngByteQRCode(qrCodeData);
        var pngBytes = qrCode.GetGraphic(Math.Clamp(request.PixelsPerModule, 4, 20));
        var base64 = Convert.ToBase64String(pngBytes);
        return Task.FromResult(new QrCodeResponse(base64));
    }

    public Task<string?> DecodeAsync(string base64Image, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(base64Image))
        {
            return Task.FromResult<string?>(null);
        }

        try
        {
            var imageBytes = Convert.FromBase64String(base64Image);
            using var image = Image.Load<Rgba32>(imageBytes);
            var pixelData = new byte[image.Width * image.Height * 4];
            image.CopyPixelDataTo(pixelData);

            var luminance = new RGBLuminanceSource(pixelData, image.Width, image.Height, RGBLuminanceSource.BitmapFormat.RGBA32);
            var binaryBitmap = new BinaryBitmap(new HybridBinarizer(luminance));
            var reader = new QRCodeReader();
            var result = reader.decode(binaryBitmap);
            return Task.FromResult(result?.Text);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to decode QR image");
            return Task.FromResult<string?>(null);
        }
    }
}
