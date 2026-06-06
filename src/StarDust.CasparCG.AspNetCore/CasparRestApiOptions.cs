namespace StarDust.CasparCG.AspNetCore;

/// <summary>
/// Configures the REST and SSE surface exposed for a CasparCG client.
/// </summary>
public sealed class CasparRestApiOptions
{
    /// <summary>
    /// Gets or sets the optional route prefix used when mapping the API.
    /// </summary>
    public string RoutePrefix { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether admin endpoints are mapped.
    /// </summary>
    public bool MapAdminEndpoints { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the SSE endpoint is mapped.
    /// </summary>
    public bool EnableSse { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether the OpenAPI endpoint is mapped.
    /// </summary>
    public bool EnableOpenApi { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether configured clients should be warmed up at startup.
    /// </summary>
    public bool WarmUpClientsOnStartup { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether health endpoints are mapped.
    /// </summary>
    public bool MapHealthEndpoints { get; set; } = true;
}
