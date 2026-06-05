# CasparCG Typed Catalog Design

## Goal

Replace string-based catalog results with typed records for CasparCG data retrieval APIs, starting with media, template, and font listings. The catalog retrieval method must be configurable so AMCP remains the default source while future HTTP media-server retrieval can be added without changing the public consumer-facing API.

This is an intentional breaking change for high-level catalog methods. The library should expose objects for object data, not raw protocol lines.

## Current State

`CasparClient.GetMediaFilesAsync` sends `CLS` through AMCP and returns `IReadOnlyList<string>` from `AmcpResponse.Lines`.

The protocol command surface already contains `ClsCommand`, `TlsCommand`, and `FlsCommand`, but only media has a high-level helper. `AmcpResponseParser` is intentionally generic and keeps payload lines as strings.

An obsolete REST API project was previously planned for cleanup. The active library does not currently have a supported REST catalog provider. CasparCG server exposes a configurable media-server HTTP endpoint, so this design keeps the retrieval source configurable and prepares an extension point instead of baking AMCP into the public catalog API.

## Public Contract

High-level catalog methods return records:

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

public sealed record TemplateFile(string Name);

public sealed record FontFile(string Name, string Path);
```

`CasparClient.GetMediaFilesAsync` returns `ValueTask<IReadOnlyList<MediaFile>>`.

Add high-level helpers:

```csharp
ValueTask<IReadOnlyList<TemplateFile>> GetTemplateFilesAsync(CancellationToken cancellationToken);
ValueTask<IReadOnlyList<FontFile>> GetFontFilesAsync(CancellationToken cancellationToken);
```

Fields that vary by CasparCG version should be nullable rather than guessed. Raw `AmcpResponse` remains available through lower-level AMCP query paths; high-level catalog methods should not return raw strings.

## Catalog Provider

Introduce a focused catalog abstraction:

```csharp
public interface ICasparCatalogProvider
{
    ValueTask<IReadOnlyList<MediaFile>> GetMediaFilesAsync(CancellationToken cancellationToken);
    ValueTask<IReadOnlyList<TemplateFile>> GetTemplateFilesAsync(CancellationToken cancellationToken);
    ValueTask<IReadOnlyList<FontFile>> GetFontFilesAsync(CancellationToken cancellationToken);
}
```

The default provider is AMCP-based. It sends `CLS`, `TLS`, and `FLS`, then maps response lines into records.

Consumers can configure the source:

```csharp
options.UseAmcpCatalog();
options.UseCatalogProvider(customProvider);
```

Reserve the shape for future HTTP media-server support:

```csharp
options.UseHttpMediaServerCatalog(new Uri("http://localhost:8000"));
```

The first implementation does not need to implement HTTP retrieval. It must keep the extension point clear so HTTP can be added later without changing `CasparClient` method signatures.

## Parse Error Handling

Parsing errors are controlled by `CatalogParseErrorHandling`:

```csharp
public enum CatalogParseErrorHandling
{
    Throw,
    SkipInvalidLines
}
```

Default: `Throw`.

When set to `Throw`, an invalid catalog line raises an explicit parsing error, preferably a dedicated `CasparCatalogParseException` containing the invalid line.

When set to `SkipInvalidLines`, invalid lines are ignored and valid records are returned. The implementation should still leave diagnostics available, such as a skipped-line counter, a diagnostic event, or integration with existing client diagnostics. The public catalog methods should continue to return only valid records.

## Parsing

Keep parsing separate from `CasparClient`, for example in `CasparCatalogParsers`.

Expected parser responsibilities:

- Parse `CLS` lines with quoted names, including names with spaces.
- Map known media kinds to `MediaFileKind`; unknown values become `MediaFileKind.Unknown`.
- Parse numeric size and frame count when present.
- Parse AMCP timestamp fields into `DateTimeOffset?` when possible.
- Preserve frame rate or duration as a string in the first implementation because CasparCG formats can vary.
- Parse `TLS` into `TemplateFile` using the template name.
- Parse `FLS` into `FontFile` using name and path.

The parser should be deterministic and covered by unit tests. It should not depend on network transport.

## Client Flow

`CasparClient` delegates catalog requests to `ICasparCatalogProvider`.

For the default AMCP provider:

1. `CasparClient.GetMediaFilesAsync` calls the catalog provider.
2. The provider sends the appropriate AMCP command through the existing command/query path.
3. The provider passes `AmcpResponse.Lines` to the parser.
4. The parser returns records or handles invalid lines according to `CatalogParseErrorHandling`.

This keeps protocol parsing and client orchestration separate and prevents `CasparClient` from accumulating catalog-specific parsing logic.

## Configuration

The options model should support:

- AMCP catalog source as the default.
- Custom catalog provider injection.
- `CatalogParseErrorHandling.Throw` as the default parse behavior.
- `CatalogParseErrorHandling.SkipInvalidLines` for tolerant environments.

If existing hosting or DI options already define client registration patterns, follow those patterns instead of creating a parallel configuration style.

## Testing

Use red-green tests with Arrange, Act, Assert.

Required tests:

- `CLS` parses a complete media line into `MediaFile`.
- `CLS` supports quoted names containing spaces.
- `CLS` maps unknown media kinds to `MediaFileKind.Unknown`.
- `CatalogParseErrorHandling.Throw` raises an explicit error for invalid lines.
- `CatalogParseErrorHandling.SkipInvalidLines` ignores invalid lines and returns valid records.
- `TLS` returns `TemplateFile` records.
- `FLS` returns `FontFile` records.
- `CasparClient.GetMediaFilesAsync` sends `CLS` and returns typed records.
- `CasparClient.GetTemplateFilesAsync` sends `TLS` and returns typed records.
- `CasparClient.GetFontFilesAsync` sends `FLS` and returns typed records.
- Configuration can replace the default catalog provider with a custom provider.

## Out Of Scope

- Implementing the HTTP media-server provider.
- Adding broad REST API support.
- Changing the generic `AmcpResponseParser` contract.
- Typing every AMCP query response in this iteration.

## Acceptance Criteria

- High-level catalog methods return records, not strings.
- AMCP is the default catalog source.
- Catalog source can be replaced through configuration.
- Parse error handling is configurable with `CatalogParseErrorHandling.Throw` as the default.
- Unit tests cover parser behavior, client delegation, and configuration replacement.
- Existing raw AMCP response access remains available for lower-level workflows.
