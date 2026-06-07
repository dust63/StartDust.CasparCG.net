# OSC Event Coverage REST SSE Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Expand OSC parsing and event mapping so raw and typed CasparCG OSC monitor events flow through `client.Events`, REST SSE, state projection, and docs.

**Architecture:** Keep the existing UDP OSC and SSE pipeline, but change OSC mapping from single-event to multi-event. The parser learns CasparCG numeric tags, the mapper emits a raw fallback plus selected typed events, the state store projects typed events, and SSE names each event type with a stable string.

**Tech Stack:** .NET 10, C# preview, xUnit v2, ASP.NET Core minimal endpoints, Server-Sent Events.

---

### Task 1: Parser Coverage

**Files:**
- Modify: `src/StarDust.CasparCG/Protocol/Osc/OscPacketParser.cs`
- Test: `test/StarDust.CasparCG.UnitTests/OscMessageParserTests.cs`

- [x] **Step 1: Write failing parser tests**

Add tests that build OSC packets with type tags `h` and `d`, then assert parsed values are `long` and `double`.

- [x] **Step 2: Run parser tests to verify failure**

Run: `dotnet test test/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj --filter FullyQualifiedName~OscMessageParserTests`

Expected: FAIL because tag `h` or `d` is unsupported.

- [x] **Step 3: Implement parser support**

Add `ReadInt64` and `ReadDouble`, and handle tags `h` and `d` in `ParseArguments`.

- [x] **Step 4: Run parser tests to verify pass**

Run the same filtered command.

Expected: PASS.

### Task 2: Multi-Event OSC Mapping

**Files:**
- Modify: `src/StarDust.CasparCG/Osc/IOscMessageMapper.cs`
- Modify: `src/StarDust.CasparCG/Osc/DefaultOscMessageMapper.cs`
- Modify: `src/StarDust.CasparCG/CasparClient.cs`
- Create: `src/StarDust.CasparCG/Events/OscStateChangedEvent.cs`
- Create: `src/StarDust.CasparCG/Events/LayerProducerChangedEvent.cs`
- Create: `src/StarDust.CasparCG/Events/LayerPausedChangedEvent.cs`
- Create: `src/StarDust.CasparCG/Events/LayerProgressChangedEvent.cs`
- Create: `src/StarDust.CasparCG/Events/LayerFramesLeftChangedEvent.cs`
- Test: `test/StarDust.CasparCG.UnitTests/OscMessageParserTests.cs`
- Test: `test/StarDust.CasparCG.UnitTests/OscClientTests.cs`

- [x] **Step 1: Write failing mapper tests**

Add tests proving one OSC message can emit `OscStateChangedEvent` plus a typed event for clip, producer, paused, progress, and frames-left paths.

- [x] **Step 2: Run mapper tests to verify failure**

Run: `dotnet test test/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj --filter "FullyQualifiedName~OscMessageParserTests|FullyQualifiedName~OscClientTests"`

Expected: FAIL because the new event types and multi-event mapper API do not exist.

- [x] **Step 3: Implement events and mapper**

Change `IOscMessageMapper` to return `IReadOnlyList<CasparEvent> Map(...)`. Implement raw channel mapping and selected typed mappings in `DefaultOscMessageMapper`. Update `CasparClient.OnOscPacketAsync` to publish all mapped events.

- [x] **Step 4: Run mapper tests to verify pass**

Run the same filtered command.

Expected: PASS.

### Task 3: State Projection

**Files:**
- Modify: `src/StarDust.CasparCG/State/CasparStateSnapshot.cs`
- Modify: `src/StarDust.CasparCG/State/CasparStateStore.cs`
- Test: `test/StarDust.CasparCG.UnitTests/EventStreamAndStateTests.cs`

- [x] **Step 1: Write failing state tests**

Add a test that applies typed layer events and asserts clip, producer, paused, progress, and frames-left are projected.

- [x] **Step 2: Run state tests to verify failure**

Run: `dotnet test test/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj --filter FullyQualifiedName~EventStreamAndStateTests`

Expected: FAIL because snapshot fields do not exist.

- [x] **Step 3: Implement state projection**

Extend `LayerStateSnapshot` and update `CasparStateStore.Apply` to merge changes without dropping existing fields.

- [x] **Step 4: Run state tests to verify pass**

Run the same filtered command.

Expected: PASS.

### Task 4: REST SSE Event Names

**Files:**
- Modify: `src/StarDust.CasparCG.AspNetCore/Sse/CasparSseWriter.cs`
- Test: `test/StarDust.CasparCG.IntegrationTests/CasparRestApiSseTests.cs`

- [x] **Step 1: Write failing SSE tests**

Add tests for `oscStateChanged`, `layerProducerChanged`, `layerPausedChanged`, `layerProgressChanged`, and `layerFramesLeftChanged`.

- [x] **Step 2: Run SSE tests to verify failure**

Run: `dotnet test test/StarDust.CasparCG.IntegrationTests/StarDust.CasparCG.IntegrationTests.csproj --filter FullyQualifiedName~CasparRestApiSseTests`

Expected: FAIL for new event names.

- [x] **Step 3: Implement stable event names**

Update `CasparSseWriter.ToEventName` with explicit cases for every new event type.

- [x] **Step 4: Run SSE tests to verify pass**

Run the same filtered command.

Expected: PASS.

### Task 5: Documentation and Full Validation

**Files:**
- Modify: `docs/vnext/events-and-state.md`
- Modify: `docs/vnext/rest-api-addon.md`
- Modify: `README.md`

- [x] **Step 1: Update docs**

Document raw OSC fallback, typed events, SSE names, payload examples, and current typed coverage limits.

- [x] **Step 2: Run focused and full validation**

Run:

```bash
dotnet test src/StarDust.CasparCG.net.sln
```

Expected: PASS.

- [x] **Step 3: Review git diff**

Run: `git diff --stat` and inspect changed files for scope.

Expected: only OSC events, SSE names, tests, docs, and the implementation plan are changed.

