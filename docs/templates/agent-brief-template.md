---
id: AGENT-<slug>
title: "Agent brief: <service or repo name>"
status: active
created: YYYY-MM-DD
author: ""
reviewers: []
affected_repos: []
---

## Reviewer Notes

<!-- Leave empty until review. -->

---

# Agent brief: <service or repo name>

> **Audience.** AI agents about to write code for this service. Read top-to-bottom before your first edit.

## What it is

<!-- One paragraph. What does this service/repo do, who calls it, and what does it own? -->

## Tech stack

| Aspect | Value |
|---|---|
| Language | |
| Framework | |
| Build tool | |
| Test framework | |
| Runtime | |

## Entry points

<!-- Where execution starts. Main file, route handlers, CLI command, etc. -->

## Folder map

<!-- Top-level folders and what lives in each. Keep terse. -->

## Public interface

<!-- The contract this service exposes: HTTP routes, CLI flags, library functions, IPC surface, etc. Include shape, not implementation. -->

## Dependencies

### Internal

<!-- Other services/repos this one calls, and what for. -->

### External

<!-- Third-party services, libraries with notable API surface, paid SaaS. -->

## Configuration

<!-- Env vars, config files, secrets. Where they're read from. What happens if they're missing. -->

## How to run locally

```
<commands>
```

## How to test

```
<commands>
```

## Gotchas

<!-- Non-obvious things that will burn you. Surprising defaults, build-step side effects, platform quirks, undocumented assumptions. -->

## Where to look first when…

- **A test fails:** ...
- **The build breaks:** ...
- **Behavior diverges from the spec:** ...

## Out of scope for this agent

<!-- Things this brief deliberately doesn't cover, with pointers to where they do live. -->
