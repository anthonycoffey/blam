---
id: REPO-blam
title: "Repo reference: Blam!"
status: active
created: 2026-05-14
author: Anthony Coffey
reviewers: []
---

# Repo reference: Blam!

Comprehensive technical reference for the Blam! repository. For the script-authoring API and AI-agent quickstart, see [`agents/blam.md`](../agents/blam.md). For process and workflow, see [`development-standards.md`](../development-standards.md).

## Layout

```
.
├── Woop/                 Main UWP application project
├── Woop.Tests/           MSTest UWP unit test project
├── Woop.sln              Visual Studio solution
├── submodules/Boop/      Git submodule — script source from upstream IvanMathy/Boop
├── icons/                Source assets for app icons
├── Screenshots/          Marketing/store screenshots
├── docs/                 This documentation system
├── .claude/commands/     Slash commands (/new-spec, /new-bug, /new-adr, /new-agent-brief)
├── CONTRIBUTING.md
├── readme.md
├── LICENSE               GPLv3
└── .gitmodules
```

## Build pipeline

The solution builds one app project (`Woop`) and one test project (`Woop.Tests`). The interesting part is the pre-build event on `Woop.csproj`:

```
del /q "$(SolutionDir)Woop\Assets\Scripts\"
xcopy /y /e "$(SolutionDir)submodules\Boop\Boop\Boop\scripts" "$(SolutionDir)Woop\Assets\Scripts\"
```

This wipes `Woop/Assets/Scripts/` (which is gitignored) and repopulates it from the submodule on every build. The submodule must be initialized or the xcopy fails. The `Woop.csproj` then `<Content Include="Assets\Scripts\*" />` ships those files inside the .appx package.

Output layout follows standard UWP conventions:

```
Woop/bin/<Platform>/<Configuration>/
Woop/AppPackages/<Bundle name>_<Version>_Test/
```

`AppxBundlePlatforms=x86|x64|arm64` in `Woop.csproj` means the produced bundle covers those three architectures. The csproj also declares per-platform configurations for `ARM` (32-bit) — these are buildable but not part of the default bundle.

## Configurations & platforms

| Configuration | Platform | Notes |
|---|---|---|
| Debug / Release | x86 | Default startup platform; Prefer32Bit=true |
| Debug / Release | x64 |  |
| Debug / Release | ARM | Buildable, not bundled |
| Debug / Release | ARM64 |  |

`Any CPU` is mapped to `x86` for active config. Release builds enable `UseDotNetNativeToolchain=true` (.NET Native compilation — slower build, faster runtime, harder to debug).

## Dependencies

NuGet `PackageReference` style (see `Woop.csproj`). Key dependencies and what they're for:

| Package | Version | Purpose |
|---|---|---|
| `Microsoft.NETCore.UniversalWindowsPlatform` | 6.2.14 | UWP runtime |
| `Microsoft.UI.Xaml` | 2.8.2 | WinUI 2 controls |
| `Microsoft.Toolkit.Mvvm` | 7.1.2 | `ObservableObject`, `RelayCommand`, etc. |
| `Microsoft.Toolkit.Uwp.UI(.Controls\|.Animations)` | 7.1.3 | Extra UWP controls and animations |
| `Microsoft.ClearScript.V8` | 7.3.7 | V8 JS engine host |
| `Microsoft.ClearScript.V8.Native.win-{x86,x64,arm64}` | 7.3.7 | Per-arch V8 natives |
| `Microsoft.Xaml.Behaviors.Uwp.Managed` | 2.0.1 | XAML behaviors (for `FocusAction`) |
| `FuseSharp` | 1.4.0 | Fuzzy search in the script picker |
| `ColorCode.Core` | 2.0.14 | Syntax highlighting in the editor |
| `System.Text.Json` | 7.0.2 | Parsing script JSDoc metadata |

Updating: edit `<Version>` in `Woop.csproj` or use the VS NuGet Package Manager. Run `nuget restore Woop.sln` or `msbuild /restore` to pull.

## Key abstractions

### `Script` (`Woop/Models/Script.cs`)

Wraps a single user/built-in JavaScript script. Constructor parses the `/** ... **/` JSON metadata block. `Run()` lazily creates a per-script `V8ScriptEngine`, loads the `Require.js` polyfill, executes the script source (which defines a `main` function), then calls `main(execution)` with a `ScriptExecution` payload.

### `ScriptExecution` (`Woop/Models/ScriptExecution.cs`)

The contract object passed to every script. Lowercase property names match the Boop JS API. See [`agents/blam.md`](../agents/blam.md) for the field-by-field shape.

### `ScriptMetadata` (`Woop/Models/ScriptMetadata.cs`)

Holds the parsed metadata block and implements `FuseSharp.IFuseable` so the script picker can fuzzy-rank by Name (0.9) / Tags (0.6) / Description (0.2).

### `ScriptManager` (`Woop/Services/ScriptManager.cs`)

Loads all scripts on startup. Reads built-ins from `Assets/Scripts/` via `Windows.ApplicationModel.Package.Current.InstalledLocation`, plus any user-configured custom scripts folder. Failures during script load are logged via `Debug.WriteLine` and the script is silently skipped.

### `SettingsService` (`Woop/Services/SettingsService.cs`)

Persists user settings in `Windows.Storage.ApplicationData.Current.LocalSettings`.

### `MainViewModel` (`Woop/ViewModels/MainViewModel.cs`)

Top-level view model. Owns the editor state, the script picker, and the status bar.

### Custom controls

- `Woop/Views/SyntaxHighlightingRichEditBox.cs` — text editor with ColorCode-powered highlighting.
- `Woop/Views/LineNumbers.xaml(.cs)` — gutter control rendering line numbers next to the editor.
- `Woop/Views/RtfFormatter.cs` — handles RTF round-tripping for the rich edit box.

## Code signing & packaging

`Woop.csproj` references certificate thumbprint `9B8BE8375019C354A32D6EFACC0808A7003F2432`. This certificate belongs to upstream maintainer "FS Apps" / Felix Seidl. A fresh clone will **not** have this cert installed. Options:

1. **Local sideload, unsigned:** set `<AppxPackageSigningEnabled>False</AppxPackageSigningEnabled>` temporarily, or pass `/p:AppxPackageSigningEnabled=False`.
2. **Local sideload, self-signed:** generate via `New-SelfSignedCertificate -Type CodeSigningCert -Subject "CN=<your-name>"` and either install it to `Cert:\CurrentUser\My` then update the thumbprint in the csproj, or sign post-build with `signtool.exe`.
3. **Store submission:** use the Partner Center–issued publisher cert. The existing thumbprint and identity (`53621FSApps.41283D331BF23`) MUST be replaced first.

`Package.StoreAssociation.xml` is in the project but gitignored by `*.publishproj` / Store-related patterns and would be regenerated by the VS Store Association wizard against a new publisher.

## Renaming reality

The repo and README say "Blam!" but several artifacts still say "Woop":

- Solution file: `Woop.sln`
- Main project: `Woop/Woop.csproj`
- Root namespace: `Woop`
- `Package.appxmanifest` `DisplayName="Woop!"`, app description, tile short name
- Pre-build event paths

A full rename is a deliberate, scoped change (touches namespaces, manifest, Store identity, signing cert, App.xaml.cs `EntryPoint`). Don't do it incidentally — write a spec.

## Testing

Tests live in `Woop.Tests/` as a UWP Unit Test App (MSTest). See [`development-standards.md`](../development-standards.md) for the TDD cycle. Caveats specific to UWP test hosting are documented there.

## Out of scope for this reference

- Boop's own JS script docs — see [`IvanMathy/Boop`](https://github.com/IvanMathy/Boop).
- Process / lifecycle / branch conventions — see [`development-standards.md`](../development-standards.md).
- Quickstart commands — see [`../../readme.md`](../../../readme.md).
