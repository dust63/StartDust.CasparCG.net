using System.Net.Http.Json;
using System.Text;
using StarDust.CasparCG.AspNetCore.Contracts;
using Xunit;

namespace StarDust.CasparCG.IntegrationTests;

public sealed class CasparRestApiCgTests
{
    [Fact]
    public async Task Post_cg_add_with_json_sends_expected_command()
    {
        await using var fixture = await CasparRestApiTestHost.StartAsync(scenario => scenario
            .WithAmcpReply(
                "CG ADD 1-10 LowerThird 1 <templateData><componentData id=\"f0\"><data id=\"headline\" value=\"Hello\" /></componentData></templateData>",
                "202 CG ADD OK\r\n"));

        var response = await fixture.Client.PostAsJsonAsync(
            "/channels/1/layers/10/cg/add",
            new CgAddJsonRequest(
                "LowerThird",
                true,
                new TemplateDataEnvelope(
                    [
                        new("f0", [new("headline", "Hello")])
                    ])));

        response.EnsureSuccessStatusCode();
        Assert.Equal(
            ["CG ADD 1-10 LowerThird 1 <templateData><componentData id=\"f0\"><data id=\"headline\" value=\"Hello\" /></componentData></templateData>"],
            fixture.ReceivedCommands);
    }

    [Fact]
    public async Task Post_cg_update_with_xml_body_sends_expected_command()
    {
        await using var fixture = await CasparRestApiTestHost.StartAsync(scenario => scenario
            .WithAmcpReply(
                "CG UPDATE 1-10 <templateData><componentData id=\"f0\" /></templateData>",
                "201 CG UPDATE OK\r\nUPDATED\r\n"));

        using var content = new StringContent("<templateData><componentData id=\"f0\" /></templateData>", Encoding.UTF8, "application/xml");
        var response = await fixture.Client.PostAsync("/channels/1/layers/10/cg/update", content);

        response.EnsureSuccessStatusCode();
        Assert.Equal(
            ["CG UPDATE 1-10 <templateData><componentData id=\"f0\" /></templateData>"],
            fixture.ReceivedCommands);
    }

    [Fact]
    public async Task Post_cg_invoke_sends_expected_command()
    {
        await using var fixture = await CasparRestApiTestHost.StartAsync(scenario => scenario
            .WithAmcpReply("CG INVOKE 1-10 next()", "202 CG INVOKE OK\r\n"));

        var response = await fixture.Client.PostAsJsonAsync(
            "/channels/1/layers/10/cg/invoke",
            new CgInvokeRequest("next()"));

        response.EnsureSuccessStatusCode();
        Assert.Equal(["CG INVOKE 1-10 next()"], fixture.ReceivedCommands);
    }
}
