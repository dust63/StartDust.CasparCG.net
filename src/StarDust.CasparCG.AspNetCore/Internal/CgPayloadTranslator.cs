using System.Text.Json;
using System.Xml.Linq;
using Microsoft.AspNetCore.Http;
using StarDust.CasparCG.AspNetCore.Contracts;

namespace StarDust.CasparCG.AspNetCore.Internal;

internal static class CgPayloadTranslator
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static async ValueTask<CgPayloadTranslationResult> TranslateAddAsync(
        HttpRequest request,
        CancellationToken cancellationToken)
    {
        if (IsJson(request.ContentType))
        {
            var payload = await JsonSerializer.DeserializeAsync<CgAddJsonRequest>(
                    request.Body,
                    JsonOptions,
                    cancellationToken: cancellationToken)
                ?? throw new BadHttpRequestException("Request body is required.");

            return new(payload.Template, payload.PlayOnLoad, ToTemplateXml(payload.TemplateData));
        }

        if (IsXml(request.ContentType))
        {
            var template = request.Query["template"].ToString();
            var playOnLoad = bool.TryParse(request.Query["playOnLoad"], out var parsed) && parsed;

            if (string.IsNullOrWhiteSpace(template))
            {
                throw new BadHttpRequestException("Query parameter 'template' is required for XML CG add requests.");
            }

            return new(template, playOnLoad, await ReadBodyAsync(request, cancellationToken));
        }

        throw new BadHttpRequestException($"Unsupported content type '{request.ContentType}'.");
    }

    public static async ValueTask<CgPayloadTranslationResult> TranslateUpdateAsync(
        HttpRequest request,
        CancellationToken cancellationToken)
    {
        if (IsJson(request.ContentType))
        {
            var payload = await JsonSerializer.DeserializeAsync<CgUpdateJsonRequest>(
                    request.Body,
                    JsonOptions,
                    cancellationToken: cancellationToken)
                ?? throw new BadHttpRequestException("Request body is required.");

            return new(null, false, ToTemplateXml(payload.TemplateData));
        }

        if (IsXml(request.ContentType))
        {
            return new(null, false, await ReadBodyAsync(request, cancellationToken));
        }

        throw new BadHttpRequestException($"Unsupported content type '{request.ContentType}'.");
    }

    private static bool IsJson(string? contentType) =>
        contentType?.StartsWith("application/json", StringComparison.OrdinalIgnoreCase) == true;

    private static bool IsXml(string? contentType) =>
        contentType?.StartsWith("application/xml", StringComparison.OrdinalIgnoreCase) == true
        || contentType?.StartsWith("text/xml", StringComparison.OrdinalIgnoreCase) == true;

    private static async Task<string> ReadBodyAsync(HttpRequest request, CancellationToken cancellationToken)
    {
        using var reader = new StreamReader(request.Body, leaveOpen: true);
        return await reader.ReadToEndAsync(cancellationToken);
    }

    private static string ToTemplateXml(TemplateDataEnvelope templateData)
    {
        var document = new XElement(
            "templateData",
            templateData.Components.Select(component =>
                new XElement(
                    "componentData",
                    new XAttribute("id", component.Id),
                    component.Data.Select(item =>
                        new XElement(
                            "data",
                            new XAttribute("id", item.Id),
                            new XAttribute("value", item.Value))))));

        return document.ToString(SaveOptions.DisableFormatting);
    }
}
