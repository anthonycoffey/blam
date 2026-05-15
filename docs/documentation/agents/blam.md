---
id: AGENT-blam
title: "Agent brief: Blam!"
status: active
created: 2026-05-14
author: Anthony Coffey
reviewers: []
affected_repos: [blam]
---

# Agent brief: Blam!

> **Audience.** AI agents about to write code in `Blam/` or author a new built-in/custom script. Read this first.

## What it is

Blam! is a Windows UWP desktop app: a scriptable text scratchpad. The user types or pastes text, presses **Ctrl+B**, fuzzy-searches a script by name/tags/description, and Enter-executes it. The selected script transforms the text (or selection) via a V8 JavaScript engine and writes the result back into the editor.

This is a fork of upstream [Woop](https://github.com/felixse/Woop), itself a Windows port of macOS [Boop](https://github.com/IvanMathy/Boop). Scripts are sourced from the `submodules/Boop` git submodule, copied into `Blam/Assets/Scripts/` on every build.

## Tech stack

| Aspect | Value |
|---|---|
| Language | C# 8.0 |
| Framework | UWP (`Microsoft.NETCore.UniversalWindowsPlatform` 6.2.14) |
| UI | `Microsoft.UI.Xaml` 2.8.2 (WinUI 2), MVVM via `Microsoft.Toolkit.Mvvm` 7.1.2 |
| Scripting | `Microsoft.ClearScript.V8` 7.3.7 |
| Fuzzy search | `FuseSharp` 1.4.0 |
| Syntax highlighting | `ColorCode.Core` 2.0.14 |
| JSON | `System.Text.Json` 7.0.2 |
| Test framework | MSTest (UWP Unit Test App) — see `Blam.Tests/` |
| Min Windows | 10.0.18362 (Windows 10 1903) |
| Target SDK | 10.0.19041.0 |
| Platforms | x86, x64, ARM, ARM64 |

## Entry points

- **App entry:** `Blam/App.xaml.cs` → `Blam.App` class.
- **Main UI:** `Blam/Views/MainPage.xaml(.cs)` bound to `Blam/ViewModels/MainViewModel.cs`.
- **Script execution:** `Blam/Models/Script.cs::Run()` invokes user JavaScript inside a per-script `V8ScriptEngine`.

## Folder map

```
Blam/
├── Actions/       Custom XAML actions (FocusAction)
├── Converters/    XAML value converters (theme, status color, icon mapping)
├── Models/        Script, ScriptExecution, ScriptMetadata, Selection
├── Services/      ScriptManager, SettingsService, RequireLoader, BoopPseudoLanguage
├── ViewModels/    MainViewModel, ScriptViewModel, SettingsViewModel, StatusViewModel
├── Views/         MainPage + dialogs + LineNumbers + SyntaxHighlightingRichEditBox + RtfFormatter + ColorCodeThemes
├── Assets/
│   ├── Scripts/   Pre-build xcopy from submodules/Boop/Boop/Boop/scripts (gitignored)
│   ├── Require.js Embedded require() polyfill loaded into every V8 engine
│   └── ...        Tile/icon/splash PNGs
├── Properties/    AssemblyInfo, Default.rd.xml
├── App.xaml(.cs)  Application bootstrap
├── Package.appxmanifest
└── Blam.csproj
```

## Script API surface

This is what JS scripts running inside Blam! see. The C# `ScriptExecution` object (`Blam/Models/ScriptExecution.cs`) is exposed to the V8 engine and is the entire contract.

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
    // input.isSelection : bool      — true if the user had text selected when triggering
    // input.selection   : string    — the selected text (writable)
    // input.fullText    : string    — the full editor buffer (writable)
    // input.text        : string    — alias for selection if isSelection, else fullText (writable)
    // input.insertIndex : number    — caret/insertion position
    //
    // input.insert(s)      — insert at the caret
    // input.postInfo(msg)  — show a transient info message in the status bar
    // input.postError(msg) — show a transient error message in the status bar

    input.text = input.text.toUpperCase();
}
```

**Metadata fields:**

| Field | Required | Notes |
|---|---|---|
| `api` | yes | Always `1` for current schema |
| `name` | yes | Displayed in the script picker (fuzzy-weighted 0.9) |
| `description` | yes | Displayed in the picker (fuzzy-weighted 0.2) |
| `tags` | recommended | Comma-separated; fuzzy-weighted 0.6 |
| `icon` | recommended | Name of a PNG in `Blam/Assets/{dark,light}/` (`flask`, `dice`, `globe`, etc.) |
| `bias` | optional | Float that nudges fuzzy ranking; positive = surface earlier |
| `author` | optional | Credit string |

Metadata is parsed from the `/** ... **/` block at the top of the script via `JsonSerializer.Deserialize<ScriptMetadata>` with `AllowTrailingCommas = true, PropertyNameCaseInsensitive = true`. See `Blam/Models/Script.cs`.

## Where scripts live

| Location | Source of truth | Build behavior |
|---|---|---|
| `Blam/Assets/Scripts/` | **None — gitignored** | Wiped + repopulated from the submodule on every build by the pre-build event in `Blam.csproj` |
| `submodules/Boop/Boop/Boop/scripts/` | Upstream Boop repo | Read-only here; PRs for general-purpose scripts go to `IvanMathy/Boop` |
| User-configured custom scripts folder | Loaded at runtime from a path the user sets in Settings | Out of repo |

**Implication:** you cannot ship a Blam!-only built-in script by adding a file to `Blam/Assets/Scripts/` — it will be deleted on the next build. To ship a custom built-in script, either upstream it to Boop or modify the pre-build event to merge from a second source. The latter requires a spec.

## Settings

Persisted via `Windows.Storage.ApplicationData.Current.LocalSettings` (per-user, per-install). See `Blam/Services/SettingsService.cs`. Fields include theme override and custom scripts folder path.

## How to run locally

```powershell
git submodule update --init --recursive
# Open Blam.sln in Visual Studio 2019/2022 (UWP workload installed)
# Press F5
```

See [readme.md](../../../readme.md) for command-line build alternatives.

## How to test

```powershell
# In Visual Studio: Test Explorer → Run All
# Or:
msbuild Blam.sln /t:Restore /p:Configuration=Debug /p:Platform=x86
msbuild Blam.sln /t:Build   /p:Configuration=Debug /p:Platform=x86
vstest.console.exe Blam.Tests\bin\x86\Debug\Blam.Tests.build.appxrecipe
```

Test methodology: see [development-standards.md](../development-standards.md).

## Gotchas

- **Pre-build deletes scripts.** `Blam.csproj` PreBuildEvent runs `del /q` then `xcopy` on `Assets/Scripts/`. Anything you drop into that folder by hand is destroyed at next build.
- **Submodule is mandatory.** Build fails cryptically if `submodules/Boop` isn't initialized — the xcopy errors but the failure surface is one line in MSBuild output.
- **No signing cert configured.** `Blam.csproj` has `<PackageCertificateThumbprint></PackageCertificateThumbprint>` (empty). Local sideload builds need either a self-signed cert or `AppxPackageSigningEnabled=False` for unsigned builds.
- **Store identity is a placeholder.** `Package.appxmanifest` ships `Name="REPLACE.STORE.IDENTITY.NAME"`, `Publisher="CN=REPLACE_PUBLISHER"`, zero-GUID `PhoneProductId`, and `PublisherDisplayName="REPLACE_PUBLISHER_DISPLAY_NAME"`. Replace all four with Partner Center–issued values before any Store submission attempt.
- **Bundle platforms mismatch.** `Blam.csproj` `AppxBundlePlatforms=x86|x64|arm64` (no 32-bit ARM) even though Debug/Release configs exist for ARM. Sideload bundles include x86/x64/arm64 only.
- **V8 native NuGet packages are per-arch.** `Microsoft.ClearScript.V8.Native.win-x86`, `-x64`, `-arm64` are referenced separately. Restoring with the wrong runtime ID can leave one missing.
- **Property casing is intentional.** `ScriptExecution` uses lowercase property names (`isSelection`, `fullText`, etc.) to match the Boop JS API contract; suppressed with `[SuppressMessage("Style", "IDE1006")]`. Do not "fix" the casing.

## Where to look first when…

- **A script fails silently:** `Script.cs::Run()` catches all exceptions and calls `postError`. Set a breakpoint there or check the status bar.
- **The build complains about `xcopy`:** `submodules/Boop` is not initialized. Run `git submodule update --init --recursive`.
- **A new script doesn't appear:** ensure its `/** ... **/` JSON parses (use `System.Text.Json` rules: `AllowTrailingCommas` is on, single quotes are NOT allowed). See `Models/Script.cs::Script(...)`.
- **The script picker mis-ranks:** weights are hardcoded in `ScriptMetadata.cs::Properties` (`Name`: 0.9, `Tags`: 0.6, `Description`: 0.2). Adjust `bias` in script metadata before changing weights.
- **Theme doesn't update:** look in `Converters/` and `Views/ColorCodeThemes.cs`.

## Out of scope for this brief

- Boop's own script authoring docs — see the [Boop repo](https://github.com/IvanMathy/Boop).
- Microsoft Store submission flow — see [readme.md](../../../readme.md) §"Building executables".
- DDD/TDD process — see [development-standards.md](../development-standards.md).
