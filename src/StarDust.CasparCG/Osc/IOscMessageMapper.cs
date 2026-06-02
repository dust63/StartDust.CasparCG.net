using StarDust.CasparCG.Events;

namespace StarDust.CasparCG.Osc;

/// <summary>
/// Maps parsed OSC messages into domain events.
/// </summary>
public interface IOscMessageMapper
{
    /// <summary>
    /// Attempts to map a raw OSC message into a domain event.
    /// </summary>
    /// <param name="address">The OSC address.</param>
    /// <param name="arguments">The OSC arguments.</param>
    /// <param name="evt">The mapped domain event, if successful.</param>
    /// <returns><see langword="true"/> when a domain event was produced.</returns>
    bool TryMap(string address, IReadOnlyList<object?> arguments, out CasparEvent? evt);
}
