---
description: Create a new bug report from the canonical template
argument-hint: <short bug summary>
---

# /new-bug

Create a new bug spec in `docs/specs/active/` from the canonical template.

## Steps

1. **Read the template:** `docs/templates/bug-template.md`.

2. **Compute the next bug ID:**
   - List existing files in `docs/specs/active/` and `docs/specs/archive/` matching pattern `BUG-NNN-*.md` (3-digit zero-padded).
   - Take the maximum numeric prefix across both folders and add 1. If none exist, start at `001`.

3. **Prompt the user for required fields** (use AskUserQuestion if not provided as an argument):
   - **Title** — short summary. If `$ARGUMENTS` is non-empty, treat it as the title.
   - **Severity** — one of `P0`, `P1`, `P2`, `P3`. Default `P2`. The template's frontmatter comment block explains the severity scale.
   - **Author** — default to the current git user.name.
   - **Affected repos** — default `[blam]`.

4. **Slugify the title:** lowercase, hyphen-separated, drop punctuation, max 6 words.

5. **Construct the filename:** `docs/specs/active/BUG-NNN-<slug>.md`.

6. **Fill the template:**
   - `id: BUG-NNN`
   - `title: "<title>"`
   - `status: draft`
   - `severity: <severity>`
   - `created: <today's date in YYYY-MM-DD>`
   - `author: <author>`
   - `reviewers: []`
   - `affected_repos: [<repos>]`
   - Replace `# Bug: <title>` with the real title.
   - Leave the body sections for the user.

7. **Write the file** at the constructed path.

8. **Open the file** for the user to edit and tell them:
   - The bug ID and severity.
   - The branch naming convention: `fix/BUG-NNN-<slug>`.
   - That TDD requires writing the failing reproduction test BEFORE the fix — see `docs/documentation/development-standards.md`.

## Notes

- Do not overwrite an existing file. If the computed filename collides, abort.
- ID assignment scans both `active/` and `archive/`.
