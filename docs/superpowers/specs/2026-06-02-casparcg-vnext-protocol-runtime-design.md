# CasparCG vNext Protocol Runtime Design

- Date: 2026-06-02
- Project: `StartDust.CasparCG.net`
- Status: Proposed and validated in discussion
- Supersedes: the protocol, command-surface, and legacy-retirement assumptions in `docs/superpowers/specs/2026-06-02-casparcg-vnext-design.md`

## 1. Intent

This design defines the protocol-focused rewrite needed to make the vNext library production-usable against a real CasparCG server.

The original vNext phase-one work established a minimal client, some AMCP command serialization, a basic event stream, and a dummy integration test. That is not enough for real use. The next iteration must provide:

- full AMCP request/response handling with typed return codes
- real success and failure semantics for high-level commands
- broad command-family coverage
- real OSC UDP intake and mapping
- removal of the legacy projects as runtime dependencies and eventually from the repository

This remains a breaking redesign. Backward compatibility with the legacy API is not a goal.

## 2. Product Goals

The protocol runtime redesign must optimize for the following:

- target `.NET 10`
- operate correctly against a real local CasparCG server
- cover the main AMCP command families:
  - playout
  - query
  - CG
  - mixer
  - data
- expose AMCP responses as first-class typed results
- let high-level APIs throw rich exceptions for `4xx` and `5xx`
- ingest OSC over UDP using CasparCG's documented default behavior
- map OSC into a consistent domain event stream and state projection
- remove legacy runtime projects from the active solution and migration path once vNext covers the required functionality

## 3. External Protocol Constraints

### 3.1 AMCP

The runtime must follow the official AMCP protocol behavior:

- all command text is UTF-8
- every command is terminated by `\r\n`
- command parsing is case-insensitive
- quoted values may contain spaces
- return codes are meaningful and must be preserved:
  - `100`, `101` informational
  - `200`, `201`, `202` success
  - `400` to `404` client errors
  - `500` to `503` server errors

### 3.2 OSC

The runtime must follow the official OSC behavior:

- OSC is UDP, not TCP
- CasparCG sends OSC one-way from server to client
- the OSC client is associated with an AMCP connection unless configured as a predefined persistent client
- default OSC port is `6250`
- the current local server configuration also includes a predefined client at `127.0.0.1:5253`

## 4. Public API Shape

### 4.1 Two-level AMCP API

The public API must expose both low-level and high-level AMCP flows.

Low-level:

```csharp
AmcpResponse response = await client.SendAsync(
    AmcpRequest.Play(channel: 1, layer: 10, clip: "AMB"),
    ct);
```

High-level:

```csharp
await client.PlayAsync(1, 10, "AMB", ct);
await client.PauseAsync(1, 10, ct);
var version = await client.VersionAsync(ct);
```

Rules:

- low-level `SendAsync` never hides the response
- high-level APIs throw on `4xx` and `5xx`
- high-level query APIs may still return typed payload models derived from the response

### 4.2 Response model

The vNext runtime must replace the legacy `AMCPEventArgs` model with a typed response object:

```csharp
public sealed class AmcpResponse
{
    public int StatusCode { get; }
    public AmcpStatusCategory Category { get; }
    public string CommandText { get; }
    public string StatusLine { get; }
    public IReadOnlyList<string> Lines { get; }
    public string Raw { get; }
    public bool IsSuccess { get; }
}
```

This model must preserve:

- the exact return code
- the exact status line
- the exact payload lines
- enough information to rebuild diagnostics and meaningful exceptions

### 4.3 Exception model

High-level command methods must throw a dedicated exception type on AMCP failure:

```csharp
public sealed class AmcpCommandException : InvalidOperationException
{
    public AmcpResponse Response { get; }
}
```

This exception must expose the full `AmcpResponse`, not only a simplified error enum.

### 4.4 Command-family entry points

The public surface must cover:

- direct methods on `CasparClient` for common playout and query commands
- scoped/fluent methods for layered media commands
- dedicated sub-surfaces for grouped command families when needed

Examples:

```csharp
await client.PlayAsync(1, 10, "AMB", ct);
await client.ClearAsync(1, 10, ct);
await client.Cg.AddAsync(1, 10, 1, "LOWER_THIRD", data, ct);
await client.Mixer.VolumeAsync(1, 10, 0.5m, ct);
await client.Data.StoreAsync("my-key", payload, ct);
var media = await client.Query.TlsAsync(ct);
```

The API does not need to expose every possible command as a fluent builder. It does need complete coverage of the agreed command families.

## 5. Internal Architecture

### 5.1 Project responsibilities

- `src/StarDust.CasparCG`
  Public API, client orchestration, typed results, exceptions, events, state, diagnostics.
- `src/StarDust.CasparCG.Protocol.Amcp`
  Command definitions, serialization, response parsing, return-code categorization, typed payload parsing.
- `src/StarDust.CasparCG.Protocol.Osc`
  UDP packet decoding, OSC message representation, address-pattern mapping to domain events.
- `src/StarDust.CasparCG.Transport`
  TCP and UDP transport implementations, lifecycle, serialized AMCP send/receive loop.
- `src/StarDust.CasparCG.Testing`
  Dummy AMCP server, OSC emitters, protocol test helpers.

### 5.2 AMCP transport contract

The AMCP transport must become response-aware. It must no longer only accept raw command text and return a string opportunistically.

Target contract:

```csharp
public interface IAmcpTransport
{
    ValueTask ConnectAsync(CancellationToken cancellationToken);
    ValueTask DisconnectAsync(CancellationToken cancellationToken);
    ValueTask<AmcpResponse> SendAsync(AmcpCommand command, CancellationToken cancellationToken);
}
```

Key rule:

- requests must be serialized unless and until the protocol layer explicitly supports safe pipelining

This is required because the legacy event-based "wait for next response" approach is concurrency-fragile.

### 5.3 AMCP parser behavior

The AMCP parser must be a deterministic line-oriented state machine.

Required cases:

- `202`: header only
- `201`: header plus one payload line
- `200`: header plus multi-line payload terminated by an empty line
- `400`: error line, optionally with error payload line(s) if returned by server
- `101`: informational single-line events
- unknown but syntactically valid codes must still be preserved in the response object

The parser must keep the raw block for diagnostics.

### 5.4 OSC runtime behavior

The OSC transport must listen on a configurable UDP endpoint, defaulting to `6250`.

The OSC parser/mapping layer must:

- parse OSC datagrams into address plus typed arguments
- normalize channel/layer extraction from address patterns
- map known addresses into domain events
- expose unmatched packets as raw diagnostics instead of dropping them silently

Initial mapped addresses must include:

- `/channel/{n}/format`
- `/channel/{n}/stage/layer/{m}/time`
- `/channel/{n}/stage/layer/{m}/frame`
- `/channel/{n}/stage/layer/{m}/paused`
- `/channel/{n}/stage/layer/{m}/background/type`
- `/channel/{n}/output/port/{p}/type`
- `/channel/{n}/mixer/audio/nb_channels`

## 6. State and Event Model

### 6.1 Event unification

AMCP and OSC both feed the same domain event stream.

Examples:

- `PlaybackClipChangedEvent`
- `LayerPausedChangedEvent`
- `LayerTimeChangedEvent`
- `ChannelFormatChangedEvent`
- `OutputConsumerChangedEvent`

### 6.2 Raw access

The runtime must expose:

- raw AMCP responses for diagnostics
- raw OSC packets/messages for diagnostics

These are advanced surfaces and must not replace the domain event model.

### 6.3 State projection

The state store must keep only the latest known values needed for practical reads:

- channel format
- current clip per layer when known
- layer paused state
- layer time/frame when known
- output consumer type when known

This is a pragmatic projection, not a full mirror of server internals.

## 7. Command Coverage

The first complete vNext protocol release must cover at least these families.

### 7.1 Playout

- `LOADBG`
- `LOAD`
- `PLAY`
- `PAUSE`
- `RESUME`
- `STOP`
- `CLEAR`
- `CALL`
- `SWAP`

### 7.2 Query

- `VERSION`
- `INFO`
- `INFO TEMPLATE`
- `INFO CONFIG`
- `INFO PATHS`
- `INFO SYSTEM`
- `INFO SERVER`
- `INFO QUEUES`
- `INFO THREADS`
- `INFO DELAY`
- `CLS`
- `FLS`
- `TLS`
- `CINF`
- `DIAG`
- `HELP`

### 7.3 CG

- `CG ADD`
- `CG PLAY`
- `CG STOP`
- `CG NEXT`
- `CG REMOVE`
- `CG CLEAR`
- `CG UPDATE`
- `CG INVOKE`
- `CG INFO`

### 7.4 Mixer

At minimum, the common mixer commands plus query-safe response handling:

- `MIXER KEYER`
- `MIXER CHROMA`
- `MIXER BLEND`
- `MIXER OPACITY`
- `MIXER BRIGHTNESS`
- `MIXER SATURATION`
- `MIXER CONTRAST`
- `MIXER FILL`
- `MIXER CLIP`
- `MIXER ANCHOR`
- `MIXER CROP`
- `MIXER ROTATION`
- `MIXER PERSPECTIVE`
- `MIXER MIPMAP`
- `MIXER VOLUME`
- `MIXER MASTERVOLUME`
- `MIXER STRAIGHT_ALPHA_OUTPUT`
- `MIXER GRID`
- `MIXER COMMIT`
- `MIXER CLEAR`

### 7.5 Data

- `DATA STORE`
- `DATA RETRIEVE`
- `DATA LIST`
- `DATA REMOVE`

## 8. Legacy Retirement

### 8.1 Immediate rule

Legacy projects must stop being the implementation source of truth.

Specifically:

- no new runtime behavior may be added to legacy AMCP or OSC projects
- legacy code may be read for extraction of protocol knowledge only
- new public behavior must live in `StarDust.CasparCG.*`

### 8.2 Solution cleanup

Once vNext command and protocol coverage is sufficient:

- remove old legacy projects from `src/StarDust.CasparCG.net.sln`
- keep them in git only temporarily if needed for migration/reference
- delete them from the repository after the vNext tests and docs fully replace their role

### 8.3 Migration safety

Before deleting legacy source, ensure:

- every required command family above exists in vNext
- AMCP return-code handling is verified
- OSC ingest is verified
- unit and integration coverage exists for the vNext path
- docs no longer direct users to the legacy API

## 9. Testing Strategy

### 9.1 AMCP parser tests

The parser must have focused tests for:

- `200`, `201`, `202`
- `400`, `401`, `402`, `403`, `404`
- `500`, `501`, `502`, `503`
- single response blocks
- multi-response payloads
- unknown command text with valid code
- quoted and escaped values where relevant

### 9.2 Command serialization tests

Each command family must have serialization tests for representative commands and optional parameters.

### 9.3 Local server integration

The real local server must be used for integration coverage:

- AMCP connect and common playout commands
- representative query commands
- at least one CG command
- at least one mixer command
- at least one data command

### 9.4 OSC integration

OSC must be validated against the default local configuration:

- UDP listener on `6250`
- compatibility with predefined client behavior to `127.0.0.1:5253`
- real packet ingestion and event projection

### 9.5 Packaging verification

Packaging is a required gate, not optional cleanup.

The build and package flow must succeed for the vNext projects without depending on legacy solution restore behavior.

## 10. Success Criteria

This redesign is complete when:

- `CasparClient` can send all agreed AMCP command families
- high-level methods throw `AmcpCommandException` on non-`2xx`
- low-level AMCP calls return typed `AmcpResponse`
- OSC UDP packets are ingested and mapped into domain events
- the active solution no longer depends on legacy runtime projects
- release tests pass against the local server
- packaging succeeds for the vNext package set

## 11. Non-Goals

This redesign does not require:

- preserving the old event-per-command public surface
- perfect coverage of every obscure AMCP command before deleting the legacy API
- a full object model for every possible `INFO` payload on day one
- web or desktop UI components in the core runtime
