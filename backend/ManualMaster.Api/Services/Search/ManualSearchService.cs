using System.Text.RegularExpressions;

namespace ManualMaster.Api.Services.Search;

public class ManualSearchService : IManualSearchService
{
    private static readonly Regex MultiSpace = new("\\s+", RegexOptions.Compiled);

    public string? NormalizeSearchTerm(string? term)
    {
        if (string.IsNullOrWhiteSpace(term))
        {
            return null;
        }

        var normalized = MultiSpace.Replace(term.Trim(), " ");
        return normalized.Length > 255 ? normalized[..255] : normalized;
    }
}
