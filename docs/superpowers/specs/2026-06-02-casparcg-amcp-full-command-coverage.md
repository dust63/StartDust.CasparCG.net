# CasparCG AMCP Full Command Coverage

**Goal:** expose the full AMCP command surface of CasparCG through the vNext client, with a coherent command model, typed request helpers where responses are stable, and raw response access for the rest.

**Architecture:** the AMCP protocol layer owns one command record per AMCP verb or verb group, grouped by family (`Basic`, `Query`, `Data`, `Template`, `Mixer`, `Thumbnail`, `OSC`). `CasparClient` stays the public entry point and provides thin convenience methods that forward to protocol commands through the existing transport and response pipeline. Stable read commands get typed helpers when it is useful; everything else stays available as raw `AmcpResponse` so the surface remains exhaustive without forcing brittle parsers.

**Tech Stack:** .NET 10, C#, `ReadOnlySpan<char>`, `IReadOnlyList<string>`, existing AMCP transport/parser stack, xUnit.

---

## Scope

This work completes the AMCP command surface currently missing from the vNext client. The implementation must cover the command families exposed by the server source:

- Basic channel control commands
- Query commands
- Data commands
- Template / CG commands
- Mixer commands
- Thumbnail commands
- OSC subscribe / unsubscribe commands

The work must preserve the existing behavior of:

- AMCP transport connection and response parsing
- error handling through `AmcpCommandException`
- OSC event ingestion and deduplication
- the demo probe workflow

## Design Rules

- Every AMCP command must have a single protocol object in `StarDust.CasparCG.Protocol.Amcp.Commands`.
- Command serialization must stay exact and testable.
- `CasparClient` methods should be grouped by intent, not by internal implementation detail.
- When a response shape is stable and common, add a helper method that returns a typed result.
- When a response shape is inconsistent, expose the command as a raw `AmcpResponse` or `IReadOnlyList<string>` and do not overfit a parser.
- Keep the public API backward-compatible with existing commands already used by the probe and tests.

## Command Coverage Plan

### Basic

Cover the common channel operations:

- `LOAD`
- `LOADBG`
- `PLAY`
- `PAUSE`
- `RESUME`
- `STOP`
- `CLEAR`
- `CALL`
- `SWAP`
- `ADD`
- `REMOVE`
- `APPLY`
- `PRINT`
- `CLEAR ALL`
- `SET`

### Query

Cover the introspection commands:

- `VERSION`
- `INFO`
- `INFO CONFIG`
- `INFO PATHS`
- `CINF`
- `CLS`
- `FLS`
- `TLS`
- `GL INFO`
- `GL GC`

### Data

Cover the data store commands:

- `DATA STORE`
- `DATA RETRIEVE`
- `DATA LIST`
- `DATA REMOVE`

### Template / CG

Cover the template and CG flow:

- `CG ADD`
- `CG PLAY`
- `CG STOP`
- `CG NEXT`
- `CG REMOVE`
- `CG CLEAR`
- `CG UPDATE`
- `CG INVOKE`

### Mixer

Cover the full mixer command family:

- `MIXER KEYER`
- `MIXER INVERT`
- `MIXER CHROMA`
- `MIXER BLEND`
- `MIXER OPACITY`
- `MIXER BRIGHTNESS`
- `MIXER SATURATION`
- `MIXER CONTRAST`
- `MIXER LEVELS`
- `MIXER FILL`
- `MIXER CLIP`
- `MIXER ANCHOR`
- `MIXER CROP`
- `MIXER ROTATION`
- `MIXER PERSPECTIVE`
- `MIXER VOLUME`
- `MIXER MASTERVOLUME`
- `MIXER GRID`
- `MIXER COMMIT`
- `MIXER CLEAR`
- `CHANNEL_GRID`

### Thumbnail

Cover the thumbnail commands:

- `THUMBNAIL LIST`
- `THUMBNAIL RETRIEVE`
- `THUMBNAIL GENERATE`
- `THUMBNAIL GENERATE_ALL`

### OSC

Cover the OSC subscription lifecycle:

- `OSC SUBSCRIBE`
- `OSC UNSUBSCRIBE`

## Public API Shape

The client API should remain simple:

- channel-oriented helpers for common play/control operations
- explicit methods for query commands that have stable return shapes
- command methods returning `ValueTask` when the command only needs success/failure
- raw access to `AmcpResponse` for commands where the response content is not stable enough for a stronger type

The client must not become a large switch statement. The command objects stay in the protocol layer and the client remains a thin orchestration layer.

## Testing

Coverage must include:

- command serialization tests for each new AMCP command family
- response parsing tests for the stable query helpers
- client tests proving the correct command text is emitted
- a few integration tests using the recording transport to cover success and AMCP error paths

The implementation is done only when:

- all command families above are represented in code
- the demo probe still works
- the test suite passes
- there is no regression in OSC subscribe/unsubscribe or event deduplication
