using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using StarDust.CasparCG.AspNetCore.Contracts;
using StarDust.CasparCG.Query;

namespace StarDust.CasparCG.AspNetCore.Internal;

internal static class OpenApiHandlers
{
    private static readonly Regex RouteParameterRegex = new(@"\{(?<name>[^}:]+)(?::(?<constraint>[^}]+))?\}", RegexOptions.Compiled | RegexOptions.CultureInvariant);
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true
    };

    private static readonly IReadOnlyDictionary<string, RouteDocumentation> Routes = new Dictionary<string, RouteDocumentation>(StringComparer.OrdinalIgnoreCase)
    {
        ["GET /server/version"] = new("Read the server version", ResponseType: typeof(ServerVersionResponse)),
        ["GET /server/info"] = new("Read the INFO payload", ResponseType: typeof(QueryDataMap)),
        ["GET /server/info/config"] = new("Read the INFO CONFIG payload", ResponseType: typeof(QueryDataMap)),
        ["GET /server/info/paths"] = new("Read the INFO PATHS payload", ResponseType: typeof(QueryDataMap)),
        ["GET /server/gl/info"] = new("Read the GL INFO payload", ResponseType: typeof(QueryDataMap)),
        ["POST /server/gl/gc"] = new("Send GL GC"),
        ["POST /server/diag"] = new("Send DIAG"),
        ["GET /admin/log-level"] = new("Read the current log level", ResponseType: typeof(StringValueRequest)),
        ["PUT /admin/log-level"] = new("Update the current log level", RequestType: typeof(StringValueRequest), ResponseType: typeof(StringValueRequest)),
        ["POST /admin/bye"] = new("Send BYE"),
        ["POST /admin/kill"] = new("Send KILL"),
        ["GET /media/files"] = new("List media files", ResponseType: typeof(IReadOnlyList<MediaFileResponse>)),
        ["GET /media/files/{fileName}"] = new("Read detailed media info", ResponseType: typeof(MediaInfo)),
        ["GET /fonts"] = new("List fonts", ResponseType: typeof(IReadOnlyList<FontFile>)),
        ["GET /templates"] = new("List templates", ResponseType: typeof(IReadOnlyList<TemplateFile>)),
        ["GET /data"] = new("List data keys", ResponseType: typeof(IReadOnlyList<string>), QueryParameters: [new("subDirectory", typeof(string), false, "Optional data sub-directory filter")]),
        ["GET /data/{key}"] = new("Read a data payload", ResponseType: typeof(DataValueResponse)),
        ["PUT /data/{key}"] = new("Store a data payload", RequestType: typeof(DataStoreRequest)),
        ["DELETE /data/{key}"] = new("Remove a data payload"),
        ["POST /channels/clear-all"] = new("Send CLEAR ALL"),
        ["POST /channels/{channel}/clear"] = new("Clear the whole channel"),
        ["POST /channels/{channel}/grid"] = new("Send CHANNEL_GRID"),
        ["POST /channels/{channel}/layers/{layer}/load"] = new("Send LOAD", RequestType: typeof(LoadRequest)),
        ["POST /channels/{channel}/layers/{layer}/play"] = new("Send PLAY", RequestType: typeof(PlayRequest)),
        ["POST /channels/{channel}/layers/{layer}/loadbg"] = new("Send LOADBG", RequestType: typeof(LoadBackgroundRequest)),
        ["POST /channels/{channel}/layers/{layer}/pause"] = new("Pause the active layer"),
        ["POST /channels/{channel}/layers/{layer}/resume"] = new("Resume the active layer"),
        ["POST /channels/{channel}/layers/{layer}/stop"] = new("Stop the active layer"),
        ["POST /channels/{channel}/layers/{layer}/clear"] = new("Clear the active layer"),
        ["POST /channels/{channel}/layers/{layer}/call"] = new("Send CALL", RequestType: typeof(CallRequest)),
        ["POST /channels/{channel}/layers/{layer}/callbg"] = new("Send CALLBG", RequestType: typeof(CallRequest)),
        ["POST /channels/{channel}/layers/{layer}/swap"] = new("Swap two layers", RequestType: typeof(SwapRequest)),
        ["POST /channels/{channel}/add"] = new("Add a channel consumer", RequestType: typeof(AddRequest)),
        ["POST /channels/{channel}/remove"] = new("Remove a channel consumer", RequestType: typeof(RemoveRequest)),
        ["POST /channels/{channel}/apply"] = new("Apply a producer or consumer call", RequestType: typeof(ApplyRequest)),
        ["POST /channels/{channel}/print"] = new("Print the channel state"),
        ["POST /channels/{channel}/set"] = new("Set a channel property", RequestType: typeof(SetRequest)),
        ["POST /channels/{channel}/layers/{layer}/mixer/keyer"] = new("Set mixer keyer", RequestType: typeof(BooleanValueRequest)),
        ["POST /channels/{channel}/layers/{layer}/mixer/invert"] = new("Set mixer invert", RequestType: typeof(BooleanValueRequest)),
        ["POST /channels/{channel}/layers/{layer}/mixer/blend"] = new("Set mixer blend", RequestType: typeof(StringValueRequest)),
        ["POST /channels/{channel}/layers/{layer}/mixer/chroma"] = new("Set mixer chroma", RequestType: typeof(ArgumentsRequest)),
        ["POST /channels/{channel}/layers/{layer}/mixer/opacity"] = new("Set mixer opacity", RequestType: typeof(MixerOpacityRequest)),
        ["POST /channels/{channel}/layers/{layer}/mixer/brightness"] = new("Set mixer brightness", RequestType: typeof(NumericValueRequest)),
        ["POST /channels/{channel}/layers/{layer}/mixer/saturation"] = new("Set mixer saturation", RequestType: typeof(NumericValueRequest)),
        ["POST /channels/{channel}/layers/{layer}/mixer/contrast"] = new("Set mixer contrast", RequestType: typeof(NumericValueRequest)),
        ["POST /channels/{channel}/layers/{layer}/mixer/levels"] = new("Set mixer levels", RequestType: typeof(ArgumentsRequest)),
        ["POST /channels/{channel}/layers/{layer}/mixer/fill"] = new("Set mixer fill", RequestType: typeof(ArgumentsRequest)),
        ["POST /channels/{channel}/layers/{layer}/mixer/clip"] = new("Set mixer clip", RequestType: typeof(ArgumentsRequest)),
        ["POST /channels/{channel}/layers/{layer}/mixer/anchor"] = new("Set mixer anchor", RequestType: typeof(ArgumentsRequest)),
        ["POST /channels/{channel}/layers/{layer}/mixer/crop"] = new("Set mixer crop", RequestType: typeof(ArgumentsRequest)),
        ["POST /channels/{channel}/layers/{layer}/mixer/rotation"] = new("Set mixer rotation", RequestType: typeof(ArgumentsRequest)),
        ["POST /channels/{channel}/layers/{layer}/mixer/perspective"] = new("Set mixer perspective", RequestType: typeof(ArgumentsRequest)),
        ["POST /channels/{channel}/layers/{layer}/mixer/volume"] = new("Set mixer volume", RequestType: typeof(NumericValueRequest)),
        ["POST /channels/{channel}/layers/{layer}/mixer/grid"] = new("Set mixer grid", RequestType: typeof(ArgumentsRequest)),
        ["POST /channels/{channel}/layers/{layer}/mixer/commit"] = new("Commit mixer changes"),
        ["POST /channels/{channel}/layers/{layer}/mixer/clear"] = new("Clear mixer changes"),
        ["POST /channels/{channel}/layers/{layer}/cg/add"] = new("Add a CG template", RequestType: typeof(CgAddJsonRequest), RequestContentTypes: ["application/json", "application/xml"]),
        ["POST /channels/{channel}/layers/{layer}/cg/play"] = new("Play the active CG template"),
        ["POST /channels/{channel}/layers/{layer}/cg/stop"] = new("Stop the active CG template"),
        ["POST /channels/{channel}/layers/{layer}/cg/next"] = new("Advance the active CG template"),
        ["POST /channels/{channel}/layers/{layer}/cg/remove"] = new("Remove the active CG template"),
        ["POST /channels/{channel}/layers/{layer}/cg/clear"] = new("Clear CG state"),
        ["POST /channels/{channel}/layers/{layer}/cg/update"] = new("Update the active CG template", RequestType: typeof(CgUpdateJsonRequest), RequestContentTypes: ["application/json", "application/xml"]),
        ["POST /channels/{channel}/layers/{layer}/cg/invoke"] = new("Invoke a CG method", RequestType: typeof(CgInvokeRequest)),
        ["POST /admin/restart"] = new("Restart the server"),
        ["POST /admin/locks/{channel}/acquire"] = new("Acquire a channel lock", RequestType: typeof(LockAcquireRequest)),
        ["POST /admin/locks/{channel}/release"] = new("Release a channel lock"),
        ["POST /admin/locks/{channel}/clear"] = new("Clear a channel lock", RequestType: typeof(LockClearRequest)),
        ["GET /thumbnails"] = new("List thumbnails", ResponseType: typeof(IReadOnlyList<ThumbnailListItemResponse>)),
        ["GET /thumbnails/{fileName}"] = new("Retrieve a thumbnail", ResponseType: typeof(byte[]), ResponseContentTypes: ["application/octet-stream"]),
        ["POST /thumbnails/{fileName}/generate"] = new("Generate a thumbnail"),
        ["POST /thumbnails/generate-all"] = new("Generate all thumbnails"),
        ["GET /events"] = new("Stream server events", ResponseType: typeof(string), ResponseContentTypes: ["text/event-stream"])
    };

    public static IResult GetDocumentAsync(IEnumerable<EndpointDataSource> dataSources, string documentName)
    {
        ArgumentNullException.ThrowIfNull(dataSources);
        ArgumentNullException.ThrowIfNull(documentName);

        var paths = BuildPaths(dataSources);
        var document = new Dictionary<string, object?>
        {
            ["openapi"] = "3.0.1",
            ["info"] = new Dictionary<string, object?>
            {
                ["title"] = "StarDust.CasparCG REST API",
                ["version"] = documentName,
                ["description"] = "Generated from the mapped CasparCG REST endpoints."
            },
            ["paths"] = paths
        };

        return Results.Text(JsonSerializer.Serialize(document, SerializerOptions), "application/json");
    }

    private static Dictionary<string, object?> BuildPaths(IEnumerable<EndpointDataSource> dataSources)
    {
        var paths = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);

        foreach (var endpoint in dataSources.SelectMany(source => source.Endpoints).OfType<RouteEndpoint>())
        {
            if (!TryParseDisplayName(endpoint.DisplayName, out var method))
            {
                continue;
            }

            var rawPattern = endpoint.RoutePattern.RawText ?? string.Empty;
            var actualPath = NormalizeActualPath(rawPattern);
            var relativePath = TrimRoutePrefix(actualPath);

            if (IsOpenApiRoute(relativePath))
            {
                continue;
            }

            var documentationKey = GetRouteKey(method, relativePath);
            var documentation = Routes.TryGetValue(documentationKey, out var value)
                ? value
                : new RouteDocumentation($"Handle {method} {relativePath}");

            var operation = BuildOperation(
                method,
                actualPath,
                documentation,
                BuildPathParameters(endpoint.RoutePattern.RawText ?? string.Empty, actualPath));

            if (!paths.TryGetValue(actualPath, out var pathItem) || pathItem is not Dictionary<string, object?> operations)
            {
                operations = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
                paths[actualPath] = operations;
            }

            operations[method.ToLowerInvariant()] = operation;
        }

        return paths;
    }

    private static Dictionary<string, object?> BuildOperation(
        string method,
        string path,
        RouteDocumentation documentation,
        IReadOnlyList<OpenApiParameter> pathParameters)
    {
        var operation = new Dictionary<string, object?>
        {
            ["operationId"] = BuildOperationId(method, path),
            ["summary"] = documentation.Summary
        };

        var parameters = new List<object?>();
        parameters.AddRange(pathParameters.Select(parameter => BuildParameter(parameter.Name, "path", true, parameter.Type, parameter.Description)));
        parameters.AddRange((documentation.QueryParameters ?? Array.Empty<OpenApiParameter>())
            .Select(parameter => BuildParameter(parameter.Name, "query", parameter.Required, parameter.Type, parameter.Description)));

        if (parameters.Count > 0)
        {
            operation["parameters"] = parameters;
        }

        if (documentation.RequestType is not null)
        {
            operation["requestBody"] = new Dictionary<string, object?>
            {
                ["required"] = true,
                ["content"] = BuildContent(documentation.RequestType, documentation.RequestContentTypes)
            };
        }

        operation["responses"] = BuildResponses(method, documentation);

        return operation;
    }

    private static Dictionary<string, object?> BuildResponses(string method, RouteDocumentation documentation)
    {
        var description = method.Equals("DELETE", StringComparison.OrdinalIgnoreCase)
            ? "No content"
            : "Successful response";

        if (documentation.ResponseType is null)
        {
            return new Dictionary<string, object?>
            {
                ["200"] = new Dictionary<string, object?>
                {
                    ["description"] = description
                }
            };
        }

        return new Dictionary<string, object?>
        {
            ["200"] = new Dictionary<string, object?>
            {
                ["description"] = description,
                ["content"] = BuildContent(documentation.ResponseType, documentation.ResponseContentTypes)
            }
        };
    }

    private static Dictionary<string, object?> BuildContent(Type type, IReadOnlyList<string>? contentTypes)
    {
        var result = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
        var effectiveContentTypes = contentTypes is { Count: > 0 } ? contentTypes : ["application/json"];

        foreach (var contentType in effectiveContentTypes)
        {
            result[contentType] = new Dictionary<string, object?>
            {
                ["schema"] = BuildSchema(type)
            };
        }

        return result;
    }

    private static Dictionary<string, object?> BuildSchema(Type type)
    {
        var nonNullableType = Nullable.GetUnderlyingType(type) ?? type;

        if (nonNullableType == typeof(string))
        {
            return new Dictionary<string, object?> { ["type"] = "string" };
        }

        if (nonNullableType == typeof(bool))
        {
            return new Dictionary<string, object?> { ["type"] = "boolean" };
        }

        if (nonNullableType == typeof(int))
        {
            return new Dictionary<string, object?> { ["type"] = "integer", ["format"] = "int32" };
        }

        if (nonNullableType == typeof(long))
        {
            return new Dictionary<string, object?> { ["type"] = "integer", ["format"] = "int64" };
        }

        if (nonNullableType == typeof(double) || nonNullableType == typeof(float) || nonNullableType == typeof(decimal))
        {
            return new Dictionary<string, object?>
            {
                ["type"] = "number",
                ["format"] = nonNullableType == typeof(double) ? "double" : nonNullableType == typeof(float) ? "float" : "decimal"
            };
        }

        if (nonNullableType == typeof(DateTime) || nonNullableType == typeof(DateTimeOffset))
        {
            return new Dictionary<string, object?> { ["type"] = "string", ["format"] = "date-time" };
        }

        if (nonNullableType == typeof(byte[]))
        {
            return new Dictionary<string, object?> { ["type"] = "string", ["format"] = "binary" };
        }

        if (nonNullableType.IsEnum)
        {
            return new Dictionary<string, object?>
            {
                ["type"] = "string",
                ["enum"] = Enum.GetNames(nonNullableType)
            };
        }

        if (TryGetEnumerableElementType(nonNullableType, out var elementType))
        {
            return new Dictionary<string, object?>
            {
                ["type"] = "array",
                ["items"] = BuildSchema(elementType)
            };
        }

        if (nonNullableType.IsDictionaryType())
        {
            var valueType = nonNullableType.GetDictionaryValueType() ?? typeof(string);
            return new Dictionary<string, object?>
            {
                ["type"] = "object",
                ["additionalProperties"] = BuildSchema(valueType)
            };
        }

        return BuildObjectSchema(nonNullableType);
    }

    private static Dictionary<string, object?> BuildObjectSchema(Type type)
    {
        var properties = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
        var required = new List<string>();

        foreach (var property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            if (property.GetMethod is null || property.GetIndexParameters().Length > 0)
            {
                continue;
            }

            var name = JsonNamingPolicy.CamelCase.ConvertName(property.Name);
            properties[name] = BuildSchema(property.PropertyType);

            if (!IsNullable(property.PropertyType))
            {
                required.Add(name);
            }
        }

        var schema = new Dictionary<string, object?>
        {
            ["type"] = "object",
            ["properties"] = properties
        };

        if (required.Count > 0)
        {
            schema["required"] = required;
        }

        return schema;
    }

    private static object BuildParameter(string name, string location, bool required, Type type, string? description)
    {
        var parameter = new Dictionary<string, object?>
        {
            ["name"] = name,
            ["in"] = location,
            ["required"] = required,
            ["schema"] = BuildSchema(type)
        };

        if (!string.IsNullOrWhiteSpace(description))
        {
            parameter["description"] = description;
        }

        return parameter;
    }

    private static IReadOnlyList<OpenApiParameter> BuildPathParameters(string rawPattern, string actualPath)
    {
        var constraints = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (Match match in RouteParameterRegex.Matches(rawPattern))
        {
            constraints[match.Groups["name"].Value] = match.Groups["constraint"].Value;
        }

        var parameters = new List<OpenApiParameter>();
        foreach (Match match in RouteParameterRegex.Matches(actualPath))
        {
            var name = match.Groups["name"].Value;
            constraints.TryGetValue(name, out var constraint);
            parameters.Add(new OpenApiParameter(name, GetParameterType(name, constraint), true));
        }

        return parameters;
    }

    private static Type GetParameterType(string parameterName, string? constraint)
    {
        if (!string.IsNullOrWhiteSpace(constraint))
        {
            if (constraint.Contains("int", StringComparison.OrdinalIgnoreCase))
            {
                return typeof(int);
            }

            if (constraint.Contains("long", StringComparison.OrdinalIgnoreCase))
            {
                return typeof(long);
            }
        }

        if (parameterName.Equals("channel", StringComparison.OrdinalIgnoreCase) || parameterName.Equals("layer", StringComparison.OrdinalIgnoreCase))
        {
            return typeof(int);
        }

        return typeof(string);
    }

    private static string NormalizeActualPath(string rawPattern)
    {
        var path = RouteParameterRegex.Replace(rawPattern, match => $"{{{match.Groups["name"].Value}}}");
        return path.StartsWith('/') ? path : $"/{path}";
    }

    private static string TrimRoutePrefix(string actualPath)
    {
        var markerOrder = new[]
        {
            "/servers/",
            "/server/",
            "/admin/",
            "/channels/",
            "/data",
            "/media/",
            "/fonts",
            "/templates",
            "/thumbnails",
            "/events",
            "/swagger/"
        };

        foreach (var marker in markerOrder)
        {
            var index = actualPath.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
            if (index >= 0)
            {
                return actualPath[index..];
            }
        }

        return actualPath;
    }

    private static bool TryParseDisplayName(string? displayName, out string method)
    {
        method = string.Empty;

        if (string.IsNullOrWhiteSpace(displayName))
        {
            return false;
        }

        var separatorIndex = displayName.IndexOf(' ');
        if (separatorIndex <= 0 || separatorIndex >= displayName.Length - 1)
        {
            return false;
        }

        method = displayName[..separatorIndex];
        return true;
    }

    private static bool IsOpenApiRoute(string path) =>
        path.Equals("/swagger/{documentName}/swagger.json", StringComparison.OrdinalIgnoreCase)
        || path.EndsWith("/swagger/{documentName}/swagger.json", StringComparison.OrdinalIgnoreCase);

    private static string GetRouteKey(string method, string path)
    {
        var normalizedPath = path.StartsWith("/servers/{name}", StringComparison.OrdinalIgnoreCase)
            ? path["/servers/{name}".Length..]
            : path;

        return $"{method.ToUpperInvariant()} {normalizedPath}";
    }

    private static string BuildOperationId(string method, string path)
    {
        var builder = new System.Text.StringBuilder(method.Length + path.Length);
        builder.Append(char.ToUpperInvariant(method[0]));
        builder.Append(method[1..].ToLowerInvariant());

        foreach (var segment in path.Trim('/').Split('/', StringSplitOptions.RemoveEmptyEntries))
        {
            var token = segment.Trim('{', '}');
            foreach (var fragment in token.Split(['-', '_'], StringSplitOptions.RemoveEmptyEntries))
            {
                if (fragment.Length == 0)
                {
                    continue;
                }

                builder.Append(char.ToUpperInvariant(fragment[0]));
                if (fragment.Length > 1)
                {
                    builder.Append(fragment[1..]);
                }
            }
        }

        return builder.ToString();
    }

    private static bool TryGetEnumerableElementType(Type type, out Type elementType)
    {
        if (type.IsArray)
        {
            elementType = type.GetElementType()!;
            return true;
        }

        var enumerableInterface = type
            .GetInterfaces()
            .Concat([type])
            .FirstOrDefault(candidate =>
                candidate.IsGenericType && candidate.GetGenericTypeDefinition() == typeof(IEnumerable<>));

        if (enumerableInterface is not null)
        {
            elementType = enumerableInterface.GetGenericArguments()[0];
            return true;
        }

        elementType = typeof(object);
        return false;
    }

    private static bool IsNullable(Type type) =>
        !type.IsValueType || Nullable.GetUnderlyingType(type) is not null;

    private static bool IsDictionaryType(this Type type)
    {
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IReadOnlyDictionary<,>))
        {
            return true;
        }

        return type.GetInterfaces().Any(candidate =>
            candidate.IsGenericType && candidate.GetGenericTypeDefinition() == typeof(IReadOnlyDictionary<,>));
    }

    private static Type? GetDictionaryValueType(this Type type)
    {
        var dictionaryType = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IReadOnlyDictionary<,>)
            ? type
            : type.GetInterfaces().FirstOrDefault(candidate =>
                candidate.IsGenericType && candidate.GetGenericTypeDefinition() == typeof(IReadOnlyDictionary<,>));

        return dictionaryType?.GetGenericArguments()[1];
    }

    private sealed record RouteDocumentation(
        string Summary,
        Type? ResponseType = null,
        Type? RequestType = null,
        IReadOnlyList<string>? RequestContentTypes = null,
        IReadOnlyList<string>? ResponseContentTypes = null,
        IReadOnlyList<OpenApiParameter>? QueryParameters = null);

    private sealed record OpenApiParameter(string Name, Type Type, bool Required, string? Description = null);
}
