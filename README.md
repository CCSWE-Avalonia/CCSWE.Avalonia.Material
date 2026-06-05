# CCSWE.Avalonia.Theme

[![Build](https://github.com/CoryCharlton/CCSWE.Avalonia.Theme/actions/workflows/dotnet-build-publish-library.yml/badge.svg)](https://github.com/CoryCharlton/CCSWE.Avalonia.Theme/actions/workflows/dotnet-build-publish-library.yml)
[![NuGet](https://img.shields.io/nuget/v/CCSWE.Avalonia.Theme.svg)](https://www.nuget.org/packages/CCSWE.Avalonia.Theme)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE.md)

A branded **Material 3** theme for [Avalonia](https://avaloniaui.net) 12. It gives
stock Avalonia controls the CCSWE look: a Dark/Light color system, the M3 type
scale, motion, embedded brand fonts, and M3 control themes for buttons (incl.
toggle buttons), text fields, autocomplete, numeric steppers, selection controls,
lists, tree views, dropdowns, menus, expander, cards, sliders, progress, and tabs
(tab control + tab strip).

It is the desktop sibling of the CCSWE **web** and **Android** bundles — all three
consume the same shared cross-platform design tokens. This library turns those
tokens into Avalonia `ResourceDictionary` + `Styles` and ships them as a NuGet
package.

![CCSWE.Avalonia.Theme demo gallery — Material 3 controls with a Dark/Light toggle](docs/images/demo.gif)

## Install & wire up

```sh
dotnet add package CCSWE.Avalonia.Theme
```

Then add three things to your `App.axaml` (full sample in
[`docs/samples/App.sample.axaml`](docs/samples/App.sample.axaml)):

```xml
<Application xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             x:Class="YourApp.App"
             RequestedThemeVariant="Dark"> <!-- Dark is the CCSWE default -->

  <Application.Styles>
    <FluentTheme />
    <StyleInclude Source="avares://CCSWE.Avalonia.Theme/Theme.axaml" />
  </Application.Styles>

  <Application.Resources>
    <ResourceDictionary>
      <ResourceDictionary.MergedDictionaries>
        <ResourceInclude Source="avares://CCSWE.Avalonia.Theme/FluentOverrides.axaml" />
      </ResourceDictionary.MergedDictionaries>
    </ResourceDictionary>
  </Application.Resources>
</Application>
```

1. **`<FluentTheme />`** supplies the control templates this theme doesn't override.
2. **`Theme.axaml`** layers the tokens, type scale, motion, and M3 control themes on top.
3. **`FluentOverrides.axaml`** (merged *after* the styles) remaps Fluent's accent
   onto the brand so stock controls (Slider, ProgressBar, selection highlights)
   read as CCSWE too.

## Use it

```xml
<StackPanel Spacing="12">
  <TextBlock Classes="HeadlineSmall" Text="Devices" />
  <TextBox Classes="Outlined" PlaceholderText="IP address" />
  <Button Classes="Filled" Content="Connect" />
  <Button Classes="Outlined" Content="Pair new device" />
  <CheckBox Content="Connect automatically" />
  <ToggleSwitch Content="Wireless debugging" />
  <Button Classes="Text" Content="Cancel" />
</StackPanel>
```

- **Buttons:** `Filled`, `FilledTonal`, `Elevated`, `Outlined`, `Text`, `Icon`.
- **Text fields:** `Outlined`, `Filled`.
- **Cards:** a `Border` with `Card` + `Elevated` / `Filled` / `Outlined`.
- **Default-themed (no class needed):** `ToggleButton`, `AutoCompleteBox`,
  `NumericUpDown`, `CheckBox`, `RadioButton`, `ToggleSwitch`, `ListBox`, `TreeView`,
  `ComboBox`, `Menu`, `Expander`, `Slider`, `ProgressBar`, `TabControl`, `TabStrip`.
- **Type scale:** `DisplayLarge … LabelSmall` as `TextBlock` classes.
- **Color roles:** `{DynamicResource Primary}` (brush) / `{DynamicResource PrimaryColor}`
  (color). Use `DynamicResource` for role brushes so they re-resolve on a
  Dark↔Light switch; `StaticResource` is fine for the theme-invariant metrics
  (`CornerRadius*`, `Spacing*`, `Motion*`).

### Switching theme variant

Dark is the default. Flip at runtime — the `ThemeDictionaries` do the rest:

```csharp
Application.Current!.RequestedThemeVariant =
    isLight ? ThemeVariant.Light : ThemeVariant.Dark;
```

## Repository layout

```
src/
  CCSWE.Avalonia.Theme/        the theme library (NuGet package)
    Theme.axaml                one-stop include (add this to App.axaml)
    Tokens.axaml               Dark/Light color roles + metrics
    Fonts.axaml                embedded FontFamily resources
    Motion.axaml               durations + easings
    Typography.axaml           M3 type-scale TextBlock classes
    Controls/*.axaml           M3 control themes
    FluentOverrides.axaml      FluentTheme accent remap (hand-authored)
    Assets/Fonts/              embedded OFL TTFs (DM Sans, Plus Jakarta Sans)
  CCSWE.Avalonia.Theme.Demo/   gallery app — visual verification harness
tokens/                        shared token JSON (source of truth)
docs/design-system/            the design-system handoff bundle (conventions, fonts, audit)
docs/samples/                  App.axaml wiring sample
```

The `Theme.axaml` / `Tokens.axaml` / `Controls/*` files are **emitted from the
shared tokens** — treat them as consume-verbatim and regenerate from `tokens/`
rather than hand-editing. The library hand-authors only the *glue* the tokens
can't express (`FluentOverrides.axaml`, the font bytes, packaging). See
[`CLAUDE.md`](CLAUDE.md) and [`docs/design-system/`](docs/design-system/) for the
full contract.

## Build & run

```bash
# Build everything
dotnet build src/CCSWE.Avalonia.Theme.slnx -c Release

# Run the gallery (Dark/Light toggle + every themed control)
dotnet run --project src/CCSWE.Avalonia.Theme.Demo

# Pack the library
dotnet pack src/CCSWE.Avalonia.Theme/CCSWE.Avalonia.Theme.csproj -c Release
```

Requires the .NET 10 SDK (pinned via `global.json`).

## License

The library is under [`LICENSE.md`](LICENSE.md). The embedded fonts (DM Sans, Plus
Jakarta Sans) are under the SIL Open Font License 1.1 — their `OFL.txt` files ship
alongside the TTFs in `src/CCSWE.Avalonia.Theme/Assets/Fonts/`.
