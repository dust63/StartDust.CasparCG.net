# Test Layout Cleanup Design

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** move the maintained test projects out of `src/` into a dedicated top-level `test/` tree while keeping the current project identities and runtime code unchanged.

**Architecture:** the repository will keep one runtime tree under `src/StarDust.CasparCG*` and one test tree under `test/StarDust.CasparCG*`. The move is purely structural: project names, namespaces, package references, and test behavior stay the same. `StarDust.CasparCG.Testing` moves with the other test projects because it is test infrastructure, not runtime code.

**Tech Stack:** .NET 10, SDK-style projects, xUnit, `dotnet sln`, `dotnet test`, `dotnet build`

---

### Scope

Move these maintained test-related projects from `src/` to `test/`:

- `src/StarDust.CasparCG.UnitTests`
- `src/StarDust.CasparCG.IntegrationTests`
- `src/StarDust.CasparCG.Testing`

Leave runtime code under `src/StarDust.CasparCG*` and do not rename assemblies or namespaces unless a path update requires it in project metadata.

### Task 1: Move the test projects under `test/`

**Files:**
- Move: `src/StarDust.CasparCG.UnitTests/**` -> `test/StarDust.CasparCG.UnitTests/**`
- Move: `src/StarDust.CasparCG.IntegrationTests/**` -> `test/StarDust.CasparCG.IntegrationTests/**`
- Move: `src/StarDust.CasparCG.Testing/**` -> `test/StarDust.CasparCG.Testing/**`

- [ ] **Step 1: Verify the current test project paths**

Run:
```bash
find src -maxdepth 2 -name '*Tests.csproj' -o -name '*Testing.csproj' | sort
```
Expected: the three maintained test projects are currently under `src/`.

- [ ] **Step 2: Move the directories**

Move the three directories into `test/` with the same folder names.

- [ ] **Step 3: Verify the new layout**

Run:
```bash
find test -maxdepth 2 -name '*.csproj' | sort
```
Expected: the three maintained test projects now live under `test/`.

- [ ] **Step 4: Commit**

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
- Modify if needed: any project files that still reference the old relative paths

- [ ] **Step 1: Update the solution paths**

Replace the old `src/...` entries in `src/StarDust.CasparCG.net.sln` so they point to the `test/...` project paths.

- [ ] **Step 2: Fix project references**

Update the moved test project files so their `ProjectReference` entries still point to the runtime projects under `src/StarDust.CasparCG*`.

- [ ] **Step 3: Re-run a stale-reference search**

Run:
```bash
grep -R "src/StarDust.CasparCG.UnitTests\|src/StarDust.CasparCG.IntegrationTests\|src/StarDust.CasparCG.Testing" -n src test README.md CONTRIBUTING.md BREAKING_CHANGES.md docs 2>/dev/null
```
Expected: no active references to the old `src/` test paths.

- [ ] **Step 4: Commit reference updates**

```bash
git add src/StarDust.CasparCG.net.sln test/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj test/StarDust.CasparCG.IntegrationTests/StarDust.CasparCG.IntegrationTests.csproj test/StarDust.CasparCG.Testing/StarDust.CasparCG.Testing.csproj
git commit -m "chore: update solution paths for test layout"
```

### Task 3: Final validation and documentation alignment

**Files:**
- Modify only if needed:
  - `README.md`
  - `CONTRIBUTING.md`
  - `BREAKING_CHANGES.md`
  - `docs/vnext/**`

- [ ] **Step 1: Validate the new solution layout**

Run:
```bash
dotnet sln src/StarDust.CasparCG.net.sln list
```
Expected: the maintained test projects appear from `test/...`, not `src/...`.

- [ ] **Step 2: Run the standard validation**

Run:
```bash
dotnet test test/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj -c Release -v minimal
dotnet test test/StarDust.CasparCG.IntegrationTests/StarDust.CasparCG.IntegrationTests.csproj -c Release -v minimal
dotnet build src/StarDust.CasparCG/StarDust.CasparCG.csproj -c Release -p:TreatWarningsAsErrors=true -m:1 -v minimal
```
Expected: all pass with zero warnings.

- [ ] **Step 3: Update docs if they still say tests live under `src/`**

If any active doc still instructs users to run tests from `src/StarDust.CasparCG.UnitTests` or `src/StarDust.CasparCG.IntegrationTests`, rewrite those paths to `test/...`.

- [ ] **Step 4: Commit final cleanup**

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
