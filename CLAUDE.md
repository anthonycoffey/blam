# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What This Is

**Blam!** is a Windows UWP (Universal Windows Platform) desktop application — a scriptable, AI-powered scratchpad for running text transformations via JavaScript. It is a Windows port of the macOS app [Boop](https://github.com/IvanMathy/Boop), extended with AI model support (Claude, Gemini, OpenAI).

- **Language:** C# 8.0, XAML
- **Framework:** UWP with WinUI 2 (Microsoft.UI.Xaml 2.8.2), MVVM via Microsoft.Toolkit.Mvvm 7.1.2
- **Scripting engine:** Microsoft.ClearScript.V8 (V8 JavaScript)
- **Min Windows version:** 10.0.18362.0 (1903)
- **Build system:** MSBuild / Visual Studio

## Build Commands

All commands run from the repo root in PowerShell or Developer Command Prompt:

```powershell
# Restore and build (Debug, x64)
msbuild Blam.sln /t:Restore /p:Configuration=Debug /p:Platform=x64
msbuild Blam.sln /t:Build /p:Configuration=Debug /p:Platform=x64

# Release sideload build (no signing required)
msbuild Blam.sln /restore /p:Configuration=Release /p:Platform=x64 /p:AppxPackageSigningEnabled=False

# Store submission bundle
msbuild Blam.sln /restore /p:Configuration=Release /p:Platform=x64 /p:AppxBundle=Always /p:AppxBundlePlatforms="x86|x64|arm64" /p:UapAppxPackageBuildMode=StoreUpload
```

The recommended approach for local dev is opening `Blam.sln` in Visual Studio and pressing F5.

## Running Tests

Tests live in `Blam.Tests/` (MSTest UWP Unit Test App). The test host launches inside a UWP app container — first run is slow, this is expected.

```powershell
# Build then run via vstest
msbuild Blam.sln /t:Restore /p:Configuration=Debug /p:Platform=x64
msbuild Blam.sln /t:Build /p:Configuration=Debug /p:Platform=x64
vstest.console.exe Blam.Tests\bin\x64\Debug\Blam.Tests.build.appxrecipe
```

Or use Visual Studio's Test Explorer (Test → Run All Tests).

## Architecture

MVVM pattern throughout:

- `Blam/Models/` — core domain types: `Script`, `ScriptExecution`, `ScriptMetadata`, `Selection`
- `Blam/Services/` — `ScriptManager` (loads/runs scripts), `SettingsService`, `BoopPseudoLanguage` (parses script metadata headers), `RequireLoader` (JS module resolution)
- `Blam/ViewModels/` — `MainViewModel`, `ScriptViewModel`, `SettingsViewModel`, `StatusViewModel`
- `Blam/Views/` — XAML pages (`MainPage`, `SettingsDialog`, `AboutDialog`) plus custom controls (`SyntaxHighlightingRichEditBox`, `RtfFormatter`)
- `Blam/Converters/` — XAML value converters
- `Blam/Actions/` — attached behaviors

## Critical Quirks

### Submodule pre-build sync
`Blam.csproj` has a pre-build event that **deletes and repopulates** `Blam/Assets/Scripts/` from `submodules/Boop`. Never edit files directly in `Blam/Assets/Scripts/` — they will be overwritten. Script changes must either go upstream into the Boop submodule or be placed in the user's local settings folder at runtime.

### Code signing
The project references the upstream maintainer's cert thumbprint (`9B8BE8375019C354A32D6EFACC0808A7003F2432`). For local builds, skip signing:
```
/p:AppxPackageSigningEnabled=False
```

### Store identity
`Package.appxmanifest` contains the upstream Store identity (`53621FSApps.41283D331BF23`). This must be replaced before any Store submission.

## Development Workflow

This project follows a spec-first, TDD workflow: **spec → test → implement → review**. See `docs/documentation/development-standards.md` for the full lifecycle.

Active feature specs live in `docs/specs/active/`. Architecture Decision Records (ADRs) live in `docs/specs/adrs/` and are permanent (never archived).

Useful slash commands for this project:
- `/new-spec` — create a feature spec from template
- `/new-bug` — create a bug report
- `/new-adr` — create an Architecture Decision Record
- `/new-agent-brief` — create an AI agent brief
