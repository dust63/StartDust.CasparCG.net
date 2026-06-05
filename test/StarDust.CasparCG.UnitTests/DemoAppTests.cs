using StarDust.CasparCG.Demo;
using Xunit;

namespace StarDust.CasparCG.UnitTests;

public class DemoAppTests
{
    [Fact]
    public void ResolveShowcaseConnection_uses_prompts_when_connection_values_are_default()
    {
        var options = new DemoApp.DemoOptions(
            "127.0.0.1",
            5250,
            6250,
            1,
            10,
            "AMB",
            8,
            false);

        var asked = new List<string>();

        var connection = DemoApp.ResolveShowcaseConnection(
            options,
            askString: (label, defaultValue) =>
            {
                asked.Add($"{label}:{defaultValue}");
                return label switch
                {
                    "host" => "10.0.0.5",
                    _ => defaultValue
                };
            },
            askInt: (label, defaultValue) =>
            {
                asked.Add($"{label}:{defaultValue}");
                return label switch
                {
                    "amcp-port" => 6001,
                    "osc-port" => 7001,
                    _ => defaultValue
                };
            });

        Assert.Equal(("10.0.0.5", 6001, 7001), connection);
        Assert.Equal(3, asked.Count);
    }

    [Fact]
    public void ResolveShowcaseConnection_uses_arguments_without_prompting_when_connection_values_are_provided()
    {
        var options = new DemoApp.DemoOptions(
            "10.0.0.5",
            6001,
            7001,
            1,
            10,
            "AMB",
            8,
            false);

        var connection = DemoApp.ResolveShowcaseConnection(
            options,
            askString: (_, _) => throw new InvalidOperationException("prompt should not be used"),
            askInt: (_, _) => throw new InvalidOperationException("prompt should not be used"));

        Assert.Equal(("10.0.0.5", 6001, 7001), connection);
    }
}
