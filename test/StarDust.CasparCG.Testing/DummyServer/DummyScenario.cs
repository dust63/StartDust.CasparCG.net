namespace StarDust.CasparCG.Testing.DummyServer;

/// <summary>
/// Stores scripted AMCP replies for the dummy server.
/// </summary>
public sealed class DummyScenario
{
    private readonly Dictionary<string, string> _amcpReplies = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Creates an empty dummy scenario.
    /// </summary>
    /// <returns>An empty scenario.</returns>
    public static DummyScenario Empty() => new();

    /// <summary>
    /// Adds a scripted reply for a command.
    /// </summary>
    /// <param name="command">The AMCP command text.</param>
    /// <param name="reply">The reply to send.</param>
    /// <returns>The current scenario.</returns>
    public DummyScenario WithAmcpReply(string command, string reply)
    {
        _amcpReplies[command] = reply;
        return this;
    }

    internal bool TryGetReply(string command, out string reply) => _amcpReplies.TryGetValue(command, out reply!);
}
