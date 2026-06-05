# CasparCG Typed Query Results Design

## Goal

Replace high-level string and raw-response query results with object-oriented return types for AMCP queries that return data.

This design keeps strict typed records only where the payload shape is stable enough to model confidently:

- `CLS`
- `CINF`
- `TLS`
- `FLS`

For other data-returning queries with looser or version-sensitive payloads, the high-level API returns an object wrapper around a dictionary-like representation instead of raw `AmcpResponse`:

- `INFO`
- `INFO CONFIG`
- `INFO PATHS`
- `GL INFO`

The goal is to make the normal `CasparClient` experience object-oriented by default without forcing brittle parsers onto heterogeneous AMCP payloads.

## Current State

The current client surface mixes three styles:

- `GetVersionAsync` returns a string
- `GetMediaFilesAsync` returns `IReadOnlyList<string>`
- several other query helpers return raw `AmcpResponse`

`AmcpResponseParser` is intentionally generic. It parses AMCP framing, status, command text, and payload lines, but it does not project those lines into domain objects.

The repository already contains a narrower catalog design for typed `CLS`, `TLS`, and `FLS` results. This design extends that direction to the full set of high-level query methods that return data while keeping the low-level AMCP response model available internally.

## Design Rules

- `CasparClient` should return objects for data queries instead of raw strings or raw `AmcpResponse`.
- Only stable query payloads get dedicated typed records.
- Unstable or mixed payloads return a generic object model built around a dictionary and preserved raw content.
- `AmcpResponseParser` remains a low-level framing parser and does not become a large query-specific switch.
- Query-specific mapping lives in dedicated projection/parsing helpers above `AmcpResponseParser`.
- Unknown fields must be preserved rather than discarded.
- Parsing failure for loose payloads must not throw by default when generic fallback data can still be returned.

## Public Contract

### Stable typed query models

The high-level client returns dedicated records for stable payloads:

```csharp
public sealed record MediaFile(
    string Name,
    MediaFileKind Kind,
    long SizeBytes,
    DateTimeOffset? LastModified,
    long? FrameCount,
    string? FrameRateOrDuration);

public enum MediaFileKind
{
    Unknown,
    Movie,
    Still,
    Audio
}

public sealed record MediaInfo(
    string Name,
    MediaFileKind Kind,
    long? SizeBytes,
    DateTimeOffset? LastModified,
    long? FrameCount,
    string? FrameRateOrDuration,
    IReadOnlyDictionary<string, string> Properties);

public sealed record TemplateFile(string Name);

public sealed record FontFile(string Name, string Path);
```

High-level methods become:

```csharp
ValueTask<IReadOnlyList<MediaFile>> GetMediaFilesAsync(CancellationToken cancellationToken);
ValueTask<MediaInfo> MediaInfoAsync(string fileName, CancellationToken cancellationToken);
ValueTask<IReadOnlyList<TemplateFile>> GetTemplateFilesAsync(string? subDirectory, CancellationToken cancellationToken);
ValueTask<IReadOnlyList<FontFile>> GetFontFilesAsync(CancellationToken cancellationToken);
```

`ServerScope` mirrors these result types.

### Generic object model for loose query payloads

Loose query payloads return a generic object wrapper:

```csharp
public sealed record QueryDataMap(
    IReadOnlyDictionary<string, string> Values,
    IReadOnlyList<string> Lines,
    string Raw);
```

High-level methods become:

```csharp
ValueTask<QueryDataMap> InfoAsync(CancellationToken cancellationToken);
ValueTask<QueryDataMap> InfoConfigAsync(CancellationToken cancellationToken);
ValueTask<QueryDataMap> InfoPathsAsync(CancellationToken cancellationToken);
ValueTask<QueryDataMap> GlInfoAsync(CancellationToken cancellationToken);
```

This object model gives consumers a dictionary-first result while preserving original lines and raw payload for diagnostics or edge-case inspection.

## Compatibility

This is an intentional breaking change for the vNext public query API.

The following behavior changes are expected:

- `GetMediaFilesAsync` no longer returns raw lines
- `MediaInfoAsync` no longer returns `AmcpResponse`
- `FileListAsync` is replaced by `GetFontFilesAsync`
- `TemplateListAsync` is replaced by `GetTemplateFilesAsync`
- `InfoAsync`, `InfoConfigAsync`, `InfoPathsAsync`, and `GlInfoAsync` no longer return `AmcpResponse`

Low-level raw AMCP access remains available inside the transport and protocol pipeline and can later be exposed through an explicit advanced API if needed. The normal high-level query surface should not require callers to interpret protocol text manually.

## Parsing Architecture

Keep parsing layered:

1. `AmcpResponseParser` keeps responsibility for AMCP framing and payload line extraction.
2. A dedicated query projection layer maps `AmcpResponse` into typed records or `QueryDataMap`.
3. `CasparClient` stays thin and delegates to those query projection helpers.

Recommended shape:

```csharp
internal static class CasparQueryResultParser
{
    public static IReadOnlyList<MediaFile> ParseMediaFiles(AmcpResponse response);
    public static MediaInfo ParseMediaInfo(AmcpResponse response);
    public static IReadOnlyList<TemplateFile> ParseTemplateFiles(AmcpResponse response);
    public static IReadOnlyList<FontFile> ParseFontFiles(AmcpResponse response);
    public static QueryDataMap ParseQueryDataMap(AmcpResponse response);
}
```

This keeps query interpretation cohesive without overloading the protocol layer.

## Stable Query Parsing

### `CLS`

Parse each line into `MediaFile`.

Requirements:

- support quoted file names, including spaces
- map kind tokens into `MediaFileKind`
- parse size when present
- parse timestamp when present
- parse frame count when present
- keep frame rate or duration as string in the first iteration
- map unknown media kinds to `MediaFileKind.Unknown`

### `CINF`

Parse the returned line set into `MediaInfo`.

Requirements:

- support the same core fields as `CLS`
- preserve extra key-value or trailing fields in `Properties`
- avoid guessing semantics for version-specific tail fields

### `TLS`

Parse each line into `TemplateFile`.

Requirements:

- use the template identifier as `Name`
- tolerate additional columns by ignoring or preserving them only if needed later

### `FLS`

Parse each line into `FontFile`.

Requirements:

- parse font name
- parse path or backing font file when present
- preserve compatibility with names containing spaces

## Generic Query Parsing

`INFO`, `INFO CONFIG`, `INFO PATHS`, and `GL INFO` should be projected into `QueryDataMap`.

The parser should attempt to extract structured values in this order:

1. XML payload:
   - flatten simple elements into dictionary keys
   - use stable joined paths such as `configuration.paths.media-path`
   - when repeated elements make flattening ambiguous, keep raw lines and only extract what is safe
2. key-value text payload:
   - parse common `key=value`, `key: value`, or `key value` patterns conservatively
3. fallback:
   - preserve indexed lines in `Lines`
   - populate only the dictionary entries that can be derived confidently

The fallback rule is:

- never throw solely because a loose payload is only partially recognized
- return the best-effort dictionary plus original lines and raw content

## Error Handling

Stable typed query parsers should fail fast when a line that must match the contract is malformed. Prefer a dedicated parsing exception such as:

```csharp
public sealed class CasparQueryParseException : Exception
{
}
```

Generic dictionary projections should not fail fast for partial recognition. They should return:

- extracted dictionary entries
- original response lines
- original raw payload

This gives the caller a usable object even when the payload is richer than the current parser understands.

## Client Flow

For each high-level query:

1. `CasparClient` sends the AMCP command through the existing query path.
2. The raw AMCP text is parsed into `AmcpResponse`.
3. The query projection layer maps the response into a typed result or `QueryDataMap`.
4. `CasparClient` returns the object result.

`ServerScope` should remain a thin forwarding layer and expose the same return types as `CasparClient`.

## Testing

Use red-green tests with Arrange, Act, Assert.

Required tests:

- `CLS` parses a complete media line into `MediaFile`
- `CLS` supports quoted names containing spaces
- `CLS` maps unknown media kinds to `MediaFileKind.Unknown`
- `CINF` parses a stable media info payload into `MediaInfo`
- `CINF` preserves extra fields in `Properties`
- `TLS` returns `TemplateFile` records
- `FLS` returns `FontFile` records
- `INFO CONFIG` XML payload is projected into `QueryDataMap`
- `INFO PATHS` XML payload is projected into `QueryDataMap`
- `GL INFO` text payload is projected into `QueryDataMap`
- partial or unfamiliar `INFO*` payloads still return `QueryDataMap` without throwing
- `CasparClient.GetMediaFilesAsync` sends `CLS` and returns typed records
- `CasparClient.MediaInfoAsync` sends `CINF <file>` and returns `MediaInfo`
- `CasparClient.GetTemplateFilesAsync` sends `TLS` and returns typed records
- `CasparClient.GetFontFilesAsync` sends `FLS` and returns typed records
- `CasparClient.InfoAsync` sends `INFO` and returns `QueryDataMap`
- `CasparClient.InfoConfigAsync` sends `INFO CONFIG` and returns `QueryDataMap`
- `CasparClient.InfoPathsAsync` sends `INFO PATHS` and returns `QueryDataMap`
- `CasparClient.GlInfoAsync` sends `GL INFO` and returns `QueryDataMap`
- `ServerScope` forwards the query methods and preserves the typed return contracts

## Out Of Scope

- changing the low-level `AmcpResponseParser` contract
- typing every possible AMCP payload variant into dedicated records
- creating a public advanced raw-query API in this iteration
- implementing HTTP-based catalog retrieval

## Acceptance Criteria

- high-level query methods return objects instead of raw strings or raw `AmcpResponse`
- `CLS`, `CINF`, `TLS`, and `FLS` return dedicated typed records
- `INFO`, `INFO CONFIG`, `INFO PATHS`, and `GL INFO` return `QueryDataMap`
- unknown or partially understood loose payloads do not fail the high-level query API
- query-specific parsing is separated from the generic AMCP response parser
- unit tests cover stable parsing, generic fallback parsing, and client/scoped forwarding behavior
