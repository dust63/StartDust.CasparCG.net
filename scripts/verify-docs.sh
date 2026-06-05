#!/usr/bin/env bash
set -euo pipefail

required_files=(
  README.md
  BREAKING_CHANGES.md
  CONTRIBUTING.md
  docs/vnext/getting-started.md
  docs/vnext/fluent-api-cookbook.md
  docs/vnext/events-and-state.md
  docs/vnext/hosting-and-di.md
  docs/vnext/testing-with-dummy-server.md
)

for file in "${required_files[@]}"; do
  [[ -f "$file" ]] || { echo "missing $file"; exit 1; }
done

grep -q "Breaking changes from the current release" BREAKING_CHANGES.md
grep -q "AddCasparCG" README.md
grep -q "DummyServer" docs/vnext/testing-with-dummy-server.md
