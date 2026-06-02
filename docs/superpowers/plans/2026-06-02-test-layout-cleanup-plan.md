# Test Layout Cleanup Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** move the maintained test projects into a top-level `test/` tree while keeping runtime code under `src/` and preserving project identities.

**Architecture:** the repo will have a clear split between runtime sources in `src/StarDust.CasparCG*` and test-related projects in `test/StarDust.CasparCG*`. Only paths and solution references change; the build graph, namespaces, and package identities stay the same. `StarDust.CasparCG.Testing` moves with the tests because it is test infrastructure, not runtime code.

**Tech Stack:** .NET 10, SDK-style projects, xUnit, `dotnet sln`, `dotnet build`, `dotnet test`

---

### Task 1: Move the test projects into `test/`

**Files:**
- Move: `src/StarDust.CasparCG.UnitTests/**`
- Move: `src/StarDust.CasparCG.IntegrationTests/**`
- Move: `src/StarDust.CasparCG.Testing/**`

- [ ] **Step 1: Confirm the current layout**

Run:
```bash
find src -maxdepth 2 -name '*Tests.csproj' -o -name '*Testing.csproj' | sort
```
Expected: the three maintained test-related projects are still under `src/`.

- [ ] **Step 2: Move the directories**

Move the three directories into `test/` with the same folder names:
```text
test/StarDust.CasparCG.UnitTests
test/StarDust.CasparCG.IntegrationTests
test/StarDust.CasparCG.Testing
```

- [ ] **Step 3: Verify the new layout**

Run:
```bash
find test -maxdepth 2 -name '*.csproj' | sort
```
Expected: the three maintained test-related projects are now under `test/`.

- [ ] **Step 4: Commit the move**

```bash
git add -A src/StarDust.CasparCG.UnitTests src/StarDust.CasparCG.IntegrationTests src/StarDust.CasparCG.Testing test/StarDust.CasparCG.UnitTests test/StarDust.CasparCG.IntegrationTests test/StarDust.CasparCG.Testing
git commit -m "chore: move test projects under test directory"
```

### Task 2: Update solution and project references

**Files:**
- Modify: `src/StarDust.CasparCG.net.sln`
- Modify: `test/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj`
- Modify: `test/StarDust.CasparCG.IntegrationTests/StarDust.CasparCG.IntegrationTests.csproj`
- Modify: `test/StarDust.CasparCG.Testing/StarDust.CasparCG.Testing.csproj`

- [ ] **Step 1: Update the solution entries**

Replace the old `src/...` paths in `src/StarDust.CasparCG.net.sln` with the new `test/...` paths for the moved projects.

- [ ] **Step 2: Fix the moved test project references**

Update the `ProjectReference` entries so the moved projects still reference runtime projects under `src/StarDust.CasparCG*` using the correct relative paths:

```xml
<ItemGroup>
  <ProjectReference Include="..\..\src\StarDust.CasparCG\StarDust.CasparCG.csproj" />
  <ProjectReference Include="..\..\src\StarDust.CasparCG.Hosting\StarDust.CasparCG.Hosting.csproj" />
</ItemGroup>
```

Use the same pattern for the integration and testing projects, keeping only the runtime/test-infrastructure references they already need.

- [ ] **Step 3: Verify no old `src/` test paths remain**

Run:
```bash
grep -R "src/StarDust.CasparCG.UnitTests\|src/StarDust.CasparCG.IntegrationTests\|src/StarDust.CasparCG.Testing" -n src test README.md CONTRIBUTING.md BREAKING_CHANGES.md docs 2>/dev/null
```
Expected: no active references to the old `src/` test paths.

- [ ] **Step 4: Commit the reference update**

```bash
git add src/StarDust.CasparCG.net.sln test/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj test/StarDust.CasparCG.IntegrationTests/StarDust.CasparCG.IntegrationTests.csproj test/StarDust.CasparCG.Testing/StarDust.CasparCG.Testing.csproj
git commit -m "chore: update solution paths for test layout"
```

### Task 3: Final validation and doc alignment

**Files:**
- Modify only if needed:
  - `README.md`
  - `CONTRIBUTING.md`
  - `BREAKING_CHANGES.md`
  - `docs/vnext/**`

- [ ] **Step 1: Confirm the solution view**

Run:
```bash
dotnet sln src/StarDust.CasparCG.net.sln list
```
Expected: the maintained test projects appear from `test/...`, not `src/...`.

- [ ] **Step 2: Run the core validation**

Run:
```bash
dotnet test test/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj -c Release -v minimal
dotnet test test/StarDust.CasparCG.IntegrationTests/StarDust.CasparCG.IntegrationTests.csproj -c Release -v minimal
dotnet build src/StarDust.CasparCG/StarDust.CasparCG.csproj -c Release -p:TreatWarningsAsErrors=true -m:1 -v minimal
```
Expected: all commands succeed with zero warnings.

- [ ] **Step 3: Update docs if a path still points to `src/`**

If active docs still instruct users to run tests from `src/StarDust.CasparCG.UnitTests` or `src/StarDust.CasparCG.IntegrationTests`, rewrite those paths to `test/...`.

- [ ] **Step 4: Commit any doc fixups**

```bash
git add README.md CONTRIBUTING.md BREAKING_CHANGES.md docs src/StarDust.CasparCG.net.sln test
git commit -m "docs: align repo docs with test layout"
```

### Success Criteria

- The maintained test projects live under `test/`
- The active solution references the `test/` paths
- Runtime code remains under `src/StarDust.CasparCG*`
- `dotnet test` and `dotnet build` still pass on the moved projects
- No active documentation points to the old `src/` test paths
