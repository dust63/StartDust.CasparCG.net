using System.Text;
using StarDust.CasparCG.AspNetCore.Internal;
using Xunit;

namespace StarDust.CasparCG.UnitTests;

public sealed class RestApiThumbnailResponseParserTests
{
    [Fact]
    public void Parse_list_returns_structured_items()
    {
        const string response = """
        200 THUMBNAIL LIST OK
        "AMB" 42 20240101T120000
        "BKG" 84 20240101T121500

        """;

        var items = ThumbnailResponseParser.ParseList(response);

        Assert.Collection(
            items,
            item =>
            {
                Assert.Equal("AMB", item.Name);
                Assert.Equal(42, item.SizeBytes);
            },
            item =>
            {
                Assert.Equal("BKG", item.Name);
                Assert.Equal(84, item.SizeBytes);
            });
    }

    [Fact]
    public void Parse_retrieve_decodes_base64_payload()
    {
        var bytes = ThumbnailResponseParser.ParseBinary(
            "201 THUMBNAIL RETRIEVE OK\r\nSGVsbG8=\r\n");

        Assert.Equal("Hello", Encoding.UTF8.GetString(bytes));
    }
}
