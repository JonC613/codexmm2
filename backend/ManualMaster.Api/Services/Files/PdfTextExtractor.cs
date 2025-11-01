using UglyToad.PdfPig;

namespace ManualMaster.Api.Services;

public class PdfTextExtractor
{
    public Task<string?> TryExtractTextAsync(Stream stream, CancellationToken cancellationToken)
    {
        try
        {
            using var document = PdfDocument.Open(stream, new ParsingOptions() { UseLenientParsing = true });
            var text = string.Join(Environment.NewLine, document.GetPages().Select(p => p.Text));
            if (string.IsNullOrWhiteSpace(text))
            {
                return Task.FromResult<string?>(null);
            }

            return Task.FromResult<string?>(text.Trim());
        }
        catch
        {
            return Task.FromResult<string?>(null);
        }
    }
}
