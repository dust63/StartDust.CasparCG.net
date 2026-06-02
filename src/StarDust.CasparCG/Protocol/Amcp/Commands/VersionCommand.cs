namespace StarDust.CasparCG.Protocol.Amcp.Commands;

/// <summary>
/// Represents a VERSION command.
/// </summary>
/// <param name="Component">The optional version target.</param>
public sealed record VersionCommand(string? Component = null) : AmcpCommand
{
    /// <inheritdoc />
    public override string Serialize() =>
        string.IsNullOrWhiteSpace(Component)
            ? "VERSION\r\n"
            : $"VERSION {Component}\r\n";
}
