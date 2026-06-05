using System.Linq;
using StarDust.CasparCG.Protocol.Amcp;
using StarDust.CasparCG.Query;
using Xunit;

namespace StarDust.CasparCG.UnitTests;

public sealed class CasparQueryResultParserTests
{
    [Fact]
    public void ParseMediaFiles_parses_complete_cls_lines()
    {
        var response = AmcpResponseParser.Parse(
            "200 CLS OK\r\n" +
            "\"AMB\" MOVIE 42 20240101120000 240 1/25\r\n" +
            "\"PROMO WITH SPACE\" STILL 13 20240101120001\r\n\r\n");

        var result = CasparQueryResultParser.ParseMediaFiles(response);

        Assert.Equal(2, result.Count);
        Assert.Equal("AMB", result[0].Name);
        Assert.Equal(MediaFileKind.Movie, result[0].Kind);
        Assert.Equal("PROMO WITH SPACE", result[1].Name);
        Assert.Equal(MediaFileKind.Still, result[1].Kind);
    }

    [Fact]
    public void ParseMediaFiles_throws_for_malformed_cls_line()
    {
        var response = AmcpResponseParser.Parse("200 CLS OK\r\nBROKEN_LINE\r\n\r\n");

        var exception = Assert.Throws<CasparQueryParseException>(
            () => CasparQueryResultParser.ParseMediaFiles(response));

        Assert.Contains("BROKEN_LINE", exception.Message);
    }

    [Fact]
    public void ParseMediaInfo_preserves_extra_fields_as_properties()
    {
        var response = AmcpResponseParser.Parse(
            "200 CINF OK\r\n" +
            "\"AMB\" MOVIE 42 20240101120000 240 1/25 FIELD_A VALUE_A FIELD_B VALUE_B\r\n\r\n");

        var result = CasparQueryResultParser.ParseMediaInfo(response);

        Assert.Equal("AMB", result.Name);
        Assert.Equal("VALUE_A", result.Properties["FIELD_A"]);
        Assert.Equal("VALUE_B", result.Properties["FIELD_B"]);
    }

    [Fact]
    public void ParseTemplateFiles_returns_template_records()
    {
        var response = AmcpResponseParser.Parse("200 TLS OK\r\nLOWERTHIRD\r\nFULLFRAME\r\n\r\n");

        var result = CasparQueryResultParser.ParseTemplateFiles(response);

        Assert.Equal(["LOWERTHIRD", "FULLFRAME"], result.Select(x => x.Name).ToArray());
    }

    [Fact]
    public void ParseFontFiles_returns_font_records()
    {
        var response = AmcpResponseParser.Parse("200 FLS OK\r\n\"Roboto\" fonts/roboto.ttf\r\n\r\n");

        var result = CasparQueryResultParser.ParseFontFiles(response);

        Assert.Equal("Roboto", result.Single().Name);
        Assert.Equal("fonts/roboto.ttf", result.Single().Path);
    }

    [Fact]
    public void ParseQueryDataMap_flattens_xml_payloads()
    {
        var response = AmcpResponseParser.Parse(
            "200 INFO PATHS OK\r\n" +
            "<paths><media-path>media/</media-path><log-path>log/</log-path></paths>\r\n\r\n");

        var result = CasparQueryResultParser.ParseQueryDataMap(response);

        Assert.Equal("media/", result.Values["paths.media-path"]);
        Assert.Equal("log/", result.Values["paths.log-path"]);
    }

    [Fact]
    public void ParseQueryDataMap_skips_ambiguous_repeated_xml_elements()
    {
        var response = AmcpResponseParser.Parse(
            "200 INFO CONFIG OK\r\n" +
            "<configuration><paths><media-path>media/</media-path></paths><channels><channel><video-mode>1080i5000</video-mode></channel><channel><video-mode>720p5000</video-mode></channel></channels></configuration>\r\n\r\n");

        var result = CasparQueryResultParser.ParseQueryDataMap(response);

        Assert.Equal("media/", result.Values["configuration.paths.media-path"]);
        Assert.DoesNotContain("configuration.channels.channel.video-mode", result.Values.Keys);
    }

    [Fact]
    public void ParseQueryDataMap_parses_key_value_text_payloads()
    {
        var response = AmcpResponseParser.Parse(
            "201 GL INFO OK\r\n" +
            "renderer: opengl\r\n" +
            "vendor=casparcg\r\n");

        var result = CasparQueryResultParser.ParseQueryDataMap(response);

        Assert.Equal("opengl", result.Values["renderer"]);
        Assert.Equal("casparcg", result.Values["vendor"]);
    }

    [Fact]
    public void ParseQueryDataMap_keeps_lines_when_payload_is_not_fully_recognized()
    {
        var response = AmcpResponseParser.Parse("201 GL INFO OK\r\nrenderer-info vendor-info opaque-token\r\n");

        var result = CasparQueryResultParser.ParseQueryDataMap(response);

        Assert.Single(result.Lines);
        Assert.Equal("renderer-info vendor-info opaque-token", result.Lines[0]);
        Assert.Equal("201 GL INFO OK\r\nrenderer-info vendor-info opaque-token\r\n", result.Raw);
    }
}
