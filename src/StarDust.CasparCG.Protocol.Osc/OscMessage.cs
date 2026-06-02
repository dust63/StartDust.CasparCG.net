namespace StarDust.CasparCG.Protocol.Osc;

/// <summary>
/// Represents a parsed OSC message.
/// </summary>
/// <param name="Address">The OSC address.</param>
/// <param name="Arguments">The parsed OSC arguments.</param>
public sealed record OscMessage(string Address, IReadOnlyList<object?> Arguments);
