using StarDust.CasparCG.Protocol.Amcp;
using Xunit;

namespace StarDust.CasparCG.UnitTests;

public sealed class AmcpResponseParserTests
{
    [Fact]
    public void Parse_202_response_has_success_without_payload()
    {
        var response = AmcpResponseParser.Parse("202 PLAY OK\r\n");

        Assert.Equal(202, response.StatusCode);
        Assert.Equal(AmcpStatusCategory.Success, response.Category);
        Assert.True(response.IsSuccess);
        Assert.Equal("PLAY", response.CommandText);
        Assert.Empty(response.Lines);
    }

    [Fact]
    public void Parse_201_response_has_single_payload_line()
    {
        var response = AmcpResponseParser.Parse("201 VERSION OK\r\n2.4.1 Stable\r\n");

        Assert.Equal(201, response.StatusCode);
        Assert.Single(response.Lines);
        Assert.Equal("2.4.1 Stable", response.Lines[0]);
    }

    [Fact]
    public void Parse_200_response_has_multiline_payload()
    {
        var response = AmcpResponseParser.Parse("200 CLS OK\r\n\"AMB\" MOVIE 42 20240101120000 240 1/25\r\n\r\n");

        Assert.Equal(200, response.StatusCode);
        Assert.Single(response.Lines);
        Assert.Equal("\"AMB\" MOVIE 42 20240101120000 240 1/25", response.Lines[0]);
    }

    [Fact]
    public void Parse_200_query_response_keeps_xml_payload_lines()
    {
        var response = AmcpResponseParser.Parse("200 INFO CONFIG OK\r\n<config><channel>1</channel></config>\r\n\r\n");

        Assert.Equal(200, response.StatusCode);
        Assert.Equal("INFO CONFIG", response.CommandText);
        Assert.Equal("<config><channel>1</channel></config>", response.Lines[0]);
        Assert.Equal("200 INFO CONFIG OK\r\n<config><channel>1</channel></config>\r\n\r\n", response.Raw);
    }

    [Fact]
    public void Parse_404_response_preserves_error_status()
    {
        var response = AmcpResponseParser.Parse("404 PLAY FAILED\r\n");

        Assert.Equal(404, response.StatusCode);
        Assert.Equal(AmcpStatusCategory.ClientError, response.Category);
        Assert.False(response.IsSuccess);
        Assert.Equal("PLAY FAILED", response.CommandText);
    }
}
