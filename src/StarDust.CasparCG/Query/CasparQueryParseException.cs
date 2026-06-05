namespace StarDust.CasparCG.Query;

/// <summary>
/// Represents an error raised while projecting an AMCP query response into a typed result.
/// </summary>
public sealed class CasparQueryParseException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CasparQueryParseException"/> class.
    /// </summary>
    /// <param name="message">The parser failure message.</param>
    public CasparQueryParseException(string message)
        : base(message)
    {
    }
}
