namespace StarDust.CasparCG.AspNetCore.Contracts;

/// <summary>
/// Represents a JSON request to add and optionally play a CG template.
/// </summary>
/// <param name="Template">The template identifier.</param>
/// <param name="PlayOnLoad">Whether the template should play immediately.</param>
/// <param name="TemplateData">The structured template data payload.</param>
public sealed record CgAddJsonRequest(string Template, bool PlayOnLoad, TemplateDataEnvelope TemplateData);
