---
description: Create a new Architecture Decision Record from the canonical template
argument-hint: <short decision title>
---

# /new-adr

Create a new ADR in `docs/specs/adrs/` from the canonical template.

## Steps

1. **Read the template:** `docs/templates/adr-template.md`.

2. **Compute the next ADR ID:**
   - List existing files in `docs/specs/adrs/` matching pattern `ADR-NNNN-*.md` (4-digit zero-padded).
   - Take the maximum numeric prefix and add 1. If none exist, start at `0001`.

3. **Prompt the user for required fields** (use AskUserQuestion if not provided as an argument):
   - **Title** — concise statement of the decision. If `$ARGUMENTS` is non-empty, treat it as the title.
   - **Author** — default to the current git user.name.
   - **Supersedes** — optional ADR ID(s) this one replaces.

4. **Slugify the title:** lowercase, hyphen-separated, drop punctuation, max 6 words.

5. **Construct the filename:** `docs/specs/adrs/ADR-NNNN-<slug>.md`.

6. **Fill the template:**
   - `id: ADR-NNNN`
   - `title: "<title>"`
   - `status: proposed`
   - `created: <today's date in YYYY-MM-DD>`
   - `author: <author>`
   - `reviewers: []`
   - `supersedes: [<list>]` (defaults to `[]`)
   - `superseded_by: []`
   - Replace `# ADR-NNNN: <title>` with the real ID and title.
   - Leave Context, Decision, Consequences, Alternatives, References for the user.

7. **If `supersedes` is non-empty:** also edit each superseded ADR's frontmatter — set `superseded_by` to include this new ID and (if not already `deprecated`) change its status to `superseded`. Do NOT delete the superseded files; ADRs are permanent.

8. **Write the file** at the constructed path.

9. **Open the file** for the user to edit and remind them:
   - ADRs are permanent. Reversing a decision means writing a new ADR that supersedes this one — not editing or deleting.
   - Status starts at `proposed`. Flip to `accepted` after review.

## Notes

- ADR IDs use 4 digits to leave room. The seed ADR is `ADR-0001`.
- If the computed filename collides, abort.
