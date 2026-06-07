# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

# Project

A branded **Material 3** theme library for **Avalonia** 12. It gives stock Avalonia controls the CCSWE look — a Dark/Light color system, M3 type scale, motion, embedded brand fonts, and M3 control themes across the full surface: buttons (incl. toggle/split/dropdown/hyperlink + command bar + floating action button), text fields, autocomplete, numeric steppers, selection controls, lists, tree views, dropdowns, menus, expander, cards, group boxes, dividers, sliders, progress (linear + circular), tabs (tab control + tab strip), pips pager, tooltips, notifications, the date family (calendar, date/time pickers), page shells (content/tabbed/carousel/navigation), and a navigation drawer + rail (`DrawerPage`) — so consuming apps get consistent branding by referencing the package.

The library is .NET 10 / C# targeting `net10.0`, built against **Avalonia 12**, distributed as a NuGet package (`CCSWE.Avalonia.Material`). It is the desktop sibling of the CCSWE web and Android bundles: all three consume the same shared cross-platform design tokens.

It is a **standalone, no-base theme**: it depends only on **Avalonia core** (no `FluentTheme`/`SimpleTheme` base) and supplies the whole control surface itself. Consumers add a single element to `App.axaml`:

```xml
<Application xmlns:theme="using:CCSWE.Avalonia.Material">
  <Application.Styles>
    <theme:MaterialTheme />
  </Application.Styles>
</Application>
```

`MaterialTheme` is a `Styles` subclass (`MaterialTheme.axaml` + `.axaml.cs`). Dark is the default `ThemeVariant`; select via `Application.RequestedThemeVariant`. See `docs/samples/App.sample.axaml` and the root `README.md`.

## North star: Material 3 / Android fidelity

Everything we implement should match **Material 3 / Android** as closely as possible — M3 is an already-known pattern, so consumers have nothing new to learn. Use M3 component anatomy + naming; reference the **global M3 roles directly** (container = `Primary`, label = `OnPrimary`, …) rather than inventing per-control keys; apply M3 primitives (state layers 8/10/12%, the shape scale, motion) even to Avalonia-only controls that have no M3 equivalent. This also keeps desktop consistent with the shared web + Android bundles.

## The emit-vs-own contract (important)

The shared artifact across the CCSWE web/Android/desktop design systems is the **tokens**, not components — each platform themes its own framework-native components (Android themes Google's M3 library; this library *is* the Avalonia M3 component theme). So:

- **The design system emits ONLY the token layer** — `Tokens.axaml`, `Typography.axaml`, `Motion.axaml`, `Fonts.axaml`. **Treat these as consume-verbatim: regenerate from `tokens/` rather than hand-editing.**
- **The library owns everything else** — all control themes (`Controls/*.axaml`), the `MaterialTheme` entry, the `Base/*` infrastructure layer, the embedded font bytes, packaging, and the small number of **custom control types** (e.g. `Card.cs`) added only where Avalonia ships no stock control.

Litmus test: a pure function of the shared tokens → the DS emits it; an Avalonia control template, framework wiring, or a custom control type → the library owns it.

### The `Base/` interim layer + hybrid migration

`Base/*` is the control base **forked once from `Avalonia.Themes.Simple` 12.0.4** — the structural infra (Window, popups, SplitView, overlay/adorner hosts, the date-time spinner pickers, …) recolored to M3, plus the `Base/BaseAliases.axaml` / `SimplePalette.axaml` / `Strings.axaml` shims. These are **interim scaffolding**: each control is hand-rolled to real M3 (referencing our tokens directly) over time and moved to `Controls/`; the shims shrink as forks fall away. **Status:** all styleable + page-shell controls are now hand-rolled M3 in `Controls/`; what remains in `Base/` is the recolored structural infra + shims, and no control theme references the Simple `Theme*` palette anymore. Folder rule: a from-scratch M3 `ControlTheme` lives in `Controls/`; a recolored-but-still-Simple-skeleton fork stays in `Base/` (see `src/CCSWE.Avalonia.Material/Base/README.md`). `MaterialTheme.axaml` merges everything via **`ResourceInclude`** (nested, last-wins) — never `MergeResourceInclude` (it flattens and throws on duplicate keys).

The token JSON source-of-truth lives in `tokens/`; the design-system handoff docs live in `docs/design-system/`. Consumer→DS feedback (round-trip notes) lives in `eng/ds-feedback/`, which is **gitignored** — internal, not published.

## Build & Pack Commands

Projects live under `src/`; the solution is `src/CCSWE.Avalonia.Material.slnx`.

```bash
# Build everything (library + Demo)
dotnet build src/CCSWE.Avalonia.Material.slnx --configuration Release

# Run the Demo gallery (visual verification harness — Dark/Light toggle)
# To drive + screenshot the running demo under WSLg (agent-driven: launch, click/scroll via
# xdotool, capture with import) for runtime/visual checks a `dotnet build` can't catch,
# see docs/wslg-gui-debugging.md.
dotnet run --project src/CCSWE.Avalonia.Material.Demo

# Pack the NuGet package (library only)
dotnet pack src/CCSWE.Avalonia.Material/CCSWE.Avalonia.Material.csproj --configuration Release

# Run all tests (once a test project is added under tests/)
dotnet test src/CCSWE.Avalonia.Material.slnx

# Run a specific test
dotnet test src/CCSWE.Avalonia.Material.slnx --filter "FullyQualifiedName~ClassName"
```

The SDK is pinned to `10.0.0` (`rollForward: latestMinor`) via the root `global.json`. `src/Directory.Build.props` applies `LangVersion=preview`, `ImplicitUsings=enable`, and `Nullable=enable` solution-wide, and references JetBrains.Annotations and Nerdbank.GitVersioning (version derived from git history — base `12.0` in the root `version.json`).

## Package management

Package versions are centrally managed via **Central Package Management** (`src/Directory.Packages.props`, `ManagePackageVersionsCentrally=true`). To add or update a dependency, add a `<PackageVersion Include="X" Version="N" />` in `Directory.Packages.props` and a version-less `<PackageReference Include="X" />` in the project (or `Directory.Build.props` for solution-wide refs). Never put `Version=` on a `<PackageReference>` — it errors with NU1008.

Keep the Avalonia package versions (`Avalonia`, `Avalonia.Desktop`, etc.) in lockstep — they should all share the same version. The library depends only on **`Avalonia` core** — no `Avalonia.Themes.Fluent`/`Simple` base theme.

## Publishing

The library publishes to **NuGet.org** as `CCSWE.Avalonia.Material`. Versioning is **Nerdbank.GitVersioning** (root `version.json`, base `12.0` → `12.0.x`). **The major version tracks the supported Avalonia major** (12.x → Avalonia 12.x; bump to 13.x when retargeting Avalonia 13) — this mirrors the Semi.Avalonia convention and reflects the library's tight coupling to Avalonia's control templates. Minor/patch are the library's own (features/fixes), **not** Avalonia's minor/patch. Shared package metadata lives in `src/Directory.Build.props`; per-package metadata (description, tags, README) in the library csproj. The Demo sets `IsPackable=false`. SourceLink + `snupkg` symbols are enabled.

CI (`.github/workflows/dotnet-build-publish-library.yml`, "Build, test, and publish"): pushes to **`master`** build + test + pack + **publish to NuGet.org** (via the `NUGET_API_KEY` secret); PRs build + test only. NuGet versions are immutable, so every `master` push is an immutable public release. There is no committed `nuget.config` with credentials. Two more GitHub-native checks run (both free on this public repo): **CodeQL** code scanning (`.github/workflows/codeql.yml`, buildless C#, on push/PR + weekly) and **Dependabot** version updates (`.github/dependabot.yml`, NuGet via CPM with Avalonia grouped + GitHub Actions, weekly — active once on `master`, since Dependabot reads config from the default branch).

The package embeds a self-contained NuGet README (`src/CCSWE.Avalonia.Material/README.md`, distinct from the repo root `README.md`, which uses repo-relative links) and the fonts' `OFL.txt` under `THIRD-PARTY-NOTICES/`.

## Architecture

Projects in `src/CCSWE.Avalonia.Material.slnx`:

- **`CCSWE.Avalonia.Material`** — the theme class library (NuGet package). Depends only on Avalonia core. The axaml live **flat at the project root**, grouped by the `Controls/` (M3 control themes) and `Base/` (interim infra forks) subfolders, alongside a small number of custom control `*.cs` types at the root:
  - `MaterialTheme.axaml` (+ `MaterialTheme.axaml.cs`) — the `Styles` subclass consumers instantiate as `<theme:MaterialTheme/>`; merges tokens + `Base/*` + `Controls/*` (layer order documented in the file header).
  - `Card.cs` — the library's first **custom control type**: `Card` (an M3 card surface; `Card : ContentControl` with hand-rolled `Command`/`CommandParameter`/`Click`, a nullable `IsClickable` that derives from `Command`, and `:clickable`-gated hover/press state layers). Its `ControlTheme` lives in `Controls/Card.axaml` (which also keeps the `Border.Card` class convention for static surfaces). The library themes stock Avalonia controls by default; a custom type is added only where Avalonia ships no equivalent. *[library-owned]*
  - `Tokens.axaml` — Dark/Light color roles (as `ResourceDictionary.ThemeDictionaries`) + theme-invariant metrics (`CornerRadius*`, `Spacing*`, the M3 `FontSize*` scale). *[DS-emitted]*
  - `Fonts.axaml`, `Motion.axaml`, `Typography.axaml` *[DS-emitted]*; `Controls/*.axaml` — the hand-authored M3 control themes (~49; all styleable + page-shell controls now live here). *[library-owned]*
  - `Base/*.axaml` — the interim structural-infra forks (Window/popups/SplitView/overlay hosts + the date-time spinner pickers, all M3-recolored) + the `BaseAliases`/`SimplePalette`/`Strings` shims. *[library-owned; shrinking — see `Base/README.md`]*
  - `Assets/Fonts/` — embedded OFL variable TTFs (DM Sans, Plus Jakarta Sans), referenced by family name from `Fonts.axaml`.
- **`CCSWE.Avalonia.Material.Demo`** — an Avalonia desktop app that wires the theme (`<theme:MaterialTheme/>`) and renders a control gallery with a Dark/Light toggle. It is the **visual verification harness**; keep it in sync when adding controls.

Conventions when working on the theme (full detail in `docs/design-system/CONVENTIONS.md`):

- **Resource naming:** each color role emits a paired `SolidColorBrush` (bare PascalCase, e.g. `Primary`) and `Color` (role + `Color` suffix, e.g. `PrimaryColor`). Reach for the bare brush name in markup. Reference global M3 roles directly in control themes — no per-control key vocabulary.
- **`DynamicResource` inside `ControlTheme`s — always**, for color/metric/motion refs. `StaticResource` freezes a brush at parse time so the control won't repaint on a `ThemeVariant` flip (a real bug). `StaticResource` is only for same-file structural refs (`BasedOn=`, `Theme=` assignments).
- **Control themes** are full templates (no base theme to `BasedOn`), honoring required Avalonia part names (`PART_*`) and the standard pseudo-classes (`:checked`, `:error`, `:focus-within`, …). `ControlTheme`s must be declared inside `<Styles.Resources>`.
- **Custom control types** (only where Avalonia ships no stock control, e.g. `Card`) derive from the closest semantic base — `ContentControl` for containers, **not** `Button` — and hand-roll any interactivity (`Command`/`Click`, `:clickable`/`:pressed` pseudo-classes) faithfully ported from the matching Avalonia control. Set `ClipToBounds="False"` when a template-root `Border` paints a `BoxShadow` (otherwise the shadow is clipped to bounds). In Dark mode shadows are near-invisible, so convey elevation/hover-raise with a surface-tone step too.
- Files are `AvaloniaResource` (auto-globbed for axaml; fonts included explicitly in the csproj) and resolve via `avares://CCSWE.Avalonia.Material/...` URIs.
- **Include ordering in `MaterialTheme.axaml`:** group by folder (root token files → `Base/*` → Typography → `Controls/*`) and alphabetize within each group, **except** where order is load-bearing — keep such exceptions together with a comment. Current exceptions: the `Base/` shims `SimplePalette` → `Strings` → `BaseAliases` (BaseAliases overrides SimplePalette keys via last-wins, so it stays last) and the whole `Controls/*` group applying after `Base/*`. (This mirrors the alphabetize-within-group rule in Coding Standards.)

## Reference implementations (pull the source — don't guess)

For implementation questions about Avalonia theming, study the real source rather than reverse-engineering by trial:

- **Avalonia 12.0.4** — https://github.com/AvaloniaUI/Avalonia (tag `12.0.4`). `src/Avalonia.Themes.Simple` is the source our `Base/*` forks come from; `src/Avalonia.Themes.Fluent` is the reference for richer templates/animation and for control-default mappings.
- **Semi.Avalonia** — https://github.com/irihitech/Semi.Avalonia — a complete **standalone** Avalonia theme (a `Styles` subclass + `ThemeDictionaries` + a `Controls/_index.axaml` aggregator; no Fluent/Simple dependency). The model this library's architecture mirrors.

Tests belong under the solution's `tests/` folder (no test project exists yet — see Testing).

# Testing

Tests use **NUnit 4**. (No test project exists yet; follow these conventions when adding one under `tests/`.)

## Class organization

- Outer class name: `<ClassUnderTest>Tests`, decorated with `[SuppressMessage("ReSharper", "InconsistentNaming")]`. The outer class is **not** `sealed` (nested classes inherit from it).
- Nested classes group tests by method or scenario: `When_<MethodName>_Is_Called`, **inheriting the outer class**.
- Test methods describe expected behavior: `It_<expected_behavior>` (e.g., `It_Adds_UserAgent_Header`)

```csharp
public class SomeServiceTests
{
    public class When_GetAsync_Is_Called : SomeServiceTests
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
- PascalCase: classes, methods, properties, constants, namespaces, public fields, record primary constructor parameters
- camelCase: local variables, parameters
- `_camelCase`: private fields (underscore prefix)
- `I` prefix for interfaces (e.g. `IThemeProvider`)
- Two-character acronyms are uppercase (`IO`, `UI`); longer acronyms use PascalCase (`Http`, `Json`)

**Formatting**
- 4-space indentation (no tabs)
- Allman brace style — opening and closing braces on their own lines
- Always use curly braces for control flow (`if`, `for`, `foreach`, `while`, etc.) — never omit them for single-line bodies
- One statement per line; one declaration per line
- One blank line between members; no consecutive blank lines
- Space after flow-control keywords (`if (`, `for (`); no space after method names (`Method(`)

**File organization**
- One type per file, named `{TypeName}.cs`; partial classes use `{ClassName}.{Part}.cs`
- File-scoped namespaces (`namespace Foo;`), aligned with folder structure
- `using` directives outside namespace declarations; order `System` first, then third-party, then project namespaces

**Access modifiers**
- Always explicit (no implicit `private`/`internal`)
- `[PublicAPI]` (JetBrains.Annotations, referenced from `Directory.Build.props`) on intentional public API surface; `internal` for implementation details
- `[ExcludeFromCodeCoverage]` on composition-only types (bootstrappers and similar)

**Language style**
- Use `var` for all local variables where the type can be inferred
- Use language keywords for built-in types (`string`, `int`, not `String`, `Int32`)
- Prefer string interpolation (`$"..."`) over concatenation
- Use `&&`/`||`, not `&`/`|`, for logical comparisons
- Use `async`/`await` for async code; avoid `.Result` or `.Wait()`
- Prefer expression-bodied members for single-line getters/methods
- Nullable reference types are enabled solution-wide (`Directory.Build.props`); respect nullability annotations
- Use `nameof()` instead of string literals for member/property names

**Class member order**

Group members in this order, alphabetized by name within each group **regardless of access modifier** (`CreateFoo()` before `GetFoo()` whether public or private); nested types go at the bottom of the file:

1. Constants / `static readonly` fields
2. Instance fields
3. Constructors
4. Properties
5. Methods

**Frozen collections for static lookups**
- Any `static readonly` `HashSet<T>` / `Dictionary<TKey,TValue>` never mutated after construction should be a `FrozenSet<T>` / `FrozenDictionary<TKey,TValue>` (`System.Collections.Frozen`), built via `.ToFrozenSet(comparer)` / `.ToFrozenDictionary()` — faster lookups, and the type signals "immutable lookup".
