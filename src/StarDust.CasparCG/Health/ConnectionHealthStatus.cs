namespace StarDust.CasparCG.Health;

/// <summary>
/// Represents the connection health state for a client.
/// </summary>
public enum ConnectionHealthStatus
{
    /// <summary>
    /// The client is disconnected.
    /// </summary>
    Disconnected,

    /// <summary>
    /// The client is connecting.
    /// </summary>
    Connecting,

    /// <summary>
    /// The client is connected.
    /// </summary>
    Connected,

    /// <summary>
    /// The client is connected but degraded.
    /// </summary>
    Degraded,

    /// <summary>
    /// The client is reconnecting.
    /// </summary>
    Reconnecting,

    /// <summary>
    /// The client encountered a fault.
    /// </summary>
    Faulted,

    /// <summary>
    /// The client is stopping.
    /// </summary>
    Stopping
}
