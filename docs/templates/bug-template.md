---
id: BUG-NNN
title: ""
status: draft
severity: P2
created: YYYY-MM-DD
author: ""
reviewers: []
affected_repos: []
---

<!--
Severity guide:
  P0 — Production down, data loss, security incident. Fix now.
  P1 — Core feature broken for most users. Fix this week.
  P2 — Significant bug affecting some users or a non-core path. Fix in the current cycle.
  P3 — Cosmetic, low-frequency, or easy workaround exists.
-->

## Reviewer Notes

<!-- Leave empty until code review. When requesting changes, reviewer adds feedback here. -->

---

# Bug: <title>

## Summary

<!-- One sentence: what is broken and what should happen instead? -->

## Reproduction

**Environment:**

- Version / commit:
- OS / platform:
- Configuration:

**Steps:**

1. ...
2. ...
3. ...

**Observed:** ...

**Expected:** ...

## Impact

<!-- Who is affected, how often, and what is the workaround? -->

## Root cause

<!-- Fill in once diagnosed. Link the file/line that's responsible. -->

## Fix

### Approach

<!-- How will it be fixed? Reference the failing test that will go RED first. -->

### Tasks

- [ ] Reproduce locally
- [ ] Add failing test
- [ ] Implement fix
- [ ] Verify test passes
- [ ] Regression-check related code paths

## Acceptance criteria

1. The reproduction steps above no longer trigger the bug.
2. A regression test exists and would have caught the original bug.

## Notes

<!-- Related bugs, prior investigation, anything that didn't fit above. -->
