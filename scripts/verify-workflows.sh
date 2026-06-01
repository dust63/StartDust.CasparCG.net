#!/usr/bin/env bash
set -euo pipefail

grep -q "actions/checkout@v4" .github/workflows/ci.yml
grep -q "actions/setup-dotnet@v4" .github/workflows/ci.yml
grep -q "dotnet test src/StarDust.CasparCG.UnitTests/StarDust.CasparCG.UnitTests.csproj" .github/workflows/ci.yml
grep -q "dotnet test src/StarDust.CasparCG.IntegrationTests/StarDust.CasparCG.IntegrationTests.csproj" .github/workflows/ci.yml
grep -q "permissions:" .github/workflows/ci.yml
grep -q "concurrency:" .github/workflows/ci.yml
grep -q "upload-artifact" .github/workflows/ci.yml
grep -q "workflow_dispatch:" .github/workflows/package.yml
