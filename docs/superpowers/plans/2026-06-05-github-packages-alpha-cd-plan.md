# GitHub Packages Alpha CD Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Add a dedicated prerelease workflow that publishes `StarDust.CasparCG` alpha packages to GitHub Packages from `feature/**` and `mr/**` branches using version format `10.0.0-alpha.<run-number>`.

**Architecture:** Keep the existing stable `package.yml` workflow unchanged and introduce a separate `package-prerelease.yml` workflow for branch-based alpha publication. Versioning stays outside the csproj by injecting `PackageVersion` during `dotnet pack`, and local consumption guidance is documented alongside the workflow.

**Tech Stack:** GitHub Actions, .NET 10 CLI, NuGet/GitHub Packages, Markdown docs

---

## File Map

- Create: `.github/workflows/package-prerelease.yml`
- Modify: `README.md`
- Keep unchanged but verify compatibility: `.github/workflows/package.yml`
- Keep unchanged but use at pack time: `src/StarDust.CasparCG/StarDust.CasparCG.csproj`

### Task 1: Define the alpha package workflow contract

**Files:**
- Create: `.github/workflows/package-prerelease.yml`

- [ ] **Step 1: Write the failing prerelease workflow file**

Create `.github/workflows/package-prerelease.yml` with this skeleton:

```yaml
name: package-prerelease

on:
  workflow_dispatch:
  push:
    branches:
      - feature/**
      - mr/**

permissions:
  contents: read
  packages: write

jobs:
  pack-prerelease:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v4
        with:
          dotnet-version: 10.0.x
          cache: true
          cache-dependency-path: src/**/packages.lock.json
      - name: Compute alpha package version
        run: echo "PACKAGE_VERSION=10.0.0-alpha.${GITHUB_RUN_NUMBER}" >> "$GITHUB_ENV"
      - name: Pack prerelease
        run: dotnet pack src/StarDust.CasparCG/StarDust.CasparCG.csproj --configuration Release -p:PackageVersion=${PACKAGE_VERSION} -p:BuildInParallel=false -o artifacts/packages
      - uses: actions/upload-artifact@v4
        with:
          name: prerelease-packages
          path: artifacts/packages
```

- [ ] **Step 2: Verify the workflow is still incomplete**

Run:

```bash
sed -n '1,240p' .github/workflows/package-prerelease.yml
```

Expected: the workflow exists but does not yet publish to GitHub Packages or document local usage.

- [ ] **Step 3: Commit the workflow contract**

```bash
git add .github/workflows/package-prerelease.yml
git commit -m "ci: add prerelease package workflow skeleton"
```

### Task 2: Add real GitHub Packages publication

**Files:**
- Modify: `.github/workflows/package-prerelease.yml`

- [ ] **Step 1: Extend the workflow with explicit package source and push**

Update `.github/workflows/package-prerelease.yml` so the job includes:

```yaml
      - name: Add GitHub Packages source
        run: dotnet nuget add source "https://nuget.pkg.github.com/${{ github.repository_owner }}/index.json" --name github --username "${{ github.actor }}" --password "${{ secrets.GITHUB_TOKEN }}" --store-password-in-clear-text

      - name: Publish prerelease package
        run: dotnet nuget push "artifacts/packages/*.nupkg" --source github --api-key "${{ secrets.GITHUB_TOKEN }}" --skip-duplicate
```

And the full workflow should read:

```yaml
name: package-prerelease

on:
  workflow_dispatch:
  push:
    branches:
      - feature/**
      - mr/**

permissions:
  contents: read
  packages: write

concurrency:
  group: package-prerelease-${{ github.ref }}
  cancel-in-progress: true

jobs:
  pack-prerelease:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4

      - uses: actions/setup-dotnet@v4
        with:
          dotnet-version: 10.0.x
          cache: true
          cache-dependency-path: src/**/packages.lock.json

      - name: Compute alpha package version
        run: echo "PACKAGE_VERSION=10.0.0-alpha.${GITHUB_RUN_NUMBER}" >> "$GITHUB_ENV"

      - name: Restore package project
        run: dotnet restore src/StarDust.CasparCG/StarDust.CasparCG.csproj

      - name: Pack prerelease
        run: dotnet pack src/StarDust.CasparCG/StarDust.CasparCG.csproj --configuration Release --no-restore -p:PackageVersion=${PACKAGE_VERSION} -p:BuildInParallel=false -o artifacts/packages

      - name: Add GitHub Packages source
        run: dotnet nuget add source "https://nuget.pkg.github.com/${{ github.repository_owner }}/index.json" --name github --username "${{ github.actor }}" --password "${{ secrets.GITHUB_TOKEN }}" --store-password-in-clear-text

      - name: Publish prerelease package
        run: dotnet nuget push "artifacts/packages/*.nupkg" --source github --api-key "${{ secrets.GITHUB_TOKEN }}" --skip-duplicate

      - uses: actions/upload-artifact@v4
        with:
          name: prerelease-packages
          path: artifacts/packages
```

- [ ] **Step 2: Verify the alpha versioning rule is present**

Run:

```bash
grep -n "10.0.0-alpha" .github/workflows/package-prerelease.yml
```

Expected: one line showing `PACKAGE_VERSION=10.0.0-alpha.${GITHUB_RUN_NUMBER}`.

- [ ] **Step 3: Commit the publish-capable workflow**

```bash
git add .github/workflows/package-prerelease.yml
git commit -m "ci: publish alpha packages to github packages"
```

### Task 3: Document local consumption

**Files:**
- Modify: `README.md`

- [ ] **Step 1: Add a short alpha package consumption section**

Insert this section near the existing guide links in `README.md`:

```md
## Alpha packages

Feature and MR branches can publish prerelease packages to GitHub Packages using versions like `10.0.0-alpha.<run-number>`.

To consume them locally, add the GitHub Packages feed:

```bash
dotnet nuget add source "https://nuget.pkg.github.com/dust63/index.json" \
  --name github-dust63 \
  --username "<github-username>" \
  --password "<github-personal-access-token>" \
  --store-password-in-clear-text
```

Then install the package normally:

```bash
dotnet add package StarDust.CasparCG --version 10.0.0-alpha.<run-number>
```
```

- [ ] **Step 2: Verify the docs mention GitHub Packages and alpha versioning**

Run:

```bash
grep -n "Alpha packages\\|nuget.pkg.github.com\\|10.0.0-alpha" README.md
```

Expected: three matching lines or more in the new section.

- [ ] **Step 3: Commit the docs update**

```bash
git add README.md
git commit -m "docs: explain alpha package consumption"
```

### Task 4: Final validation

**Files:**
- None

- [ ] **Step 1: Verify the stable workflow stayed unchanged**

Run:

```bash
git diff -- .github/workflows/package.yml
```

Expected: no output.

- [ ] **Step 2: Re-read the prerelease workflow**

Run:

```bash
sed -n '1,260p' .github/workflows/package-prerelease.yml
```

Expected: branch triggers, `packages: write`, alpha version computation, GitHub Packages source setup, publish step, and artifact upload all present.

- [ ] **Step 3: Verify docs and workflow references together**

Run:

```bash
grep -n "10.0.0-alpha" .github/workflows/package-prerelease.yml README.md
```

Expected: matches in both files.

- [ ] **Step 4: Inspect repo state**

Run:

```bash
git status --short
```

Expected: clean worktree after commits.

- [ ] **Step 5: Record the outcome**

Report:
- the new workflow path
- the trigger branches
- the alpha version format
- the GitHub Packages feed URL documented for local usage
