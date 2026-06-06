namespace StarDust.CasparCG.AspNetCore.Contracts;

/// <summary>
/// Represents a request to invoke a method on the active CG template.
/// </summary>
/// <param name="Method">The method name to invoke.</param>
public sealed record CgInvokeRequest(string Method);
