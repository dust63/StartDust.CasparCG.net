namespace StarDust.CasparCG.Query;

/// <summary>
/// Represents a font catalog entry returned by <c>FLS</c>.
/// </summary>
public sealed record FontFile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FontFile"/> record.
    /// </summary>
    /// <param name="name">The font display name.</param>
    /// <param name="path">The source path reported by CasparCG.</param>
    public FontFile(string name, string path)
    {
        Name = name;
        Path = path;
    }

    /// <summary>
    /// Gets the font display name.
    /// </summary>
    public string Name { get; init; }

    /// <summary>
    /// Gets the source path reported by CasparCG.
    /// </summary>
    public string Path { get; init; }
}
