namespace StarDust.CasparCG.AspNetCore.Contracts;

/// <summary>
/// Represents a JSON request to update the data of an existing CG template.
/// </summary>
/// <param name="TemplateData">The structured template data payload.</param>
public sealed record CgUpdateJsonRequest(TemplateDataEnvelope TemplateData);
