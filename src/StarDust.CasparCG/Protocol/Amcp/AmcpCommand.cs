namespace StarDust.CasparCG.Protocol.Amcp;

/// <summary>
/// Represents a serializable AMCP command.
/// </summary>
public abstract record AmcpCommand
{
    /// <summary>
    /// Serializes the command to the AMCP wire format.
    /// </summary>
    /// <returns>The AMCP command text.</returns>
    public abstract string Serialize();

    /// <summary>
    /// Formats a channel-layer address.
    /// </summary>
    /// <param name="channel">The channel number.</param>
    /// <param name="layer">The layer number.</param>
    /// <returns>The AMCP address text.</returns>
    protected static string Address(int channel, int layer) => $"{channel}-{layer}";
}
