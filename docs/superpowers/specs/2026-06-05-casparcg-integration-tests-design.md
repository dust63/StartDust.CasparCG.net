# CasparCG Integration Tests Expansion

## Goal

Expand the integration test coverage of `StarDust.CasparCG.IntegrationTests` so the AMCP/TCP surface is validated against the in-repo `DummyCasparServer` across the main command families, not just a single `PLAY` scenario.

The intent is to verify that the public client API really emits the expected wire commands, handles AMCP success/error responses, and correctly parses representative structured payloads.

## Scope

In scope:

- AMCP over TCP through `DummyCasparServer`
- public client methods and their fluent scope equivalents
- representative query responses with structured payloads
- representative error responses
- command ordering within a single scripted scenario

Out of scope:

- full end-to-end validation against a real CasparCG binary
- OSC coverage beyond the existing transport test
- new protocol features or client APIs
- exhaustive coverage of every AMCP verb

## Proposed Coverage

The integration suite will be expanded around three test groups:

1. Command-family coverage
   - one test per major family: basic playback, data, template, mixer, query, admin
   - each test uses `DummyCasparServer` to script replies and capture sent commands
   - assertions focus on the exact AMCP text sent by the client and the returned parsed result when relevant

2. Multi-step scenario coverage
   - one or two tests that send multiple commands over the same TCP session
   - validates ordering and that the client can mix `SendAsync` and `QueryAsync` flows against the same server instance

3. Response and error contracts
   - representative structured responses for `INFO`, `INFO CONFIG`, `INFO PATHS`, `CINF`, `CLS`, `FLS`, `TLS`, and `GL INFO`
   - at least one negative response path, so command exceptions and error propagation are exercised in integration, not only unit tests

## Design

### Test Harness

The existing `DummyCasparServer` remains the core harness. It is already sufficient for scripted TCP replies and command capture, so the design keeps the harness lightweight rather than introducing a heavier mock framework.

The only harness expectation is that a scenario can script responses by exact command text and record every received command in order. No new server behavior is required unless a test needs a specific reply shape that the current scenario API cannot express.

### Test Shape

Integration tests will follow an arrange-act-assert style:

- arrange a `DummyScenario` with exact command-to-reply mappings
- start the server and connect a real `CasparClient` using `TcpAmcpTransport`
- act by calling the public API, preferably through the fluent scopes where the API supports them
- assert on both the sent AMCP wire text and the parsed result object

### Data Flow

The client remains the system under test:

`CasparClient` -> `TcpAmcpTransport` -> `DummyCasparServer`

For query-style commands, the response path is also exercised end to end:

`DummyCasparServer` -> raw AMCP reply -> `AmcpResponseParser` -> `CasparQueryResultParser` -> typed result

This is the main reason the integration suite should grow beyond a single `PLAY` case: it validates both command emission and parsing behavior in one loop.

## Test Matrix

The first pass of the expanded suite should include:

- `PLAY`, `LOADBG`, `PAUSE`, `RESUME`, `STOP`
- `DATA STORE`, `DATA RETRIEVE`, `DATA LIST`, `DATA REMOVE`
- `CG ADD`, `CG UPDATE`, `CG PLAY`, `CG STOP`
- a representative mixer command such as `MIXER OPACITY` or `MIXER COMMIT`
- `INFO`, `INFO CONFIG`, `INFO PATHS`, `GL INFO`
- `CINF`, `CLS`, `FLS`, `TLS`
- admin commands `DIAG`, `BYE`, `KILL`, `RESTART`, and the new `LOG LEVEL` / `LOCK` variants

The suite does not need to cover every mixer or playback verb in integration if unit tests already validate their serialization. The goal is breadth across families, not a duplicate of the unit suite.

## Error Handling

At least one integration test should script a non-success AMCP reply and assert that the client raises `AmcpCommandException` with the response metadata preserved.

If a query payload is malformed, the integration suite should prefer a single representative parse failure test rather than duplicating every parser failure path already covered by unit tests.

## Acceptance Criteria

The work is complete when:

- the integration suite covers every major AMCP family at least once
- at least one integration scenario validates a structured query response
- at least one integration scenario validates an AMCP error response
- the suite still runs quickly and deterministically against the dummy server
- the existing OSC integration test remains unchanged

## Notes

- Keep the tests small and readable; prefer several focused scenarios over one oversized integration mega-test.
- Reuse the same `DummyCasparServer` harness pattern everywhere to avoid test-specific infrastructure.
- If a requested scenario cannot be expressed cleanly with the current harness, extend the harness minimally instead of adding ad hoc mocks in the test project.
