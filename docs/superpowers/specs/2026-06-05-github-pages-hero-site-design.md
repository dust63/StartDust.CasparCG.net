# GitHub Pages Hero Site Design

## Summary

Publish a lightweight GitHub Pages site for `StarDust.CasparCG.net` with:

- a dedicated hero landing page at `/`
- a published documentation area at `/docs/`
- only the public `docs/vnext/*` guides exposed on the website
- no dependency on a frontend framework or server runtime

The landing page is independent from `README.md`. It should act as the public entry point for the project, while the repository README remains optimized for GitHub readers and contributors.

## Goals

- Create a polished public homepage for the vNext library.
- Publish the existing vNext guides on the same GitHub Pages site.
- Keep the site generation and deployment pipeline simple and durable.
- Preserve a clean separation between public documentation and internal project notes.

## Non-Goals

- Introducing a JS framework such as Docusaurus or a static site generator such as MkDocs.
- Publishing internal `docs/superpowers/*` planning and design files.
- Replacing the repository `README.md` with the website homepage.
- Building a dynamic search engine, blog, or server-rendered site.

## User Experience

### Homepage

The root page `/` is a product-facing hero page with a technical follow-through. It should quickly communicate:

- this is a modern .NET client for CasparCG
- it supports async command execution, dependency injection, events, and testing
- it provides fluent APIs over AMCP and OSC workflows

The page should start with a strong visual hero inspired by the Stitch project `Hero Stardust.CasparCg`, then move rapidly into concrete value:

- a short code sample
- capability highlights
- links to the main documentation guides
- a short section describing the library surface and repository structure

This is a mixed product and technical homepage, not a pure marketing page.

### Documentation Area

The `/docs/` section exposes only public vNext guides:

- `getting-started`
- `fluent-api-cookbook`
- `events-and-state`
- `hosting-and-di`
- `testing-with-dummy-server`

The docs experience should stay minimal:

- shared header or top navigation with a link back to `/`
- a simple docs index page
- readable typography and code block styling
- consistent visual language with the homepage

## Information Architecture

### Public Routes

- `/`
  - homepage
- `/docs/`
  - docs index
- `/docs/getting-started/`
- `/docs/fluent-api-cookbook/`
- `/docs/events-and-state/`
- `/docs/hosting-and-di/`
- `/docs/testing-with-dummy-server/`

### Source Boundaries

- `README.md`
  - remains GitHub-focused and is not used as the website source
- `docs/vnext/*.md`
  - remain the source of truth for public guides
- `docs/superpowers/**`
  - remain internal and unpublished

## Architecture

### Source Layout

Add a dedicated website source area, for example:

- `site-src/`
  - homepage source files
  - shared templates or partials
  - static assets such as CSS, JS, and images
  - docs page shell assets

Generate the final published output into a separate static directory inside the repository, for example `site/`, so it can be validated locally before deployment and clearly distinguished from hand-authored site sources.

## Generation Model

Use a lightweight build script to assemble the published site:

1. generate the homepage from static source files
2. convert or wrap each `docs/vnext/*.md` file into a published HTML page under `/docs/`
3. generate a docs index page
4. copy shared CSS, JS, and assets into the output directory

The generation flow must stay deterministic and transparent. A contributor should be able to understand the site build without learning a framework-specific convention system.

## Document Conversion

The docs are published as a simple mirror of the existing public Markdown guides. Since the chosen approach is intentionally lightweight, the conversion layer should be minimal and predictable:

- preserve heading hierarchy from the Markdown source
- preserve fenced code blocks
- preserve inline code, lists, and links
- add a lightweight shared page shell around each converted document

If a Markdown conversion dependency is needed, prefer a small, well-understood tool over a framework runtime.

## Visual Direction

The public site should borrow from the approved Stitch direction:

- dark-first presentation
- high contrast surfaces
- terminal-green accent color
- modern technical typography
- prominent code blocks and architectural feeling

The homepage should feel intentional and premium, but still appropriate for a .NET developer library. It should avoid generic template styling and avoid over-animated behavior.

## Components

### Homepage Sections

- hero section with title, subtitle, and primary actions
- featured C# snippet
- capability grid
- documentation entry section
- repository structure or feature summary section

### Shared Chrome

- compact site header
- shared footer or lightweight closing section
- docs navigation back to homepage

### Docs Shell

- page title
- optional short intro
- content area for rendered Markdown
- simple next-step links back to docs index or homepage

## Deployment

Deploy through GitHub Pages using a GitHub Actions workflow.

The workflow should:

1. build the static site output
2. validate required pages and links
3. publish the generated directory to GitHub Pages

The deployment flow must not publish private planning documents or unrelated repository content.

## Validation

Add lightweight validation around the static site build:

- required homepage file exists
- required docs pages exist
- homepage links to `/docs/`
- docs index links to all published vNext guides
- generated output excludes `docs/superpowers/*`

Validation should fail loudly if expected files are missing or if the build would produce an incomplete public site.

## Risks And Mitigations

### Risk: visual mismatch between homepage and docs

Mitigation:

- use shared CSS tokens and shared top-level layout structure
- keep docs shell intentionally minimal but clearly part of the same site

### Risk: duplicated messaging between README and homepage

Mitigation:

- keep `README.md` repo-focused
- keep homepage public-facing and narrative

### Risk: accidental publication of internal docs

Mitigation:

- whitelist `docs/vnext/*` explicitly instead of publishing the whole `docs/` tree

### Risk: fragile build pipeline

Mitigation:

- use a small script-based build
- keep dependencies minimal
- validate the generated output structure

## Testing Strategy

Use repository-level checks appropriate for a static site slice:

- build script verification
- generated file existence checks
- simple link validation for main internal links

The site work should not require introducing a heavy frontend toolchain unless a future change justifies it.

## Open Decisions Resolved

- GitHub Pages hosts both landing page and docs: yes
- Homepage path: `/`
- Docs path: `/docs/`
- Homepage source derived from README: no
- Docs publication scope: only `docs/vnext/*`
- Homepage style: mixed product and technical
- Primary message: modern .NET client for CasparCG
- Technical stack: lightweight static site

## Expected Outcome

After implementation, the repository will have:

- a public GitHub Pages homepage for `StarDust.CasparCG.net`
- a simple but coherent `/docs/` area for vNext guides
- a lightweight generation and deployment workflow
- a clean separation between public docs and internal design records
