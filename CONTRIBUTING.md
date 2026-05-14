# Contributing to Blam!

Thanks for the interest. This file is the dense, "need to know" guide — read it before your first PR. The longer "how we work" explanation lives in [`docs/README.md`](docs/README.md) and [`docs/documentation/development-standards.md`](docs/documentation/development-standards.md).

## Prerequisites

- **Windows 10** (build 18362 / version 1903) or later. Blam! is UWP; it does not build or run on macOS or Linux.
- **Visual Studio 2019 or 2022**, with these workloads:
  - *Universal Windows Platform development*
  - *.NET desktop development*
  - The Windows 10 SDK 10.0.19041.0 (1903–2004 era) — installed automatically by the UWP workload.
- **Developer Mode** enabled — Settings → Privacy & Security → For developers → *Developer Mode*. Required to install sideloaded packages and to deploy from VS.
- **Git** with submodule support (any recent git).
- A **code-signing certificate** is only required if you intend to submit to the Microsoft Store. Local sideload builds can be unsigned.

## Cloning

**Read this once or your first build will fail in a confusing way.** The repo depends on a git submodule (`submodules/Boop`) whose contents are xcopied into `Woop/Assets/Scripts/` by a pre-build event. Without the submodule, the xcopy fails and you get a cryptic MSBuild error.

```powershell
# Preferred — clone with submodules in one shot:
git clone --recurse-submodules <your-fork-url> blam
cd blam

# Or, if you cloned without --recurse-submodules:
git submodule update --init --recursive
```

To pull upstream Boop script updates later:

```powershell
git submodule update --remote submodules/Boop
git add submodules/Boop
git commit -m "chore: bump Boop submodule"
```

## First build

1. Open `Woop.sln` in Visual Studio.
2. Set the startup project to `Woop` (right-click → *Set as Startup Project*).
3. Pick a platform: `x64` if you have a modern x86_64 machine, `ARM64` for Surface Pro X-class hardware. `x86` works for everything but builds in 32-bit mode.
4. Press **F5**.

## Build pipeline quirks you should know about

### The submodule + pre-build event

`Woop.csproj` has a pre-build event:

```
del /q "$(SolutionDir)Woop\Assets\Scripts\"
xcopy /y /e "$(SolutionDir)submodules\Boop\Boop\Boop\scripts" "$(SolutionDir)Woop\Assets\Scripts\"
```

It wipes `Woop/Assets/Scripts/` (gitignored) and repopulates from the submodule on every build. **Implication:** dropping a JS file into `Woop/Assets/Scripts/` by hand is pointless — it'll be deleted next build. To add a custom built-in script, contribute it upstream to [`IvanMathy/Boop`](https://github.com/IvanMathy/Boop), or use the custom scripts folder feature (Settings → Custom scripts folder).

### Code signing

`Woop.csproj` references certificate thumbprint `9B8BE8375019C354A32D6EFACC0808A7003F2432` — that's upstream maintainer "FS Apps" / Felix Seidl's cert, not yours. A fresh clone does **not** have this cert installed.

Three ways to deal with it:

**A. Build unsigned (simplest for local sideload):**

```powershell
msbuild Woop.sln /p:Configuration=Debug /p:Platform=x64 /p:AppxPackageSigningEnabled=False /restore
```

**B. Generate your own self-signed dev cert:**

```powershell
$cert = New-SelfSignedCertificate `
  -Type CodeSigningCert `
  -Subject "CN=Blam Dev" `
  -KeyUsage DigitalSignature `
  -CertStoreLocation "Cert:\CurrentUser\My" `
  -TextExtension @("2.5.29.37={text}1.3.6.1.5.5.7.3.3", "2.5.29.19={text}")
$cert.Thumbprint   # paste into Woop.csproj <PackageCertificateThumbprint>
```

**C. Use a real publisher cert** (Microsoft Store via Partner Center, or a commercial CA). Required if you plan to ship.

### Microsoft Store identity

`Woop/Package.appxmanifest` has:

```xml
<Identity Name="53621FSApps.41283D331BF23" Publisher="CN=Felix Seidl, O=Felix Seidl, S=Bayern, C=DE" />
```

This identity belongs to the upstream maintainer's Store publisher account. **Do not submit a build with these values to the Store from your account** — it will be rejected or, worse, conflict. Replace `Name` and `Publisher` with values from your own Partner Center registration before any Store submission.

## Development workflow

Blam! follows Document Driven Development + Test Driven Development. The short version:

```
spec → test → implement → review
```

### 1. Start with a spec

Every non-trivial change starts with a spec in `docs/specs/active/`. Use the slash commands:

| Need to… | Run |
|---|---|
| Propose a new feature | `/new-spec` |
| Report a bug | `/new-bug` |
| Record an architectural decision | `/new-adr` |
| Onboard an AI agent to a new service | `/new-agent-brief` |

The slash command creates the file from the canonical template in `docs/templates/` and assigns the next ID. Fill in `Problem`, `Requirements`, and `Acceptance criteria` at minimum, then request review.

A spec moves through `draft → ready → in-progress → review-pending → complete`. Coding starts at `ready`. See [`docs/documentation/development-standards.md`](docs/documentation/development-standards.md) for the full lifecycle.

Trivial changes (typo, dependency bump, one-line fix) can skip the spec — but still need a test if the behavior is testable.

### 2. Write the failing test first

Open `Woop.Tests/`, add a test that asserts the behavior your spec requires, run it, watch it fail. RED.

```csharp
[TestMethod]
public void Selection_Uppercase_Transforms()
{
    // Arrange / Act / Assert against the public surface of Woop.
}
```

Internal types in `Woop` are visible to `Woop.Tests` via `InternalsVisibleTo`. Prefer testing public APIs where possible.

### 3. Make it pass with minimum code

Add just enough to `Woop/` to make the test green. Resist handling edge cases yet — add another failing test first.

### 4. Refactor

With tests green, clean up. Extract helpers, rename, remove duplication. Run the test suite after every meaningful change.

### 5. Open a PR

PR description must include `Implements SPEC-NNN.` or `Fixes BUG-NNN.` and link the spec file. The reviewer checks against the spec's *Acceptance criteria*.

On merge: spec moves from `docs/specs/active/` to `docs/specs/archive/` and its `status` flips to `complete`.

## Branch naming

| Pattern | Use for |
|---|---|
| `feat/SPEC-NNN-short-slug` | New feature implementing a spec |
| `fix/BUG-NNN-short-slug` | Bug fix |
| `docs/topic` | Docs-only change |
| `chore/topic` | Tooling / dependency / build-config |
| `refactor/topic` | Internal restructure, no behavior change |

Slugs: lowercase, hyphen-separated, ≤ 6 words.

## Commit messages

[Conventional Commits](https://www.conventionalcommits.org/):

```
<type>: <≤ 72 char summary>

<body — explain the *why*>

<footer — refs spec, breaking-change notice>
```

Types: `feat`, `fix`, `docs`, `chore`, `refactor`, `test`, `perf`, `style`.

The body matters more than the subject. A future contributor reading `git log -p` shouldn't have to guess motivation.

## Pull request checklist

Before requesting review:

- [ ] Spec ID linked in the PR description (`Implements SPEC-NNN` / `Fixes BUG-NNN`)
- [ ] Tests added or updated for any behavior change, or a one-line note explaining why none were added
- [ ] `Woop.Tests` test suite passes locally
- [ ] User-facing docs (`readme.md`, `CONTRIBUTING.md`, agent brief) updated if behavior is user-visible
- [ ] No cert thumbprints, Store identities, or secrets leaked into the diff
- [ ] `submodules/Boop` pointer not regressed (check `git diff submodules/Boop` is empty unless you meant to bump it)
- [ ] If the spec is `complete`, the file has been moved from `specs/active/` to `specs/archive/` and `status` updated

## Adding a new JS script

The most common contribution. Three paths:

### A. Upstream to Boop (preferred for general-purpose scripts)

The script will ship to both Blam! and macOS Boop. Open a PR at [`IvanMathy/Boop`](https://github.com/IvanMathy/Boop) under `Boop/Boop/scripts/`. Once merged, bump the submodule here:

```powershell
git submodule update --remote submodules/Boop
git add submodules/Boop
git commit -m "chore: bump Boop submodule for <script name>"
```

### B. User-installed custom script

For one-off / personal scripts that shouldn't be upstreamed: drop the `.js` file into your custom scripts folder (set in Settings → Custom scripts folder). No PR needed — these aren't in the repo.

### C. Blam!-only built-in script

**Not currently supported as a one-step contribution.** `Woop/Assets/Scripts/` is wiped by the pre-build xcopy. Shipping a Blam!-only built-in script requires modifying the pre-build event to merge from a second source folder — propose this via `/new-spec` before doing it.

### Script anatomy

```js
/**
    {
        "api":1,
        "name":"Your Script",
        "description":"What it does, one sentence.",
        "author":"Your Name",
        "icon":"flask",
        "tags":"comma,separated,search,terms",
        "bias":0
    }
**/

function main(input) {
    // input.isSelection : bool
    // input.selection   : string (writable)
    // input.fullText    : string (writable)
    // input.text        : string (alias for selection or fullText, writable)
    // input.insertIndex : number
    //
    // input.insert(s), input.postInfo(msg), input.postError(msg)

    input.text = input.text.toUpperCase();
}
```

Metadata is parsed with `AllowTrailingCommas=true` and case-insensitive keys, but the JSON must still be valid (no single quotes, no unquoted keys). Available `icon` names match PNGs in `Woop/Assets/dark/` and `Woop/Assets/light/` (`flask`, `dice`, `globe`, `flip`, `table`, etc.).

The script API is documented in full in [`docs/documentation/agents/blam.md`](docs/documentation/agents/blam.md).

## Reporting bugs

For bugs in **Blam!** (the Windows app): run `/new-bug` to create a tracked bug spec in this repo.

For bugs in **a Boop script** (transformation logic): file at [`IvanMathy/Boop`](https://github.com/IvanMathy/Boop) — the script source lives there.

For bugs in **the V8 engine, MSTest, UWP, etc.**: file with the upstream project.

## License

Blam! is GPLv3 (see [`LICENSE`](LICENSE)). By contributing, you agree your contributions are licensed under the same terms. Don't paste in code from incompatibly-licensed projects.

## Questions

Open an issue on the repo, or reach the maintainer via the contact listed in `Woop/Package.appxmanifest`. (Update this section with your preferred contact channel once your fork has its own home.)
