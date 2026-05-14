# Woop.Tests

UWP MSTest unit-test project for [Blam!](../readme.md). For the TDD workflow this project supports, see [`docs/documentation/development-standards.md`](../docs/documentation/development-standards.md).

## First-time setup

This project was scaffolded by hand rather than through the Visual Studio "Unit Test App (Universal Windows)" template. On first open in VS you may need to:

1. **Restore NuGet packages.** `MSTest.TestAdapter` and `MSTest.TestFramework` are referenced; right-click solution → *Restore NuGet Packages*, or run `nuget restore Woop.sln`.
2. **Verify the visual assets resolve.** The manifest references `Assets\StoreLogo.png`, `Assets\Square150x150Logo.png`, `Assets\Square44x44Logo.png`, and `Assets\SplashScreen.png`. These are linked from `..\Woop\Assets\*.scale-200.png` so the test app reuses the main app's icons. If a build error complains about a missing asset, regenerate via the manifest designer's *Visual Assets* tab or copy a placeholder in.
3. **Build.** First build will create the test app package; the `Test Explorer` should then discover `Woop.Tests.UnitTest1.TestHarness_Works`.
4. **Run.** Right-click in Test Explorer → *Run*. The test app launches into a UWP host (this is slower than a console runner — expect a few seconds on first run).

## Adding tests

```csharp
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Woop.Tests
{
    [TestClass]
    public class MyFeatureTests
    {
        [TestMethod]
        public void Behavior_Description()
        {
            // Arrange
            // Act
            // Assert
        }
    }
}
```

Internal types in `Woop` are visible to this project via `InternalsVisibleTo("Woop.Tests")` in `Woop/Properties/AssemblyInfo.cs`. Prefer testing public surface where possible; reach for internals only when public APIs don't expose enough.

## When this project's limitations get in your way

UWP test hosting is slow and entangles tests with `Windows.*` APIs. Pure-logic tests (JSDoc metadata parsing, fuzzy ranking, etc.) would run faster in a `.NET Standard` library. That refactor is out of scope for the initial DDD/TDD adoption — propose it in an ADR if and when it becomes painful enough to be worth the churn.
