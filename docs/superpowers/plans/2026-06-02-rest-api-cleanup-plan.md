# REST API Cleanup Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** remove the obsolete `src/StarDust.CasparCG.net.RestApi` residue from the repository and confirm no active code references it.

**Architecture:** this cleanup is intentionally narrow. The REST API folder is no longer part of the supported build or solution path, so the implementation is a filesystem deletion plus a reference sweep in repo docs and source files. The active vNext projects under `src/StarDust.CasparCG*` remain untouched.

**Tech Stack:** Git, shell search tools, .NET 10 repo layout, markdown docs

---

### Task 1: Remove the REST API residue

**Files:**
- Delete: `src/StarDust.CasparCG.net.RestApi/`

- [ ] **Step 1: Verify the folder is only residue**

Run:
```bash
find src/StarDust.CasparCG.net.RestApi -maxdepth 2 -type f | sort
```
Expected: only `localDb.db` or other non-source residue files.

- [ ] **Step 2: Delete the folder**

Run:
```bash
rm -rf src/StarDust.CasparCG.net.RestApi
```

- [ ] **Step 3: Verify the folder is gone**

Run:
```bash
test ! -e src/StarDust.CasparCG.net.RestApi
```
Expected: exit code `0`.

- [ ] **Step 4: Commit**

```bash
git add -A src/StarDust.CasparCG.net.RestApi
git commit -m "chore: remove obsolete rest api residue"
```

### Task 2: Confirm no stale references remain

**Files:**
- Modify only if needed:
  - `README.md`
  - `BREAKING_CHANGES.md`
  - `CONTRIBUTING.md`
  - `docs/**`
  - `src/StarDust.CasparCG.net.sln`

- [ ] **Step 1: Search for stale references**

Run:
```bash
grep -R "StarDust.CasparCG.net.RestApi\|Rest API\|REST API\|rest api" -n README.md BREAKING_CHANGES.md CONTRIBUTING.md docs src 2>/dev/null
```
Expected: no active references to the removed project. Documentation mentions of the concept are allowed only if they are generic and not pointing to the removed folder.

- [ ] **Step 2: Update docs only if the search finds an active reference**

If the search returns a concrete file or solution reference, remove or rewrite that reference so it no longer points to `src/StarDust.CasparCG.net.RestApi`.

- [ ] **Step 3: Re-run the search**

Run the same `grep` command again.
Expected: no stale project references.

- [ ] **Step 4: Commit any doc fixups**

```bash
git add README.md BREAKING_CHANGES.md CONTRIBUTING.md docs src/StarDust.CasparCG.net.sln
git commit -m "docs: remove rest api references"
```

### Task 3: Final validation

**Files:**
- None

- [ ] **Step 1: Check the repo state**

Run:
```bash
git status --short
```
Expected: clean worktree.

- [ ] **Step 2: Check the solution list**

Run:
```bash
dotnet sln src/StarDust.CasparCG.net.sln list
```
Expected: only the maintained vNext projects remain listed.

- [ ] **Step 3: Record the outcome**

If everything is clean, report the deleted folder and the verification results. If any stale reference remains, fix it before reporting done.
