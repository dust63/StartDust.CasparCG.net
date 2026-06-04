namespace StarDust.CasparCG.Query;

/// <summary>
/// Represents a template catalog entry returned by <c>TLS</c>.
/// </summary>
public sealed record TemplateFile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TemplateFile"/> record.
    /// </summary>
    /// <param name="name">The template name.</param>
    public TemplateFile(string name)
    {
        Name = name;
    }

    /// <summary>
    /// Gets the template name.
    /// </summary>
    public string Name { get; init; }
}
