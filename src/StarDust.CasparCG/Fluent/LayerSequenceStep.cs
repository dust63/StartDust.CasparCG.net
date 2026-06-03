namespace StarDust.CasparCG.Fluent;

/// <summary>
/// Represents a local step within a layer sequence.
/// </summary>
internal abstract record LayerSequenceStep;

/// <summary>
/// Represents a local delay in a layer sequence.
/// </summary>
/// <param name="Delay">The delay to wait before advancing.</param>
internal sealed record DelayStep(TimeSpan Delay) : LayerSequenceStep;

/// <summary>
/// Represents a command step in a layer sequence.
/// </summary>
/// <param name="SendAsync">The command execution delegate.</param>
internal sealed record CommandStep(Func<CancellationToken, ValueTask> SendAsync) : LayerSequenceStep;
