# CasparCG Structure Cleanup Design

## Goal

Simplify the repository by removing obsolete legacy projects from the repo and collapsing the remaining vNext runtime split so that `StarDust.CasparCG` becomes the single runtime project.

This cleanup has two outcomes:

1. Legacy projects that are no longer part of the supported solution path are deleted from the repository.
2. The active runtime projects `StarDust.CasparCG.Transport`, `StarDust.CasparCG.Protocol.Amcp`, and `StarDust.CasparCG.Protocol.Osc` are merged into `StarDust.CasparCG`.

## Scope

In scope:

- Delete obsolete legacy projects that are no longer part of the active solution.
- Move the active code from:
  - `src/StarDust.CasparCG.Transport`
  - `src/StarDust.CasparCG.Protocol.Amcp`
  - `src/StarDust.CasparCG.Protocol.Osc`
  into `src/StarDust.CasparCG`.
- Remove the three merged `.csproj` files from the active solution.
- Update project references, tests, demos, and docs to reflect the new single-project runtime structure.
- Keep `StarDust.CasparCG.Hosting`, `StarDust.CasparCG.Testing`, `StarDust.CasparCG.UnitTests`, and `StarDust.CasparCG.IntegrationTests` as separate projects.

Out of scope:

- Renaming public namespaces.
- Redesigning the public API.
- Refactoring behavior unrelated to structural cleanup.
- Archiving legacy projects into another folder. They are deleted, not moved.

## Target Structure

After cleanup, the active solution should contain:

- `src/StarDust.CasparCG`
- `src/StarDust.CasparCG.Hosting`
- `src/StarDust.CasparCG.Testing`
- `src/StarDust.CasparCG.UnitTests`
- `src/StarDust.CasparCG.IntegrationTests`

The runtime code that currently lives in separate projects is absorbed into `StarDust.CasparCG` with clear internal folders such as:

- `src/StarDust.CasparCG/Protocol/Amcp/...`
- `src/StarDust.CasparCG/Protocol/Osc/...`
- `src/StarDust.CasparCG/Transport/...`

These folders are organizational only. They do not imply separate projects.

## Namespace Policy

Public namespaces stay stable during this cleanup.

Examples:

- `StarDust.CasparCG.Protocol.Amcp`
- `StarDust.CasparCG.Protocol.Osc`
- `StarDust.CasparCG.Transport`

The file location may change, but namespace identity remains intact to avoid unnecessary breaking changes for tests, demos, and downstream consumers.

## Legacy Project Removal

Projects that are no longer part of the active solution and no longer part of the supported vNext path are deleted from the repository.

This includes the old legacy project family under `src/` such as:

- `StarDust.CasparCG.net.Connection`
- `StarDust.CasparCG.net.Microsoft.DependencyInjection`
- `StarDust.CasparCG.net.Models`
- `StarDust.CasparCG.net.OSC`
- `StarDust.CasparCG.net.OSC.EventHub`
- `StarDust.CasparCg.net.AmcpProtocol`
- `StarDust.CasparCg.net.Device`
- `StartDust.CasparCG.net.Crosscutting`
- old legacy unit test projects tied to that stack

If a remaining demo, test, or doc still references one of these projects, that reference must be removed or updated in the same cleanup pass.

## Runtime Project Merge

`StarDust.CasparCG` becomes the single runtime assembly and absorbs:

- AMCP command model and parsing
- OSC packet parsing and mapping
- TCP/UDP transport types

The merge should preserve existing behavior and existing public entry points.

The cleanup should be performed in this order:

1. Move active source files into `StarDust.CasparCG`.
2. Update project references and compile against the new single-project layout.
3. Remove merged projects from the solution.
4. Delete merged runtime project directories that are no longer needed.
5. Delete obsolete legacy project directories.
6. Update documentation.

## Validation Strategy

Validation should not rely only on the solution-level build because the current environment already shows a restore-graph failure without Roslyn diagnostics.

Required validation:

- Build the active projects individually with warnings as errors where practical.
- Run unit tests.
- Run integration tests.
- Build the AMCP demo.

If the solution-level build still fails with the same restore-graph issue and no code diagnostics, record that explicitly as an environment/tooling limitation rather than a code regression.

## Risks

### Reference breakage

Moving files from separate projects into `StarDust.CasparCG` can break project references and using directives if imports are not updated consistently.

Mitigation:

- Keep namespaces stable.
- Update references immediately after file moves.

### Over-merging into a monolith

Removing projects can make the codebase harder to navigate if the merged runtime has no internal structure.

Mitigation:

- Preserve clear internal folders inside `StarDust.CasparCG`.
- Keep responsibilities separated by folder even though compilation is unified.

### Hidden dependency on legacy code

A demo, test, or document may still refer to a legacy project even if the active solution no longer does.

Mitigation:

- Search the repo for legacy project names before deletion.
- Remove stale references in the same change set.

### Validation ambiguity

The current `dotnet build` behavior at solution level can fail without useful diagnostics.

Mitigation:

- Validate project-by-project and test-by-test.
- Treat the restore-graph issue as a separate infrastructure concern unless a concrete code error appears.

## Success Criteria

The cleanup is successful when:

- obsolete legacy projects are deleted from the repository,
- the three active runtime support projects are absorbed into `StarDust.CasparCG`,
- the active solution contains only the reduced vNext project set,
- namespaces remain stable,
- unit tests pass,
- integration tests pass,
- the AMCP demo builds,
- README and supporting docs describe the simplified structure.
