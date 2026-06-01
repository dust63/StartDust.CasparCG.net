# Contributing

## Workflow

- keep new vNext work under `src/StarDust.CasparCG*`
- leave legacy projects in place unless the change explicitly targets them
- prefer adding tests before implementation changes

## Validation

- run `dotnet test src/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj`
- run `dotnet test src/StarDust.CasparCG.IntegrationTests/StarDust.CasparCG.IntegrationTests.csproj`
- run `./scripts/verify-docs.sh`
- run `./scripts/verify-workflows.sh`

## Documentation

- update `BREAKING_CHANGES.md` when the public API contract changes
- keep the `docs/vnext` guides aligned with the current examples in tests

## CI

- GitHub Actions validates unit tests, integration tests, and packaging inputs
- keep workflow changes aligned with `./scripts/verify-workflows.sh`
