# Handoff — CCSWE.Avalonia.Theme

How a developer (or Claude Code) wires the emitted bundle into the
**CCSWE.Avalonia.Theme** library and a consuming desktop app.

## Scope

A **theme + tokens** library — color schemes, metrics, type scale, fonts,
motion, and M3 control themes (button family & `ToggleButton`, text fields,
`AutoCompleteBox`, `NumericUpDown`, selection controls, lists & `TreeView`,
`ComboBox`, menus, `Expander`, `Slider`, `ProgressBar`, tabs `TabControl`/`TabStrip`,
cards, navigation drawer + rail `DrawerPage`). It does
**not** ship custom controls or app plumbing. Consumers style stock Avalonia
controls with the emitted resources and classes and let the tokens drive the look.

## Library project layout

```
CCSWE.Avalonia.Theme/            (Avalonia 12 class library; TFM-agnostic, net8.0+)
├── CCSWE.Avalonia.Theme.csproj
├── Tokens.axaml                 ← emitted bundle, drop in verbatim at the project root
├── Fonts.axaml
├── Motion.axaml
├── Typography.axaml
├── Theme.axaml                  (one-stop include)
├── Controls/
│   ├── Button.axaml
│   ├── TextBox.axaml
│   ├── CheckBox.axaml
│   ├── RadioButton.axaml
│   ├── ToggleSwitch.axaml
│   ├── ListBox.axaml
│   ├── ComboBox.axaml
│   ├── Menu.axaml
│   ├── Expander.axaml
│   ├── Card.axaml
│   ├── Slider.axaml
│   ├── ProgressBar.axaml
│   ├── TabControl.axaml
│   ├── TabStrip.axaml          (1.5.0)
│   ├── TreeView.axaml          (1.5.0)
│   ├── AutoCompleteBox.axaml   (1.5.0)
│   ├── ToggleButton.axaml      (1.5.0)
│   ├── NumericUpDown.axaml     (1.5.0)
│   └── DrawerPage.axaml        (1.6.0 drawer · 1.7.0 rail)
├── FluentOverrides.axaml        ← hand-authored (see FLUENT-AUDIT.md)
├── App.sample.axaml             ← hand-authored, non-compiled wiring snippet
└── Assets/Fonts/                ← run fetch-fonts.{sh,ps1} (see FONTS.md)
    ├── fetch-fonts.sh / .ps1
    ├── PlusJakartaSans[wght].ttf  + PlusJakartaSans-OFL.txt
    └── DMSans[opsz,wght].ttf      + DMSans-OFL.txt
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

The `avares://CCSWE.Avalonia.Theme/...` URIs in the emitted files assume the
assembly name is `CCSWE.Avalonia.Theme`. If you name the assembly differently,
find-and-replace the authority segment in `Theme.axaml`, `Fonts.axaml`, and
`FluentOverrides.axaml`.

## Consuming-app wiring

See `App.sample.axaml` for the full file. The moving parts:

1. **`RequestedThemeVariant="Dark"`** on `<Application>` — dark is the app
   default (matches web + upstream intent). Bind it to a setting to make it
   user-switchable; the emitted `ThemeDictionaries` respond automatically.
2. **`<FluentTheme />` then `Theme.axaml`** in `Application.Styles` —
   FluentTheme supplies the control templates the CCSWE bundle doesn't
   override; `Theme.axaml` layers tokens + type + control themes on top.
3. **`FluentOverrides.axaml` merged via `ResourceDictionary.MergedDictionaries`**
   inside `Application.Resources` — a `ResourceInclude` cannot sit directly under
   `Application.Resources` (it's an `IResourceDictionary`). It remaps FluentTheme's
   accent onto brand so non-themed controls (e.g. `CalendarDatePicker`,
   `ScrollBar`) still read as CCSWE. Merge *after* the styles. See the sample for the exact nesting.

## Theme variant switching (library-owned)

```csharp
// Flip at runtime — ThemeDictionaries do the rest.
Application.Current!.RequestedThemeVariant =
    isLight ? ThemeVariant.Light : ThemeVariant.Dark;
```

High-contrast: not emitted this cycle. When wired, the upstream
`darkHighContrast` / `lightHighContrast` schemes would emit as custom
`ThemeVariant` keys and the host would request them the same way.

## Navigation drawer & rail — consumer markup contract

The bundle themes the **destinations**; the `DrawerPage` container, header, icons, and
labels are consumer content. Two host classes, applied to a `ListBox` inside the
`DrawerPage` pane:

**Drawer** (full-width pill, `DrawerBehavior=Locked`/`Split` or overlay) — icon + label
go in `Content`:

```xml
<ListBox Classes="NavigationDrawer" SelectedIndex="0">
  <ListBoxItem>
    <StackPanel Orientation="Horizontal" Spacing="12">
      <PathIcon Data="{StaticResource DashboardGlyph}" Width="24" Height="24" />
      <TextBlock Classes="LabelLarge" Text="Dashboard" VerticalAlignment="Center" />
    </StackPanel>
  </ListBoxItem>
  <!-- … -->
</ListBox>
```

**Rail** (compact 80dp, `DrawerLayoutBehavior=CompactInline`/`CompactOverlay`) — the
indicator hugs the **icon only** and the label sits below, so the icon goes in `Content`
and the label string in **`Tag`** (the no-C# split; `Tag` is stringly-typed — no
`HeaderTemplate`):

```xml
<ListBox Classes="NavigationRail" SelectedIndex="0">
  <ListBoxItem Tag="Dashboard">
    <PathIcon Data="{StaticResource DashboardGlyph}" Width="24" Height="24" />
  </ListBoxItem>
  <!-- … -->
</ListBox>
```

Icons are consumer-supplied (the bundle ships no icon set). The rail's active state
colors icon and label **differently** (icon `OnSecondaryContainer`, label `OnSurface`)
— that's M3-correct, not a bug.

### Trailing-edge divider (docked) — interim until upstream property

M3's docked drawer/rail carries a **1dp `OutlineVariant` divider on the pane's trailing
edge**. `DrawerPage` exposes **no border property**, so the theme can't emit it (it's a
`Style` layered over Fluent's template, and there's no `DrawerBorderBrush` to set — see
CHANGELOG 1.7.0 / CONVENTIONS). A `DrawerBorderBrush`/`DrawerBorderThickness` request is
filed upstream. **Until it lands, draw it consumer-side** in **docked** modes only
(skip `Overlay`/`CompactOverlay` — the `Scrim32` backdrop already separates):

```xml
<!-- Between the DrawerPage pane and the main content, DrawerPlacement=Left.
     Mirror to "1,0,0,0" for Right. -->
<Border BorderBrush="{DynamicResource OutlineVariant}" BorderThickness="0,0,1,0" />
```

Use this one pattern (don't fork it per view) so the seam stays consistent until the
emitted treatment replaces it.

## Regenerating the preview (no-drift contract — DS-side dev artifact)

> The HTML preview is a **design-system-side dev artifact** and is **not** shipped
> in the delivered package. This section documents the DS's own regen step; it is
> not something a library consumer needs to run.

`preview/tokens.preview.css` is generated from `Tokens.axaml`, not
hand-maintained. The generator:

1. Parses the `Dark` and `Light` `ResourceDictionary` blocks under
   `ResourceDictionary.ThemeDictionaries`, reading every `<Color x:Key="...">`.
2. Emits `:root[data-theme="dark"]` / `:root[data-theme="light"]` blocks of
   `--<kebab-role>` custom properties (e.g. `OnSurfaceVariantColor` →
   `--on-surface-variant`).
3. Parses the root `CornerRadius` + spacing `double` resources into
   `--corner-radius-*` / `--spacing-*` vars.

Re-run it after any `Tokens.axaml` edit so the showcase never diverges from
what the library ships. The button visuals in `Preview.html` mirror the roles,
radii, and state-layer opacities in `Controls/Button.axaml`.

## Verifying a fresh bundle

- `Tokens.axaml` carries a `Dark` and a `Light` theme dictionary, each with the
  full M3 role set as `Color` + matching `SolidColorBrush`.
- Every value in `Controls/Button.axaml` is a `{StaticResource}` — grep for a
  literal `#` hex inside it; there should be none except the elevation
  `BoxShadow` (a shadow tint, not a token color).
- `RequestedThemeVariant="Dark"` is the documented default everywhere.
- Embedded fonts resolve at runtime — verify on a real run, not just the
  designer preview.

## Testing checklist

- [ ] App picks up the theme — visual diff against the consuming app's current build
- [ ] Dark (default) and Light both render; runtime switch works
- [ ] Filled / Tonal / Elevated / Outlined / Text / Icon buttons all show M3
      shape (full pill), correct role colors, and hover/pressed state layers
- [ ] Disabled buttons: container @ 0.12, label @ 0.38
- [ ] TextBox Filled + Outlined: focus thickens the indicator/border to 2px
      Primary; watermark shows while empty; `:error` turns it Error-red
- [ ] CheckBox / RadioButton: unchecked = 2px Outline-variant; checked = Primary
      fill / ring + glyph; hover/press show the circular state layer
- [ ] ToggleSwitch: thumb grows 16→24 and slides; track flips to Primary on
      check, with the emphasized-easing motion
- [ ] ToggleButton: Outlined when off, Filled-Tonal (SecondaryContainer) on
      `:checked`; pill shape + state layer; disabled label = neutral OnSurface38
- [ ] AutoCompleteBox: inner field reads as the M3 Outlined TextBox; suggestion
      popup uses the SurfaceContainer chrome and rows highlight like ListBoxItems
- [ ] NumericUpDown: M3 Outlined field box, `:focus-within` → 2px Primary; the
      up/down spinners step the value and show a circular state layer
- [ ] TabStrip / TabStripItem: matches the TabControl indicator + state layer
- [ ] TreeView / TreeViewItem: 48dp rows, OnSurface state layer, selected =
      SecondaryContainer; chevron rotates on expand; nested rows indent 24dp/level
- [ ] Navigation drawer: `ListBox Classes="NavigationDrawer"` shows 56dp full-pill
      destinations; active = SecondaryContainer / OnSecondaryContainer (icon + label
      recolor together); `DrawerPage` pane = SurfaceContainerLow, modal scrim = Scrim @ 32%
- [ ] Navigation rail: `ListBox Classes="NavigationRail"` (in a CompactInline/CompactOverlay
      `DrawerPage`) shows 80dp icon-over-label items; 56×32 indicator hugs the icon; active
      recolors icon → OnSecondaryContainer **and** label → OnSurface; selection doesn't reflow
- [ ] Theme switch repaints all of the above (DynamicResource — no frozen brushes)
- [ ] Non-themed stock controls (e.g. CalendarDatePicker, ScrollBar) pick up the
      brand accent via FluentOverrides, not Fluent blue (see FLUENT-AUDIT.md)
- [ ] Embedded DM Sans / Plus Jakarta Sans load (not a system fallback)
