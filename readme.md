# Blam!
[![Downloads](https://img.shields.io/github/downloads/felixse/Woop/total.svg?label=Downloads)](https://github.com/felixse/Woop/releases/)
[![Release](https://img.shields.io/github/release/felixse/Woop.svg?label=Release)](https://github.com/felixse/Woop/releases)

<img width="320" height="320" alt="coffey codes_halftone_stipple_shaded_comic_book_onomatopoeia__20ca787f-7b69-44c1-bd0e-409e4a79aee2_2" src="https://github.com/user-attachments/assets/ea7cc9e7-b166-4eff-9692-aaeef25dd573" />

---

**Blam!** is a scriptable, AI-powered scratchpad that allows you to run any AI powered transformations on your text using the built-in or your self-written .js scripts.

---

Models Supported:
- Claude
- Gemini
- OpenAI


> A Windows port of [Boop](https://boop.okat.best/) and a fork of [Woop](https://woop.okat.best/)

---

## What is it?

Type or paste text into the editor. Press **Ctrl+B**. A fuzzy-searchable script picker appears. Choose a script — sort lines, parse JSON, hit an LLM, generate a UUID, whatever — and Enter to run it. The script transforms your text (or just your selection) via an embedded V8 JavaScript engine and writes the result back into the editor.

The script library is shared with [Boop](https://github.com/IvanMathy/Boop) (the macOS original), so anything that works there works here. User-written scripts live in a configurable folder and load at startup.

## Tech stack

| Aspect | Value |
|---|---|
| Language | C# 8.0 |
| Framework | UWP (`Microsoft.NETCore.UniversalWindowsPlatform` 6.2.14) |
| UI | `Microsoft.UI.Xaml` 2.8.2 (WinUI 2), MVVM via `Microsoft.Toolkit.Mvvm` 7.1.2 |
| Scripting engine | `Microsoft.ClearScript.V8` 7.3.7 |
| Other libs | `FuseSharp` (fuzzy search), `ColorCode.Core` (syntax highlighting), `System.Text.Json` |
| Test framework | MSTest (UWP Unit Test App) |
| Min Windows | 10.0.18362 (1903) |
| Target SDK | 10.0.19041.0 |
| Platforms | x86, x64, ARM, ARM64 |

## Prerequisites

- **Windows 10** (build 18362) or later.
- **Visual Studio 2019 or 2022** with the *Universal Windows Platform development* workload and the Windows 10 SDK 10.0.19041.0.
- **Developer Mode** enabled (Settings → Privacy & Security → For developers).
- **Git** with submodule support.

A code-signing certificate is **not** required for local development. It only matters for Microsoft Store submission or signed sideload distribution.

## Quickstart

```powershell
git clone --recurse-submodules <your-fork-url> blam
cd blam
start Blam.sln    # then press F5 in Visual Studio
```

The repo uses a git submodule (`submodules/Boop`) whose contents are xcopied into `Blam/Assets/Scripts/` by a pre-build event. **Without the submodule the build fails.** If you cloned without `--recurse-submodules`, run:

```powershell
git submodule update --init --recursive
```

## Running the dev build

**In Visual Studio:** open `Blam.sln`, set `Blam` as the startup project, pick a platform (`x64` recommended on modern hardware), press **F5**.

**Command line:**

```powershell
msbuild Blam.sln /t:Restore /p:Configuration=Debug /p:Platform=x64
msbuild Blam.sln /t:Build   /p:Configuration=Debug /p:Platform=x64
```

The `/t:Restore` step pulls NuGet packages; you can replace both calls with `msbuild Blam.sln /p:Configuration=Debug /p:Platform=x64 /restore`.

To deploy the built app to your local machine without launching VS, use the *Deploy* command in VS or `msbuild /t:Deploy`. Or build a sideloadable package per the next section.

## Building executables

UWP apps ship as `.msix` (or legacy `.appx`) packages, optionally bundled across architectures into `.msixbundle`. Three audiences, three flows.

### A. Local sideload `.msix`

For private testing on your own machine or a colleague's:

```powershell
msbuild Blam.sln /restore `
  /p:Configuration=Release `
  /p:Platform=x64 `
  /p:AppxBundle=Always `
  /p:AppxBundlePlatforms="x86|x64|arm64" `
  /p:UapAppxPackageBuildMode=SideloadOnly `
  /p:AppxPackageSigningEnabled=False
```

(`AppxPackageSigningEnabled=False` skips signing — see *Code signing* below if you want a signed sideload package.)

Output lands in `Blam/AppPackages/`. Install with:

```powershell
Add-AppxPackage -Path .\Blam\AppPackages\<bundle-folder>\<bundle-name>.msixbundle
```

To uninstall:

```powershell
Get-AppxPackage *Blam* | Remove-AppxPackage
```

#### Code signing for sideload

Sideload installs require either an unsigned dev build (works only with Developer Mode on) or a signed package whose certificate is trusted by the target machine. To self-sign:

```powershell
$cert = New-SelfSignedCertificate `
  -Type CodeSigningCert `
  -Subject "CN=Blam Dev" `
  -KeyUsage DigitalSignature `
  -CertStoreLocation "Cert:\CurrentUser\My" `
  -TextExtension @("2.5.29.37={text}1.3.6.1.5.5.7.3.3", "2.5.29.19={text}")
$cert.Thumbprint   # paste into Blam.csproj <PackageCertificateThumbprint>
```

Then run the same `msbuild` command without `AppxPackageSigningEnabled=False`. To install the resulting signed package on another machine, also install the cert into `Cert:\LocalMachine\TrustedPeople`.

### B. Microsoft Store submission

The `Blam.csproj` and `Blam/Package.appxmanifest` currently reference the upstream maintainer's Store identity (`53621FSApps.41283D331BF23`) and certificate thumbprint (`9B8BE8375019C354A32D6EFACC0808A7003F2432`). **Replace both** before submitting from your own account.

1. In [Partner Center](https://partner.microsoft.com/dashboard/windows/), reserve an app name and copy the Store-assigned `Identity Name` and `Publisher`.
2. In Visual Studio, right-click `Blam` → *Publish* → *Associate App with the Store* — the wizard rewrites `Package.appxmanifest` and generates `Package.StoreAssociation.xml`.
3. Replace `PackageCertificateThumbprint` in `Blam.csproj` with your Partner Center–issued publisher cert thumbprint.
4. Build the Store bundle:

   ```powershell
   msbuild Blam.sln /restore `
     /p:Configuration=Release `
     /p:Platform=x64 `
     /p:AppxBundle=Always `
     /p:AppxBundlePlatforms="x86|x64|arm64" `
     /p:UapAppxPackageBuildMode=StoreUpload
   ```

5. Upload the resulting `.msixupload` from `Blam/AppPackages/` via Partner Center.

### C. GitHub Releases

For distribution outside the Store. Same as the sideload build (signed), then:

1. Bundle the artifacts:
   - `Blam_<version>_x86_x64_arm64.msixbundle`
   - The `.cer` corresponding to the signing cert (consumers will install it into `Cert:\LocalMachine\TrustedPeople`)
   - A short `INSTALL.md` with the `Add-AppxPackage` command
2. Tag and push: `git tag v1.x.y && git push origin v1.x.y`
3. `gh release create v1.x.y --notes-file CHANGELOG-v1.x.y.md ./Blam/AppPackages/<bundle>.msixbundle ./certs/<cert>.cer ./INSTALL.md`

> **Heads-up about the badges at the top of this README.** They currently point at upstream `felixse/Woop`. Once your fork has its first release, update them to your repo:
>
> ```
> [![Downloads](https://img.shields.io/github/downloads/<your-user>/<your-repo>/total.svg?label=Downloads)](https://github.com/<your-user>/<your-repo>/releases/)
> [![Release](https://img.shields.io/github/release/<your-user>/<your-repo>.svg?label=Release)](https://github.com/<your-user>/<your-repo>/releases)
> ```

## Managing dependencies

NuGet via `PackageReference` (see `Blam/Blam.csproj`). To add or update:

```powershell
# Restore (after clone or after editing PackageReferences):
msbuild Blam.sln /t:Restore
# or:
nuget restore Blam.sln

# Update a specific package — easiest via Visual Studio's
#   Solution → Manage NuGet Packages for Solution → Updates
# Or edit the <Version> directly in Blam.csproj.
```

The Boop script library is **not** a NuGet dependency — it's the `submodules/Boop` git submodule. To pull upstream script updates:

```powershell
git submodule update --remote submodules/Boop
git add submodules/Boop
git commit -m "chore: bump Boop submodule"
```

## Running tests

Tests live in `Blam.Tests/` as a UWP Unit Test App (MSTest). See [`Blam.Tests/README.md`](Blam.Tests/README.md) for first-time setup and [`docs/documentation/development-standards.md`](docs/documentation/development-standards.md) for the TDD workflow.

```powershell
# In Visual Studio: Test → Test Explorer → Run All
# Or from the command line:
msbuild Blam.sln /t:Restore /p:Configuration=Debug /p:Platform=x64
msbuild Blam.sln /t:Build   /p:Configuration=Debug /p:Platform=x64
vstest.console.exe Blam.Tests\bin\x64\Debug\Blam.Tests.build.appxrecipe
```

UWP tests run inside a UWP app host — slower to start than a console runner. This is normal.

## Project layout

```
.
├── Blam/                 Main UWP application
├── Blam.Tests/           MSTest UWP unit-test project
├── Blam.sln              Visual Studio solution
├── submodules/Boop/      Upstream script library (git submodule)
├── icons/                Source assets for app icons
├── Screenshots/          Marketing screenshots
├── docs/                 Specs, ADRs, agent briefs, dev standards
├── .claude/commands/     Slash commands (/new-spec, /new-bug, /new-adr, /new-agent-brief)
├── CONTRIBUTING.md
└── LICENSE               GPLv3
```

Folder-by-folder code tour: [`docs/documentation/repos/blam.md`](docs/documentation/repos/blam.md).

## Writing scripts

Built-in scripts come from [Boop](https://github.com/IvanMathy/Boop) — contribute general-purpose scripts there. Personal scripts go in your custom scripts folder (set in *Settings*).

Script API and metadata format: [`docs/documentation/agents/blam.md`](docs/documentation/agents/blam.md).

## Contributing

Read [`CONTRIBUTING.md`](CONTRIBUTING.md) first — it covers the submodule trap, code-signing reality, the DDD/TDD workflow, branch/commit conventions, and the PR checklist. The longer rationale lives in [`docs/README.md`](docs/README.md).

## License

[GPLv3](LICENSE). Contributions are accepted under the same license.

---

> Forked from [Woop](https://github.com/felixse/Woop) (a Windows port of [Boop](https://boop.okat.best/) by Ivan Mathy).
