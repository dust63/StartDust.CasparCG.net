using System.Text;
using Microsoft.AspNetCore.Http;
using StarDust.CasparCG.AspNetCore.Internal;
using Xunit;

namespace StarDust.CasparCG.UnitTests;

public sealed class RestApiCgPayloadTranslatorTests
{
    [Fact]
    public async Task Translate_add_json_builds_expected_template_xml()
    {
        var context = new DefaultHttpContext();
        context.Request.ContentType = "application/json";
        context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(
            """
            {
              "template": "LowerThird",
              "playOnLoad": true,
              "templateData": {
                "components": [
                  {
                    "id": "f0",
                    "data": [
                      { "id": "headline", "value": "Hello" }
                    ]
                  }
                ]
              }
            }
            """));

        var result = await CgPayloadTranslator.TranslateAddAsync(context.Request, CancellationToken.None);

        Assert.Equal("LowerThird", result.Template);
        Assert.True(result.PlayOnLoad);
        Assert.Equal(
            "<templateData><componentData id=\"f0\"><data id=\"headline\" value=\"Hello\" /></componentData></templateData>",
            result.TemplateXml);
    }

    [Fact]
    public async Task Translate_add_xml_uses_query_metadata_and_raw_body()
    {
        var context = new DefaultHttpContext();
        context.Request.ContentType = "application/xml";
        context.Request.QueryString = new QueryString("?template=LowerThird&playOnLoad=true");
        context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes("<templateData><componentData id=\"f0\" /></templateData>"));

        var result = await CgPayloadTranslator.TranslateAddAsync(context.Request, CancellationToken.None);

        Assert.Equal("LowerThird", result.Template);
        Assert.True(result.PlayOnLoad);
        Assert.Equal("<templateData><componentData id=\"f0\" /></templateData>", result.TemplateXml);
    }

    [Fact]
    public async Task Translate_update_rejects_unsupported_content_type()
    {
        var context = new DefaultHttpContext();
        context.Request.ContentType = "text/plain";
        context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes("nope"));

        var exception = await Assert.ThrowsAsync<BadHttpRequestException>(
            () => CgPayloadTranslator.TranslateUpdateAsync(context.Request, CancellationToken.None).AsTask());

        Assert.Contains("Unsupported content type", exception.Message);
    }
}
