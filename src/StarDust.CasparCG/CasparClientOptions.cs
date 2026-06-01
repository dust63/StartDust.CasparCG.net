namespace StarDust.CasparCG;

/// <summary>
/// Stores the configurable connection settings for a CasparCG client registration.
/// </summary>
public sealed class CasparClientOptions
{
    /// <summary>
    /// Gets or sets the registration name.
    /// </summary>
    public string Name { get; init; } = "default";

    /// <summary>
    /// Gets or sets the AMCP host name.
    /// </summary>
    public string AmcpHost { get; set; } = "127.0.0.1";

    /// <summary>
    /// Gets or sets the AMCP port.
    /// </summary>
    public int AmcpPort { get; set; } = 5250;

    /// <summary>
    /// Gets or sets the OSC port.
    /// </summary>
    public int OscPort { get; set; } = 6250;

    /// <summary>
    /// Gets or sets a value indicating whether auto reconnect is enabled.
    /// </summary>
    public bool AutoReconnect { get; set; } = true;
}
