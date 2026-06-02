# Contributing

## Workflow

- keep new vNext work under `src/StarDust.CasparCG*`
- prefer adding tests before implementation changes
- use the active solution path under `src/StarDust.CasparCG.net.sln` for validation

## Validation

- run `dotnet test src/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj`
- run `dotnet test src/StarDust.CasparCG.IntegrationTests/StarDust.CasparCG.IntegrationTests.csproj`
- run `dotnet sln src/StarDust.CasparCG.net.sln list`
- run `./scripts/verify-docs.sh`
- run `./scripts/verify-workflows.sh`

## Documentation

- update `BREAKING_CHANGES.md` when the public API contract changes
- keep the `docs/vnext` guides aligned with the current examples in tests

## CI

- GitHub Actions validates unit tests, integration tests, and packaging inputs
- keep workflow changes aligned with `./scripts/verify-workflows.sh`
