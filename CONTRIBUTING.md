# Contributing

## Workflow

- keep new vNext work under `src/StarDust.CasparCG*`
- prefer adding tests before implementation changes
- use the active solution path under `src/StarDust.CasparCG.net.sln` for validation
- treat `src/StarDust.CasparCG` as the single runtime project; protocol and transport code now live inside it
- keep maintained tests under `test/StarDust.CasparCG*`

## Validation

- run `dotnet test test/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj`
- run `dotnet test test/StarDust.CasparCG.IntegrationTests/StarDust.CasparCG.IntegrationTests.csproj`
- run `dotnet sln src/StarDust.CasparCG.net.sln list`
- run `./scripts/verify-docs.sh`
- run `./scripts/verify-workflows.sh`

## Local Commands

Use the root `Makefile` for the common developer loop:

- `make lint` runs `dotnet format` against the maintained runtime and test projects and fails on formatting drift.
- `make build` builds the solution in `Release` with warnings treated as errors.
- `make test` runs the unit and integration test projects under `test/`.
- `make publish` packs `src/StarDust.CasparCG` into `artifacts/packages`.
- `make clean` removes build outputs and package artifacts.

The `publish` target maps to `dotnet pack` because this repository produces a NuGet package rather than an app deployment bundle.

## Documentation

- update `BREAKING_CHANGES.md` when the public API contract changes
- keep the `docs/vnext` guides aligned with the current examples in tests

## CI

- GitHub Actions validates unit tests, integration tests, and packaging inputs
- keep workflow changes aligned with `./scripts/verify-workflows.sh`
