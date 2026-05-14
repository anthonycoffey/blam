---
title: Development standards
status: active
---

# Development standards

How we work. If your PR ignores this, expect a review comment pointing back here.

## The pipeline

```
spec → test → implement → review
```

Every non-trivial change starts with a spec in [`docs/specs/active/`](../specs/active/). The spec defines *what* and *why*. Tests define *how we'll know it's done*. Implementation comes last.

Trivial changes (typo, dependency bump, single-line fix that doesn't need to be defended) skip the spec — but still get a test if the behavior is testable.

## Spec lifecycle

```
draft → ready → in-progress → review-pending → complete
                                                  ↓
                                              archive/
```

| Status | Meaning | Who moves it |
|---|---|---|
| `draft` | Initial write-up; under iteration | Author |
| `ready` | Reviewed; implementation can begin | Reviewer |
| `in-progress` | Branch open, code being written | Author |
| `review-pending` | PR open, awaiting review | Author (via PR open) |
| `complete` | PR merged | Author (via merge), moves spec to `archive/` |
| `deprecated` | Abandoned at any stage | Anyone, with a note explaining why |

ADRs use `proposed → accepted` (or `superseded` / `deprecated`) and **never leave `specs/adrs/`**.

## Test Driven Development

Tests live in `Woop.Tests/`. The cycle is:

### RED — write a failing test

Open `Woop.Tests/`, add a test that asserts the behavior the spec requires, run it, watch it fail. A test that passes on the first run is a smell — either the behavior already exists, or the test isn't testing what you think.

### GREEN — make it pass with the minimum code

Add just enough code in `Woop/` to make the test green. Resist the urge to handle the next case yet — write a second failing test first.

### REFACTOR — clean up with the test as a safety net

Now that the bar is green, restructure. Extract helpers, rename for clarity, remove duplication. Run tests after every meaningful change. The test suite is your contract that you didn't break anything.

### Order of operations

1. New behavior → RED → GREEN → REFACTOR
2. Bug fix → RED (write the test that reproduces the bug) → GREEN (fix) → REFACTOR
3. Refactor of existing behavior → tests must already be green BEFORE the refactor; if they aren't, you're not refactoring, you're rewriting blind

## UWP testing caveats

Blam!'s test project is a **UWP Unit Test App**, not a console runner. Two consequences:

- Tests launch inside a UWP app host. First-run startup takes a few seconds; this is normal.
- The test project targets the same Windows SDK as `Woop`. If you bump `TargetPlatformVersion` in one, bump it in the other.

**Prefer pure logic over UWP-coupled code for testability.** When a unit of work doesn't depend on `Windows.*` APIs (e.g., parsing the JSDoc metadata block in a script file), it can — and eventually should — live in a `.NET Standard` library that the UWP app references. That refactor isn't done yet; design new code so it could move there without rewrites.

## Branch naming

| Pattern | Use for |
|---|---|
| `feat/SPEC-NNN-short-slug` | New feature implementing a spec |
| `fix/BUG-NNN-short-slug` | Bug fix tied to a bug report |
| `docs/topic` | Docs-only change |
| `chore/topic` | Tooling, dependency, build-config change |
| `refactor/topic` | Internal restructure with no behavior change |

Slugs are lowercase, hyphen-separated, ≤ 6 words. Example: `feat/SPEC-007-export-rtf`.

## Commit messages

[Conventional Commits](https://www.conventionalcommits.org/):

```
<type>: <short summary, ≤ 72 chars>

<body — explains the *why*, not the *what*>

<footer — refs spec ID, breaking-change notice, etc.>
```

Types: `feat`, `fix`, `docs`, `chore`, `refactor`, `test`, `perf`, `style`.

The body matters more than the subject. A future you reading `git log -p` shouldn't have to guess motivation.

## Pull requests

Every PR must:

- Link the spec ID in the description (`Implements SPEC-007.` or `Fixes BUG-014.`)
- Include tests for new/changed behavior, or explain why none were added
- Update user-facing docs (`readme.md`, `CONTRIBUTING.md`) if the change is user-visible
- Move the spec file from `specs/active/` to `specs/archive/` on merge if it's now `complete`
- Not regress the `submodules/Boop` pointer unless intentional

## Out of scope for this document

- Code style / linting (no enforced linter yet; use editor defaults that match the existing C# style in `Woop/`)
- CI/CD (no workflows yet; planned in a future spec)
- Code signing & Microsoft Store submission (see [readme.md](../../readme.md) §"Building executables")
