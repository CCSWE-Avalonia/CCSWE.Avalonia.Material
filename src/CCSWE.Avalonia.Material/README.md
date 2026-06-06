# CCSWE.Avalonia.Material

A standalone **Material 3** theme for [Avalonia](https://avaloniaui.net) 12 — a
Dark/Light color system, the M3 type scale, motion, embedded brand fonts, and M3
control themes for the full control set. Depends only on **Avalonia core** — no
`FluentTheme`/`SimpleTheme` base required.

## Install

```sh
dotnet add package CCSWE.Avalonia.Material
```

> **Versioning:** the **major version tracks the supported Avalonia major** — `12.x`
> targets **Avalonia 12.x**. Pick the major matching your Avalonia version; minor/patch
> are this library's own features and fixes.

## Wire it up

Add one element to your `App.axaml`:

```xml
<Application xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             xmlns:theme="using:CCSWE.Avalonia.Material"
             x:Class="YourApp.App"
             RequestedThemeVariant="Dark"> <!-- Dark is the default -->

  <Application.Styles>
    <theme:MaterialTheme />
  </Application.Styles>
</Application>
```

`MaterialTheme` is standalone — it supplies the whole control surface itself and
depends only on Avalonia core. **No `<FluentTheme/>` (or other base theme) is required.**

## Use it

```xml
<StackPanel Spacing="12">
  <TextBlock Classes="HeadlineSmall" Text="Devices" />
  <TextBox Classes="Outlined" PlaceholderText="IP address" />
  <Button Classes="Filled" Content="Connect" />
  <Button Classes="Outlined" Content="Pair new device" />
  <CheckBox Content="Connect automatically" />
  <ToggleSwitch Content="Wireless debugging" />
</StackPanel>
```

- **Buttons:** `Filled`, `FilledTonal`, `Elevated`, `Outlined`, `Text`, `Icon`.
- **Text fields:** `Outlined`, `Filled`.
- **Cards:** `Border` with `Card` + `Elevated` / `Filled` / `Outlined`.
- **Default-themed (no class):** `ToggleButton`, `AutoCompleteBox`, `NumericUpDown`,
  `CheckBox`, `RadioButton`, `ToggleSwitch`, `ListBox`, `TreeView`, `ComboBox`,
  `Menu`, `Expander`, `Slider`, `ProgressBar`, `TabControl`, `TabStrip`.
- **Type scale:** `DisplayLarge … LabelSmall` as `TextBlock` classes.
- **Color roles:** `{DynamicResource Primary}` (brush) /
  `{DynamicResource PrimaryColor}` (color). Use `DynamicResource` for role brushes
  so they re-resolve when the theme variant flips.

### Switch theme variant at runtime

```csharp
Application.Current!.RequestedThemeVariant =
    isLight ? ThemeVariant.Light : ThemeVariant.Dark;
```

## License

[MIT](https://github.com/CoryCharlton/CCSWE.Avalonia.Material/blob/master/LICENSE.md).
Bundles **DM Sans** and **Plus Jakarta Sans** under the
[SIL Open Font License 1.1](https://openfontlicense.org); their `OFL.txt` files
ship in the package under `THIRD-PARTY-NOTICES/`.

Source, docs, and the control gallery:
<https://github.com/CoryCharlton/CCSWE.Avalonia.Material>
