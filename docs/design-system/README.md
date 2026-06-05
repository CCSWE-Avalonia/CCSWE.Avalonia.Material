# CCSWE Avalonia Design System

**Design System (Avalonia):** 1.7.3
**Tokens:** 1.1.0 · **Avalonia:** 12
**Sources of truth:** `tokens/tokens.upstream-1.1.0.json` (the master CCSWE cross-platform tokens — primitive ramps + four-scheme M3 semantic layer + 15-role type scale + shape + motion, consumed **verbatim**) and `tokens/tokens.local.json` (the Avalonia/.NET desktop translation layer — spacing scale, letterSpacing resolution, resource-naming convention, font delivery, which schemes to wire).

It is the desktop sibling of the **web** bundle (emits `tokens.css`) and the **Android** bundle (emits Kotlin/XML), all consuming one shared token set. It turns those tokens into Avalonia `ResourceDictionary` + `Styles` files that drop into the **CCSWE.Avalonia.Material** library, so desktop is a **consume-verbatim** platform alongside web/Android rather than hand-translating tokens on every version bump.

The goal: **a branded theme that gives stock Avalonia controls the Material 3 look.**

---

## What this bundle is (and isn't)

It is a **theme + tokens** bundle. It ships:

- **Color schemes** — Dark (app default) + Light, as `ResourceDictionary.ThemeDictionaries`, every M3 role emitted as a paired `Color` + `SolidColorBrush`.
- **Metrics** — the M3 shape scale as `CornerRadius` resources (plus a `CornerRadiusFull` pill sentinel), the 4px spacing scale as `double` + `Thickness` resources.
- **Type scale** — the 15-role M3 type scale as `TextBlock` style classes.
- **Fonts** — `avares://` `FontFamily` resources for the two embedded families.
- **Motion** — the M3 motion scale (10 durations as `TimeSpan`, 6 easings as `SplineEasing`) in `Motion.axaml`, consumed directly by Avalonia animations/transitions.
- **Control themes** — M3 restyling of stock controls, built entirely from token resources: the **button** family (Filled, Filled Tonal, Elevated, Outlined, Text, Icon), **`ToggleButton`** (Outlined → Filled-Tonal flip), **text fields** (Filled + Outlined `TextBox`), **`AutoCompleteBox`** (M3 field + suggestion popup), **`NumericUpDown`** (M3 field + spinner buttons), **selection controls** (`CheckBox`, `RadioButton`, `ToggleSwitch`/M3 Switch), **lists** (`ListBox` / `ListBoxItem`), **`TreeView`/`TreeViewItem`** (M3 rows + chevron + indentation), **`ComboBox`**, **menus** (`Menu` / `MenuItem` / `ContextMenu` / `MenuFlyoutPresenter` / `Separator`), **`Expander`**, **`Slider`** (H+V), **`ProgressBar`** (determinate + indeterminate), **`TabControl`/`TabItem`** and **`TabStrip`/`TabStripItem`** (M3 primary tabs), and an M3 **Card** convention (`Border.Card` Elevated/Filled/Outlined), and an M3 **navigation drawer + rail** (`DrawerPage` pane surface + scrim + width defaults, with a `ListBox.NavigationDrawer` destination treatment — the 56dp active-indicator pill — and a compact `ListBox.NavigationRail` for the 80dp rail modes — a 56×32 icon-only indicator with the label below). Popup surfaces (combo/menu/autocomplete) are themed in-control.

Targets **Avalonia 12** (`FluentTheme` base). All control-theme role references use `DynamicResource` so they track Dark↔Light `ThemeVariant` switches at runtime. As of **1.5.3**, disabled state is a uniform M3 recolor across every control (`OnSurface12` container / `OnSurface38` content, never a whole-element opacity dim), and state-driven border-thickness changes never reflow content (overlay-border / constant-thickness rule).

It does **not** ship: a component library, custom controls, or app plumbing. It also does not yet wire high-contrast schemes or the red/yellow/green variant accents (encoded upstream, but desktop has no consumer for them yet — wired-but-unused).

---

## What the DS emits vs. what the library owns

The split:

| | **Design System emits** (consume-verbatim, never hand-edit) | **CCSWE.Avalonia.Material library owns** (hand-authored) |
|---|---|---|
| **Rule** | Anything that is a *pure function of the tokens* | Anything that depends on app/runtime context or Avalonia framework wiring |
| **Files** | `Tokens.axaml`, `Fonts.axaml`, `Motion.axaml`, `Typography.axaml`, all `Controls/*.axaml`, `Theme.axaml` | `FluentOverrides.axaml` (accent remap), `App.sample.axaml` (wiring), theme-variant switching, packaging/NuGet, markup/class helper conveniences |

The **control themes are emitted** — an M3 control template is a deterministic function of the tokens (e.g. a button's container = `Primary`, label = `OnPrimary`, shape = `CornerRadiusFull`, state layers at fixed M3 opacities), so it belongs in the bundle, not in hand-written library code. What the library hand-authors is the *glue* the tokens can't express on their own: remapping FluentTheme's accent so the controls the DS doesn't theme still pick up the brand, choosing the active `ThemeVariant`, and shipping the font bytes.

**The litmus test:** if you can regenerate it from `tokens/` with no human decision, the DS emits it. If it needs to know about FluentTheme, the window, or app settings, the library owns it.

---

## Files

```
tokens/
  tokens.upstream-1.1.0.json   master tokens, consumed verbatim
  tokens.local.json            Avalonia-side desktop decisions
CCSWE.Avalonia.Material/          (library root — emit straight here, no Themes/ wrapper)
  Tokens.axaml                 ThemeDictionaries Dark/Light + metrics  ← single source of truth for color/metric values
  Fonts.axaml                  avares:// FontFamily resources
  Motion.axaml                 durations (TimeSpan) + easings (SplineEasing)
  Typography.axaml             15 M3 type roles as TextBlock classes
  Theme.axaml                  one-stop merged include
  Controls/Button.axaml        M3 button family ControlThemes
  Controls/TextBox.axaml       M3 Filled + Outlined text fields
  Controls/CheckBox.axaml      M3 checkbox
  Controls/RadioButton.axaml   M3 radio
  Controls/ToggleSwitch.axaml  M3 switch
  Controls/ListBox.axaml       M3 list + list item (state layer, selected)
  Controls/ComboBox.axaml      M3 dropdown + themed popup + item
  Controls/Menu.axaml          Menu/MenuItem/ContextMenu/MenuFlyout/Separator
  Controls/Expander.axaml      M3 expander (rotating chevron)
  Controls/Card.axaml          Border.Card (Elevated/Filled/Outlined) — reserved class name
  Controls/Slider.axaml        M3 slider (Horizontal + Vertical)
  Controls/ProgressBar.axaml   M3 progress (determinate + indeterminate)
  Controls/TabControl.axaml    M3 primary tabs + TabItem indicator
  Controls/TabStrip.axaml      M3 TabStrip / TabStripItem (content-less tabs)
  Controls/TreeView.axaml      M3 tree rows + expand chevron + per-level indent
  Controls/AutoCompleteBox.axaml  M3 field + suggestion popup (in-control chrome)
  Controls/ToggleButton.axaml  M3 toggle (Outlined → Filled-Tonal on :checked)
  Controls/NumericUpDown.axaml M3 field + ButtonSpinner + spinner RepeatButtons
  Controls/DrawerPage.axaml    M3 navigation drawer + rail (destinations + pane/scrim defaults)
  FluentOverrides.axaml        HAND-AUTHORED — FluentTheme accent → brand remap
  App.sample.axaml             HAND-AUTHORED — non-compiled wiring snippet
  Assets/Fonts/                fetch-fonts.sh / .ps1 (acquisition) → TTFs + OFL.txt (see FONTS.md)
README.md  HANDOFF.md  CONVENTIONS.md  FONTS.md  FLUENT-AUDIT.md  CHANGELOG.md
```

(The design-system source also maintains an HTML `preview/` showcase generated
from `Tokens.axaml`, but it is a dev artifact and is **not** part of this
delivered package.)

---

## Usage

```xml
<!-- App.axaml -->
<Application RequestedThemeVariant="Dark"> <!-- Dark is the app default -->
  <Application.Styles>
    <FluentTheme />
    <StyleInclude Source="avares://CCSWE.Avalonia.Material/Theme.axaml" />
  </Application.Styles>
  <Application.Resources>
    <!-- ResourceInclude must be merged via MergedDictionaries, not placed directly. -->
    <ResourceDictionary>
      <ResourceDictionary.MergedDictionaries>
        <ResourceInclude Source="avares://CCSWE.Avalonia.Material/FluentOverrides.axaml" />
      </ResourceDictionary.MergedDictionaries>
    </ResourceDictionary>
  </Application.Resources>
</Application>
```

```xml
<!-- Any view -->
<StackPanel Spacing="12">
  <TextBlock Classes="HeadlineSmall" Text="Account" />
  <TextBox Classes="Outlined" PlaceholderText="Display name" />
  <AutoCompleteBox PlaceholderText="Search city" />
  <NumericUpDown Value="1" Minimum="0" Maximum="99" />
  <Button Classes="Filled" Content="Save" />
  <Button Classes="Outlined" Content="Add another" />
  <CheckBox Content="Remember this device" />
  <ToggleSwitch Content="Enable notifications" />

  <!-- M3 toggle: Outlined when off, Filled-Tonal when :checked (default theme, no class) -->
  <ToggleButton Content="Wireless" />

  <!-- Content-less primary tabs; TabStripItem is content-based (like ListBoxItem) -->
  <TabStrip>
    <TabStripItem Content="All" />
    <TabStripItem Content="Active" />
    <TabStripItem Content="Archived" />
  </TabStrip>

  <!-- Tree rows mirror ListBoxItem; chevron + per-level indent come for free -->
  <TreeView>
    <TreeViewItem Header="Workspace" IsExpanded="True">
      <TreeViewItem Header="Documents" />
      <TreeViewItem Header="Images" />
    </TreeViewItem>
  </TreeView>

  <Button Classes="Text" Content="Cancel" />
</StackPanel>
```

The new 1.5.0 controls — `AutoCompleteBox`, `NumericUpDown`, `ToggleButton`, `TabStrip`/`TabStripItem`, `TreeView`/`TreeViewItem` — are **default-themed** (`{x:Type …}`), so they pick up the M3 look with no class. `Button`/`TextBox` still take a class to choose the variant.

Reference any color role directly with `{DynamicResource Primary}` (brush) or `{DynamicResource PrimaryColor}` (color). Use `DynamicResource` for role brushes so they re-resolve when the theme variant flips; `StaticResource` is fine for the theme-invariant metrics.

---

## No-drift discipline

`Tokens.axaml` is the single source of truth for color/metric values. The
design-system source keeps an HTML showcase whose styles are **generated by
parsing `Tokens.axaml`** — so the showcase can never drift from what the library
consumes. That showcase is a DS-side dev artifact and is not shipped in this
package; the point for consumers is simply that every value here traces back to
`Tokens.axaml`.

---

## Design decisions

The desktop translation layer (`tokens/tokens.local.json`) resolves the choices the cross-platform core leaves open: the resource-naming convention (paired `Color` + `SolidColorBrush` per role), `letterSpacing` resolution (emit the upstream value as an absolute DIP — not `em × fontSize`), the spacing scale (the web 4px scale, adopted verbatim), and font delivery (embed DM Sans + Plus Jakarta Sans as OFL static TTFs). See `CONVENTIONS.md` for the full rationale and `CHANGELOG.md` for per-version history.
