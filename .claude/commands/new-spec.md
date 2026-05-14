---
description: Create a new feature spec from the canonical template
argument-hint: <short feature title>
---

# /new-spec

Create a new feature spec in `docs/specs/active/` from the canonical template.

## Steps

1. **Read the template:** `docs/templates/feature-template.md`.

2. **Compute the next spec ID:**
   - List existing files in `docs/specs/active/` and `docs/specs/archive/` matching pattern `SPEC-NNN-*.md` (3-digit zero-padded).
   - Take the maximum numeric prefix across both folders and add 1. If none exist, start at `001`.

3. **Prompt the user for required fields** (use AskUserQuestion if not provided as an argument):
   - **Title** — short noun phrase. If `$ARGUMENTS` is non-empty, treat it as the title.
   - **Author** — default to the current git user.name.
   - **Affected repos** — default `[blam]`.

4. **Slugify the title:** lowercase, hyphen-separated, drop punctuation, max 6 words.

5. **Construct the filename:** `docs/specs/active/SPEC-NNN-<slug>.md`.

6. **Fill the template:**
   - `id: SPEC-NNN` (with leading zeros)
   - `title: "<title>"`
   - `status: draft`
   - `created: <today's date in YYYY-MM-DD>`
   - `author: <author>`
   - `reviewers: []`
   - `affected_repos: [<repos>]`
   - Replace the heading `# Feature: <title>` with the real title.
   - Leave the body sections (Problem, Requirements, Design, etc.) as-is for the user to fill in.

7. **Write the file** at the constructed path.

8. **Open the file** for the user to edit and tell them:
   - The spec ID.
   - That status starts at `draft`; flip to `ready` after review per `docs/documentation/development-standards.md`.
   - That the branch name convention is `feat/SPEC-NNN-<slug>`.

## Notes

- Do not overwrite an existing file. If the computed filename collides, abort and surface the conflict.
- ID assignment scans both `active/` and `archive/` so IDs are never reused.
