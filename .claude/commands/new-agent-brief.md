---
description: Create a new agent brief for a service or repo from the canonical template
argument-hint: <service or repo name>
---

# /new-agent-brief

Create a new agent brief in `docs/documentation/agents/` from the canonical template.

## Steps

1. **Read the template:** `docs/templates/agent-brief-template.md`.

2. **Prompt the user for required fields** (use AskUserQuestion if not provided as an argument):
   - **Service/repo name** — the thing this brief covers. If `$ARGUMENTS` is non-empty, treat it as the name.
   - **Author** — default to the current git user.name.
   - **Affected repos** — default to a list containing the slugified name.

3. **Slugify the name:** lowercase, hyphen-separated, drop punctuation.

4. **Construct the filename:** `docs/documentation/agents/<slug>.md`.

5. **Fill the template:**
   - `id: AGENT-<slug>`
   - `title: "Agent brief: <name>"`
   - `status: active`
   - `created: <today's date in YYYY-MM-DD>`
   - `author: <author>`
   - `reviewers: []`
   - `affected_repos: [<repos>]`
   - Replace `<service or repo name>` placeholders in the body with the real name.
   - Leave the body sections (What it is, Tech stack, Entry points, etc.) for the user.

6. **Write the file** at the constructed path.

7. **Update `docs/SUMMARY.md`:** add a bullet under the "Agent briefs" heading linking the new file.

8. **Open the file** for the user to edit and remind them:
   - The brief lives alongside the code and should stay in sync. If an agent has to ask "what does this service do?", the brief is incomplete.
   - The audience is AI agents about to write code in that service — be specific about gotchas and non-obvious behavior.

## Notes

- Do not overwrite an existing brief. If the file already exists, ask whether the user wants to update it instead.
- Briefs are not specs — they describe current state, not intent. Update freely as the code changes.
