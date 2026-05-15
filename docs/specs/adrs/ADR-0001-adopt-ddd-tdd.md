---
id: ADR-0001
title: "Adopt Document Driven Development and Test Driven Development"
status: accepted
created: 2026-05-14
author: Anthony Coffey
reviewers: []
supersedes: []
superseded_by: []
---

# ADR-0001: Adopt Document Driven Development and Test Driven Development

## Context

This repository is a fork of the upstream `Woop` project (a Windows port of Boop) that was inherited with:

- No `CONTRIBUTING.md`, no `docs/`, no architecture record.
- No test project of any kind.
- A non-obvious build that silently fails if the `submodules/Boop` git submodule isn't initialized (the pre-build event in `Blam.csproj` xcopies scripts from it).
- Code-signing and Microsoft Store identity baked into `Blam.csproj` and `Package.appxmanifest` that belong to the upstream maintainer.

Before any feature or fix work happens, the repo needs a workflow that prevents the next contributor (human or AI agent) from re-discovering these traps. It also needs a way to lock in non-obvious behavior with executable tests so changes that violate it fail loudly.

## Decision

This repo adopts:

1. **Document Driven Development (DDD)** per [SPEC-DDD-001](https://github.com/coffeegrind123/nextjs-flask/blob/main/docs/specs/plans/SPEC-DDD-001-ddd-initialization.md). Every non-trivial change starts with a spec in `docs/specs/active/`. The spec lifecycle is `draft → ready → in-progress → review-pending → complete`. Specs are written before code and reviewed like code.

2. **Test Driven Development (TDD)** with [MSTest](https://learn.microsoft.com/visualstudio/test/getting-started-with-unit-testing) hosted in a UWP Unit Test App at `Blam.Tests/`. The cycle is RED (write failing test) → GREEN (minimum code to pass) → REFACTOR (clean up with the test as a safety net).

The scaffold lives at:

- `docs/templates/` — canonical feature, bug, ADR, agent-brief templates
- `docs/specs/` — `plans/`, `active/`, `adrs/`, `archive/`
- `docs/documentation/` — operational docs including `development-standards.md`
- `.claude/commands/` — `/new-spec`, `/new-bug`, `/new-adr`, `/new-agent-brief` slash commands
- `Blam.Tests/` — MSTest UWP Unit Test App, referencing `Blam` with `InternalsVisibleTo`

## Consequences

### Positive

- New contributors have a single entry point: `docs/README.md` → `CONTRIBUTING.md` → spec → branch → PR.
- Behavior is defended by tests, not lore. UWP regressions that today need manual click-through start surfacing in `Blam.Tests/`.
- AI agents have anchor documents (`docs/documentation/agents/blam.md`) instead of inferring from code.
- Decisions like this one are recorded in `specs/adrs/` and survive the next "why did we…?" question.

### Negative

- Process overhead. A drive-by typo fix doesn't need a spec; a one-line behavior change does. The line is judgment-dependent and will be argued.
- MSTest UWP test host startup is slow compared to a console runner. Iteration on pure-logic tests is throttled until logic is extracted into a `.NET Standard` library (separately specced).
- ADRs are permanent. Reversing this decision means writing ADR-0002 that supersedes ADR-0001, not deleting this file.

### Neutral

- Spec IDs and ADR IDs become a vocabulary that contributors have to learn — but the slash commands generate them.

## Alternatives considered

### Ad-hoc development

How the upstream got into this state. Rejected: the inherited mess (undocumented submodule dependency, opaque cert thumbprint, no tests) is the consequence we want to avoid going forward.

### Spec-only without TDD

Specs alone catch *intent* mismatches but not *implementation* regressions. UWP apps are particularly hard to regression-test manually (multi-platform, dialog states, focus behavior). Rejected: without tests, "spec says X, code does Y" gets caught only at PR review, if at all.

### xUnit or NUnit instead of MSTest

xUnit's UWP support is weaker — it typically requires extracting testable logic into a `.NET Standard` library first, which is a larger refactor than this repo is ready for. MSTest has a first-party UWP "Unit Test App" template that runs inside a UWP host with no logic extraction needed. Rejected for now; revisit if/when the `.NET Standard` extraction happens.

### Defer DDD/TDD until after the first feature ships

Tempting but backwards: the first feature is exactly when conventions get cemented. Rejected.

## References

- [SPEC-DDD-001 — Document Driven Development — Project Initialization](https://github.com/coffeegrind123/nextjs-flask/blob/main/docs/specs/plans/SPEC-DDD-001-ddd-initialization.md)
- [development-standards.md](../../documentation/development-standards.md)
- [agents/blam.md](../../documentation/agents/blam.md)
- [repos/blam.md](../../documentation/repos/blam.md)
