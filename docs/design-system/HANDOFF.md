# Handoff — CCSWE.Avalonia.Material (token layer)

How a developer (or Claude Code) wires the emitted **token bundle** into the
**CCSWE.Avalonia.Material** library and a consuming desktop app.

> **v2 — tokens only.** This bundle emits `Tokens.axaml` / `Typography.axaml` /
> `Motion.axaml` / `Fonts.axaml` and the font assets, nothing else. The library is
> a **standalone Material 3 theme** (no `FluentTheme`/`SimpleTheme` base): it
> hand-authors every M3 control theme + an interim `Base/` infra layer (forked from
> `Avalonia.Themes.Simple` 12.0.4) and exposes one entry — `<theme:MaterialTheme/>`.
> The control themes, `MaterialTheme`, and `Base/` are **library-owned** and are
> not in this bundle. North star: **Material 3 / Android fidelity**.

## Scope

A **tokens** bundle — color schemes, metrics (shape + spacing + type sizes), the
type-scale classes, motion values, and font references. It does **not** ship
control themes, custom controls, a base theme, or app plumbing. The library
consumes these resources to drive its own hand-authored M3 component themes.

## Library project layout

The four emitted files drop in at the library root verbatim; everything else in
the library is hand-authored.

```
CCSWE.Avalonia.Material/            (Avalonia 12 class library; TFM-agnostic, net8.0+)
├── CCSWE.Avalonia.Material.csproj
│
│   ── emitted by the DS — drop in verbatim, never hand-edit ──
├── Tokens.axaml                 ThemeDictionaries Dark/Light + metrics + FontSize scale
├── Fonts.axaml
├── Motion.axaml
├── Typography.axaml
├── Assets/Fonts/                run fetch-fonts.{sh,ps1} (see FONTS.md)
│   ├── fetch-fonts.sh / .ps1
│   ├── PlusJakartaSans[wght].ttf  + PlusJakartaSans-OFL.txt
│   └── DMSans[opsz,wght].ttf      + DMSans-OFL.txt
│
│   ── hand-authored by the library (NOT emitted) ──
├── MaterialTheme.cs / .axaml    one-stop Styles entry — merges the 4 token files + the control themes
├── Base/                        interim infra forked from Avalonia.Themes.Simple 12.0.4
└── Controls/                    hand-authored M3 control themes (Button, TextBox, …)
```

### csproj — make the axaml + fonts into avares:// resources

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <!-- TFM-agnostic: net8.0+ all build fine (consumer ships on net10.0). -->
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <AvaloniaUseCompiledBindingsByDefault>true</AvaloniaUseCompiledBindingsByDefault>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Avalonia" Version="12.*" />
  </ItemGroup>
  <ItemGroup>
    <!-- .axaml are AvaloniaResource by default in an Avalonia library;
         fonts must be included explicitly. -->
    <AvaloniaResource Include="Assets/Fonts/*.ttf" />
  </ItemGroup>
</Project>
```

The `avares://CCSWE.Avalonia.Material/...` URIs in the emitted files assume the
assembly name is `CCSWE.Avalonia.Material`. If you name the assembly differently,
find-and-replace the authority segment in `Fonts.axaml` (the only emitted file
that carries an `avares://` URI).

## How the library consumes the emitted tokens

`MaterialTheme` (library-owned `Styles` subclass) merges the four emitted files
into its resources and layers the hand-authored control themes on top — roughly:

```xml
<!-- MaterialTheme.axaml (library-owned — illustrative) -->
<Styles ...>
  <Styles.Resources>
    <ResourceDictionary>
      <ResourceDictionary.MergedDictionaries>
        <ResourceInclude Source="avares://CCSWE.Avalonia.Material/Fonts.axaml" />
        <ResourceInclude Source="avares://CCSWE.Avalonia.Material/Tokens.axaml" />
        <ResourceInclude Source="avares://CCSWE.Avalonia.Material/Motion.axaml" />
      </ResourceDictionary.MergedDictionaries>
    </ResourceDictionary>
  </Styles.Resources>

  <StyleInclude Source="avares://CCSWE.Avalonia.Material/Typography.axaml" />
  <!-- … plus the library's hand-authored Base/ + Controls/ includes … -->
</Styles>
```

The control themes reference the **global M3 role brushes directly**
(`{DynamicResource Primary}`, etc.) — they have **no base theme to `BasedOn`**, so
every token reference is a `DynamicResource` that resolves against the merged
application resources (and re-resolves on a `ThemeVariant` switch).

## Consuming-app wiring

```xml
<!-- App.axaml -->
<Application xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:theme="using:CCSWE.Avalonia.Material"
             RequestedThemeVariant="Dark">
  <Application.Styles>
    <theme:MaterialTheme />
  </Application.Styles>
</Application>
```

1. **`RequestedThemeVariant="Dark"`** — dark is the app default (matches web +
   upstream intent). Bind it to a setting to make it user-switchable; the emitted
   `ThemeDictionaries` respond automatically.
2. **`<theme:MaterialTheme/>`** — the single entry. No `FluentTheme` line, no
   `FluentOverrides` merge — there is no base theme to sit on or remap.

## Theme variant switching

```csharp
// Flip at runtime — ThemeDictionaries do the rest.
Application.Current!.RequestedThemeVariant =
    isLight ? ThemeVariant.Light : ThemeVariant.Dark;
```

High-contrast: not emitted this cycle. When wired, the upstream
`darkHighContrast` / `lightHighContrast` schemes would emit as additional
`ThemeDictionaries` keys and the host would request them the same way.

## The type-size tokens (`FontSize<Role>`)

`Tokens.axaml` emits the 15-role M3 type-size scale as `x:Double` resources
(`FontSizeDisplayLarge` … `FontSizeLabelSmall`). `Typography.axaml` references them
for its `FontSize` setters, so type sizes have a single source of truth. The
library (or a consumer) can also reference a size directly —
`FontSize="{DynamicResource FontSizeTitleLarge}"` — when styling a control that
isn't a plain `TextBlock`. (These tokens were hand-added in the library during the
v1.x no-base migration; as of v2 the DS owns and emits them.)

## Verifying a fresh bundle

- `Tokens.axaml` carries a `Dark` and a `Light` theme dictionary, each with the
  full M3 role set as `Color` + matching `SolidColorBrush`, the **status roles**
  (`Success` / `Warning` / `Info`, each a four-resource quad), plus the disabled
  alpha brushes (`OnSurface12` / `OnSurface38`) and `Scrim32`.
- The root of `Tokens.axaml` carries the `CornerRadius*`, `Spacing*`
  (`double` + `Thickness`), and `FontSize*` (all 15 roles) metrics.
- `Typography.axaml`'s `FontSize` setters resolve to the `FontSize*` tokens (grep
  for a literal numeric `FontSize` value — there should be none; all are
  `{DynamicResource FontSize…}`).
- `Fonts.axaml` is the only emitted file with an `avares://` URI, and it points at
  `CCSWE.Avalonia.Material`.
- Embedded fonts resolve at runtime — verify on a real run, not just the designer
  preview.

## Workflow (verification loop)

This DS project can't compile axaml, so the **library is the verifier**. Each
emit cycle owes a compile + runtime pass by the consumer, who returns an
`uploads/avalonia-integration-*.md` note. Read the latest → apply changes → run the
release checklist in `CLAUDE.md`.

## Testing checklist

- [ ] Library builds with the four emitted files dropped in verbatim (0 errors,
      0 AVLN warnings)
- [ ] Dark (default) and Light both render; runtime `ThemeVariant` switch repaints
      every role brush (DynamicResource — no frozen brushes)
- [ ] Type-scale classes apply the right `FontSize` (display 57 → label-small 11)
      and resolve the `FontSize*` tokens, not inlined numbers
- [ ] Embedded DM Sans / Plus Jakarta Sans load (not a system fallback)
- [ ] Color roles, spacing, corner radii, and motion values match `Tokens.axaml` /
      `Motion.axaml` exactly (the no-drift preview is the visual reference)
