using StarDust.CasparCG.Events;

namespace StarDust.CasparCG.Osc;

/// <summary>
/// Maps parsed OSC messages into domain events.
/// </summary>
public interface IOscMessageMapper
{
    /// <summary>
    /// Maps a raw OSC message into zero or more domain events.
    /// </summary>
    /// <param name="address">The OSC address.</param>
    /// <param name="arguments">The OSC arguments.</param>
    /// <returns>The mapped domain events.</returns>
    IReadOnlyList<CasparEvent> Map(string address, IReadOnlyList<object?> arguments);
}
