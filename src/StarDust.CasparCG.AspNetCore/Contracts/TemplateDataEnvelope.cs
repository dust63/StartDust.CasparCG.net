namespace StarDust.CasparCG.AspNetCore.Contracts;

/// <summary>
/// Represents structured template data that can be translated into CasparCG XML.
/// </summary>
/// <param name="Components">The template components to render.</param>
public sealed record TemplateDataEnvelope(IReadOnlyList<TemplateDataEnvelope.TemplateComponent> Components)
{
    /// <summary>
    /// Represents one template component.
    /// </summary>
    /// <param name="Id">The component identifier.</param>
    /// <param name="Data">The values assigned to the component.</param>
    public sealed record TemplateComponent(string Id, IReadOnlyList<TemplateValue> Data);

    /// <summary>
    /// Represents one named value inside a component.
    /// </summary>
    /// <param name="Id">The data field identifier.</param>
    /// <param name="Value">The serialized data field value.</param>
    public sealed record TemplateValue(string Id, string Value);
}
