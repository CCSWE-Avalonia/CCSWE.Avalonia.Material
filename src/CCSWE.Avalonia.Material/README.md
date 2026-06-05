# CCSWE.Avalonia.Material

A branded **Material 3** theme for [Avalonia](https://avaloniaui.net) 12 — a
Dark/Light color system, the M3 type scale, motion, embedded brand fonts, and M3
control themes that give stock Avalonia controls a consistent look.

## Install

```sh
dotnet add package CCSWE.Avalonia.Material
```

## Wire it up

Add three things to your `App.axaml`:

```xml
<Application xmlns="https://github.com/avaloniaui"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
             x:Class="YourApp.App"
             RequestedThemeVariant="Dark"> <!-- Dark is the default -->

  <Application.Styles>
    <FluentTheme />
    <StyleInclude Source="avares://CCSWE.Avalonia.Material/Theme.axaml" />
  </Application.Styles>

  <Application.Resources>
    <ResourceDictionary>
      <ResourceDictionary.MergedDictionaries>
        <ResourceInclude Source="avares://CCSWE.Avalonia.Material/FluentOverrides.axaml" />
      </ResourceDictionary.MergedDictionaries>
    </ResourceDictionary>
  </Application.Resources>
</Application>
```

1. `<FluentTheme />` supplies the control templates this theme doesn't override.
2. `Theme.axaml` layers the tokens, type scale, motion, and M3 control themes on top.
3. `FluentOverrides.axaml` (merged *after* the styles) remaps Fluent's accent onto
   the brand so stock controls (Slider, ProgressBar, selection highlights) match.

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
