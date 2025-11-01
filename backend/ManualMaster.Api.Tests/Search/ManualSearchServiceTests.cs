using ManualMaster.Api.Services.Search;
using Xunit;

namespace ManualMaster.Api.Tests.Search;

public class ManualSearchServiceTests
{
    private readonly ManualSearchService _service = new();

    [Theory]
    [InlineData(null, null)]
    [InlineData("   ", null)]
    [InlineData("Acme   Coffee  Maker", "Acme Coffee Maker")]
    public void NormalizeSearchTerm_ReturnsExpected(string? input, string? expected)
    {
        var result = _service.NormalizeSearchTerm(input);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void NormalizeSearchTerm_TruncatesLongInput()
    {
        var input = new string('a', 400);
        var result = _service.NormalizeSearchTerm(input);
        Assert.Equal(255, result!.Length);
    }
}
