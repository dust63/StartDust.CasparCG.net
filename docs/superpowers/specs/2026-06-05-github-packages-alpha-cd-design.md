# GitHub Packages Alpha CD Design

## Summary

Add a dedicated prerelease workflow that publishes `StarDust.CasparCG` alpha packages to GitHub Packages from `feature/**` and `mr/**` branches.

## Goal

Allow feature and MR branches to produce installable prerelease NuGet packages without polluting the public NuGet feed.

## Release Model

### Alpha packages

- trigger on pushes to `feature/**`
- trigger on pushes to `mr/**`
- publish to GitHub Packages
- version format: `10.0.0-alpha.<run-number>`

The major version tracks the target .NET version, so as long as the package targets `.NET 10`, prerelease packages start with `10`.

### Stable packages

- remain on the existing stable release path
- remain separate from prerelease publication logic

## Architecture

Use a dedicated workflow instead of extending the existing `package.yml`.

This keeps:

- prerelease logic isolated
- stable publishing safer
- branch-based experimentation easier to debug

## Workflow Responsibilities

The prerelease workflow should:

1. check out the repository
2. set up `.NET 10`
3. compute the prerelease package version from `github.run_number`
4. pack `src/StarDust.CasparCG/StarDust.CasparCG.csproj`
5. publish the resulting package to GitHub Packages
6. upload generated packages as workflow artifacts for inspection

## Versioning Rules

- package version should be injected at pack time, not hardcoded into the project file
- `PackageVersion` should be set by workflow input to MSBuild
- example output: `10.0.0-alpha.142`

## Authentication

- use repository `GITHUB_TOKEN` for GitHub Packages publishing if repository permissions allow it
- workflow needs `packages: write`
- workflow also needs `contents: read`

## Local Consumption

The design should include documentation for local usage:

- GitHub Packages feed URL
- required authentication/token setup
- how to restore and reference alpha packages from a local project

## Constraints

- do not publish prerelease packages to `nuget.org`
- do not break the current stable package workflow
- keep the workflow explicit and easy to audit
- avoid hidden version logic in the csproj

## Affected Files

- `.github/workflows/package-prerelease.yml`
- optionally `README.md` or `docs/` for package feed usage notes

## Validation

- branch push on `feature/**` or `mr/**` should create an alpha package
- generated version should match `10.0.0-alpha.<run-number>`
- package artifact should be uploaded
- publish target should be GitHub Packages only
- existing stable package workflow should remain unchanged
