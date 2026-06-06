using System.Text.Json;
using Microsoft.AspNetCore.Http;
using StarDust.CasparCG.Events;

namespace StarDust.CasparCG.AspNetCore.Sse;

internal static class CasparSseWriter
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    public static async Task StreamAsync(
        HttpContext httpContext,
        ICasparClientResolver clientResolver,
        CancellationToken cancellationToken)
    {
        httpContext.Response.Headers.ContentType = "text/event-stream";
        httpContext.Response.Headers.CacheControl = "no-cache";

        var client = clientResolver.ResolveDefaultClient();

        await foreach (var evt in client.Events.ReadAllAsync(cancellationToken))
        {
            var eventName = ToEventName(evt);
            var payload = new CasparSseEvent(
                eventName,
                DateTimeOffset.UtcNow,
                evt.ClientName,
                evt);

            await httpContext.Response.WriteAsync($"event: {eventName}\n", cancellationToken);
            await httpContext.Response.WriteAsync($"data: {JsonSerializer.Serialize(payload, SerializerOptions)}\n\n", cancellationToken);
            await httpContext.Response.Body.FlushAsync(cancellationToken);
        }
    }

    private static string ToEventName(CasparEvent evt) =>
        evt switch
        {
            PlaybackClipChangedEvent => "playbackClipChanged",
            _ => evt.GetType().Name
        };
}
