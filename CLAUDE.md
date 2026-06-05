# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

# Project

A branded **Material 3** theme library for **Avalonia** 12. It gives stock Avalonia controls the CCSWE look — a Dark/Light color system, M3 type scale, motion, embedded brand fonts, and M3 control themes for buttons (incl. toggle buttons), text fields, autocomplete, numeric steppers, selection controls, lists, tree views, dropdowns, menus, expander, cards, sliders, progress, tabs (tab control + tab strip), and a navigation drawer (`DrawerPage`) — so consuming apps get consistent branding by referencing the package.

The library is .NET 10 / C# targeting `net10.0`, built against **Avalonia 12**, distributed as a NuGet package (`CCSWE.Avalonia.Material`). It is the desktop sibling of the CCSWE web and Android bundles: all three consume the same shared cross-platform design tokens.

Consumers wire it into `App.axaml` with `<FluentTheme />` + `Theme.axaml` in `Application.Styles`, and `FluentOverrides.axaml` in `Application.Resources` (see `docs/samples/App.sample.axaml` and the root `README.md`). Dark is the default `ThemeVariant`.

## The emit-vs-own contract (important)

Most of the theme is **emitted from the shared tokens** by the CCSWE design system — `Theme.axaml`, `Tokens.axaml`, `Fonts.axaml`, `Motion.axaml`, `Typography.axaml`, and `Controls/*.axaml`. **Treat these as consume-verbatim: regenerate them from `tokens/` rather than hand-editing.** The litmus test: if it's a pure function of the tokens, the design system emits it; if it depends on Avalonia framework wiring or app context, the library owns it.

The library **hand-authors** only the glue the tokens can't express: `FluentOverrides.axaml` (the FluentTheme accent remap), the embedded font bytes under `Assets/Fonts/`, and packaging.

The token JSON source-of-truth lives in `tokens/`; the design-system handoff docs live in `docs/design-system/`. The consumer→DS feedback (round-trip notes) lives in `eng/ds-feedback/`, which is **gitignored** — internal, not published. When a regenerated bundle lands, re-apply the layout/naming the DS feedback asks for (flat root layout, `Theme.axaml` name).

## Build & Pack Commands

Projects live under `src/`; the solution is `src/CCSWE.Avalonia.Material.slnx`.

```bash
# Build everything (library + Demo)
dotnet build src/CCSWE.Avalonia.Material.slnx --configuration Release

# Run the Demo gallery (visual verification harness — Dark/Light toggle)
dotnet run --project src/CCSWE.Avalonia.Material.Demo

# Pack the NuGet package (library only)
dotnet pack src/CCSWE.Avalonia.Material/CCSWE.Avalonia.Material.csproj --configuration Release

# Run all tests (once a test project is added under tests/)
dotnet test src/CCSWE.Avalonia.Material.slnx

# Run a specific test
dotnet test src/CCSWE.Avalonia.Material.slnx --filter "FullyQualifiedName~ClassName"
```

The SDK is pinned to `10.0.0` (`rollForward: latestMinor`) via the root `global.json`. `src/Directory.Build.props` applies `LangVersion=preview`, `ImplicitUsings=enable`, and `Nullable=enable` solution-wide, and references JetBrains.Annotations and Nerdbank.GitVersioning (version derived from git history — base `0.1` in the root `version.json`).

## Package management

Package versions are centrally managed via **Central Package Management** (`src/Directory.Packages.props`, `ManagePackageVersionsCentrally=true`). To add or update a dependency, add a `<PackageVersion Include="X" Version="N" />` in `Directory.Packages.props` and a version-less `<PackageReference Include="X" />` in the project (or `Directory.Build.props` for solution-wide refs). Never put `Version=` on a `<PackageReference>` — it errors with NU1008.

Keep the Avalonia package versions (`Avalonia`, `Avalonia.Themes.Fluent`, etc.) in lockstep — they should all share the same version.

## Publishing

The library publishes to **NuGet.org** as `CCSWE.Avalonia.Material`. Versioning is **Nerdbank.GitVersioning** (root `version.json`, base `0.1` → `0.1.x`). Shared package metadata lives in `src/Directory.Build.props`; per-package metadata (description, tags, README) in the library csproj. The Demo sets `IsPackable=false`. SourceLink + `snupkg` symbols are enabled.

CI (`.github/workflows/dotnet-build-publish-library.yml`): pushes to **`master`** build + test + pack + **publish to NuGet.org** (via the `NUGET_API_KEY` secret); PRs build + test only. NuGet versions are immutable, so every `master` push is an immutable public release. There is no committed `nuget.config` with credentials.

The package embeds a self-contained NuGet README (`src/CCSWE.Avalonia.Material/README.md`, distinct from the repo root `README.md`, which uses repo-relative links) and the fonts' `OFL.txt` under `THIRD-PARTY-NOTICES/`.

## Architecture

Projects in `src/CCSWE.Avalonia.Material.slnx`:

- **`CCSWE.Avalonia.Material`** — the theme class library (NuGet package). Pure library, no front-end dependency. The axaml live **flat at the project root** (no `Themes/` folder), grouped only by the `Controls/` subfolder:
  - `Theme.axaml` — the one-stop include consumers add to `App.axaml`; merges the resource dictionaries (`Fonts`, `Tokens`, `Motion`) and `StyleInclude`s the style files (`Typography`, `Controls/*`).
  - `Tokens.axaml` — Dark/Light color roles (as `ResourceDictionary.ThemeDictionaries`) + theme-invariant metrics (`CornerRadius*`, `Spacing*`).
  - `Fonts.axaml`, `Motion.axaml`, `Typography.axaml`, `Controls/{Button,ToggleButton,TextBox,AutoCompleteBox,NumericUpDown,CheckBox,RadioButton,ToggleSwitch,ListBox,TreeView,ComboBox,Menu,Expander,Card,Slider,ProgressBar,TabControl,TabStrip,DrawerPage}.axaml` (19 control files).
  - `FluentOverrides.axaml` — hand-authored accent remap (lives at the root, not in a `library-glue/` folder).
  - `Assets/Fonts/` — embedded OFL variable TTFs (DM Sans, Plus Jakarta Sans), referenced by family name from `Fonts.axaml`.
- **`CCSWE.Avalonia.Material.Demo`** — an Avalonia desktop app that wires the theme and renders a control gallery with a Dark/Light toggle. It is the **visual verification harness**; keep it in sync when adding controls to the theme.

Conventions when working on the theme (full detail in `docs/design-system/CONVENTIONS.md`):

- **Resource naming:** each color role emits a paired `SolidColorBrush` (bare PascalCase, e.g. `Primary` — the common case) and `Color` (role + `Color` suffix, e.g. `PrimaryColor`). Reach for the bare brush name in markup.
- **`DynamicResource` inside `ControlTheme`s — always**, for color/metric/motion refs. `StaticResource` freezes a brush at parse time so the control won't repaint on a `ThemeVariant` flip (a real bug). `StaticResource` is only for same-file structural refs (`BasedOn=`, the `Theme=` convenience assignments).
- **Control themes** are full templates (they don't `BasedOn` Fluent), honoring required Avalonia part names (`PART_*`) and the standard pseudo-classes (`:checked`, `:error`, `:focus-within`, …). `ControlTheme`s must be declared inside `<Styles.Resources>`.
- Files are `AvaloniaResource` (auto-globbed for axaml; fonts are included explicitly in the csproj) and resolve via `avares://CCSWE.Avalonia.Material/...` URIs.

Tests belong under the solution's `tests/` folder (no test project exists yet — see Testing).

# Testing

Tests use **NUnit 4**. (No test project exists yet; follow these conventions when adding one under `tests/`.)

## Class organization

- Outer class name: `<ClassUnderTest>Tests`, decorated with `[SuppressMessage("ReSharper", "InconsistentNaming")]`
- Nested classes group tests by method or scenario: `When_<MethodName>_Is_Called`
- Test methods describe expected behavior: `It_<expected_behavior>` (e.g., `It_Adds_UserAgent_Header`)

```csharp
public class SomeServiceTests
{
    public class When_GetAsync_Is_Called
    {
        [Test]
        public async Task It_returns_expected_result() { ... }
    }
}
```

## Arrange-Act-Assert

Follow the AAA pattern. Use blank lines to separate sections — do **not** use `// Arrange`, `// Act`, `// Assert` comments.

## Mocking

- Use **Moq** for mocking
- `ILogger` should be mocked using the `LoggerFake` class, not `new Mock<ILogger>()`
- Prefer `ReturnsAsync(...)` and `ThrowsAsync(...)` over manually setting up async mock methods

# Coding Standards

Follow standard C# conventions ([source](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions)).

**Naming**
- PascalCase: classes, methods, properties, constants, namespaces, record primary constructor parameters
- camelCase: local variables, parameters
- `_camelCase`: private fields (underscore prefix)

**Formatting**
- 4-space indentation (no tabs)
- Allman brace style — opening and closing braces on their own lines
- Always use curly braces for control flow (`if`, `for`, `foreach`, `while`, etc.) — never omit them for single-line bodies
- One statement per line; one declaration per line

**Language style**
- Use `var` for all local variables where the type can be inferred
- Use language keywords for built-in types (`string`, `int`, not `String`, `Int32`)
- Prefer string interpolation (`$"..."`) over concatenation
- Use `&&`/`||`, not `&`/`|`, for logical comparisons
- Use `async`/`await` for async code; avoid `.Result` or `.Wait()`
- File-scoped namespaces (`namespace Foo;`)
- `using` directives outside namespace declarations
