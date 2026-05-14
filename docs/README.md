---
title: Blam! Documentation
---

# Documentation

This folder is the single source of truth for everything you can't read out of the code: specs, decisions, agent briefs, and process. If a question can be answered by `git log` or by reading `Woop/`, look there first. Otherwise it should live in here.

## Folder rules

| Folder | What goes here | Lifetime |
|---|---|---|
| `templates/` | Canonical document templates. Do not modify without an ADR. | Permanent |
| `specs/plans/` | Multi-phase project plans and roadmaps | Until plan completes |
| `specs/active/` | Feature and bug specs currently in flight (`draft` → `complete`) | Until complete, then move to `archive/` |
| `specs/adrs/` | Architecture Decision Records | **Permanent — never archive or delete** |
| `specs/archive/` | Completed or deprecated specs | Permanent (read-only history) |
| `documentation/agents/` | Service/repo briefs written for AI agents | Maintained alongside code |
| `documentation/guides/` | How-to procedures for humans | Maintained alongside code |
| `documentation/deep-dives/` | Narrow technical deep dives | Maintained alongside code |
| `documentation/repos/` | Repo-level technical references | Maintained alongside code |
| `archive/` | General archive for retired non-spec docs | Permanent (read-only history) |

## Spec lifecycle

```
draft → ready → in-progress → review-pending → complete
                                                  ↓
                                              archive/

(deprecated may be reached from any status)
```

- `draft` — initial write-up; expect changes
- `ready` — reviewed; implementation can start
- `in-progress` — code is being written
- `review-pending` — code is open as a PR
- `complete` — merged; spec moves to `archive/`
- `deprecated` — abandoned; stays in `archive/` with a note

ADRs use a different lifecycle: `proposed → accepted` (or `superseded`/`deprecated`). They never leave `specs/adrs/`.

## Workflows

### Starting a new feature

1. Run `/new-spec`. The slash command creates a new file from `templates/feature-template.md` in `specs/active/`.
2. Fill in `Problem`, `Requirements` (Must / Nice-to-have / Non-goals), and `Acceptance criteria`. Leave `Design` and `Tasks` for later.
3. Status `draft`. Request review.
4. After reviewer approval, status `ready`. Now you can branch and code.
5. Open a PR linking the spec ID. Status `review-pending`.
6. On merge, status `complete` and move the file to `specs/archive/`.

### Reporting a bug

Run `/new-bug`. Same flow as features but the template includes severity (P0–P3) and a reproduction section. Bugs live in `specs/active/` until fixed.

### Recording an architectural decision

Run `/new-adr`. ADRs are short: Context, Decision, Consequences, Alternatives. Once accepted, the decision is binding — supersede with a new ADR rather than editing.

### Onboarding an AI agent to a service

Run `/new-agent-brief`. The brief lives in `documentation/agents/<service>.md` and stays current with the code. If the agent has to ask "what does this service do?", the brief is incomplete.

## Prompt cookbook

When asking an AI agent to do work in this repo, point at the relevant spec rather than re-explaining context. Examples:

- *"Implement `docs/specs/active/SPEC-007-export-rtf.md`. Use TDD per `docs/documentation/development-standards.md`."*
- *"Investigate `docs/specs/active/BUG-014-script-picker-crash.md`. Reproduce first, write a failing test, then fix."*
- *"Read `docs/documentation/agents/blam.md` before you touch `Woop/Services/ScriptManager.cs`."*

## Source

This documentation system is initialized per [SPEC-DDD-001](https://github.com/coffeegrind123/nextjs-flask/blob/main/docs/specs/plans/SPEC-DDD-001-ddd-initialization.md). The adoption decision is recorded in [ADR-0001](specs/adrs/ADR-0001-adopt-ddd-tdd.md).
