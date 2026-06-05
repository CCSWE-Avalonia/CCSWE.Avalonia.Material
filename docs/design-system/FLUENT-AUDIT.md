# FluentTheme override audit — CCSWE.Avalonia.Theme

**Avalonia:** 12.0.4 · **Bundle:** 1.6.0 · **Fluent keys verified against:** `Avalonia.Themes.Fluent` 12.0.3 (consumer confirms keys stable through 12.0.4)
**Base theme:** `FluentTheme` (Avalonia's stock control templates)
**Question:** with our token brushes loaded and our ControlThemes covering a few
controls, *which stock Fluent controls still render with Fluent's default colors
instead of CCSWE brand?* — and what's the minimum override set to fix that.

This is the "real audit" the first-cut `FluentOverrides.axaml` deferred. It
sorts every common control into one of three buckets and pins the override set.

---

## Method

FluentTheme resolves control colors three ways:

1. **From `SystemAccentColor`** (+ its `Light1..3` / `Dark1..3` tonal siblings) —
   anything Fluent considers "accent": Slider, ProgressBar, ToggleButton checked,
   selection highlights, TabItem indicator, etc. Remapping the accent reaches all
   of these at once.
2. **From neutral `SystemControl*` / `*Brush` chrome keys** — ScrollBar, ToolTip,
   borders, base text. These do **not** follow the accent; they need explicit
   overrides to land on our slate/gray ramps.
3. **From a control's own `ControlTheme`** — which we replace outright for the
   controls we theme (Button, TextBox, CheckBox, RadioButton, ToggleSwitch,
   ListBox, ComboBox, Menu family, Expander, Slider, ProgressBar, TabControl;
   plus the `Border.Card` convention).

So the override strategy is: **(a)** remap the accent (covers bucket 1), **(b)**
replace ControlThemes for high-traffic controls (bucket 3 — now incl. lists,
combobox, menus, expander, with popup surfaces themed in-control), **(c)**
override the handful of neutral chrome keys that visibly clash (bucket 2).

---

## Bucket 1 — reached by the accent remap (no per-control work)

Remapping `SystemAccentColor` + tonal siblings to brand Primary (per theme
variant) brand-izes these stock Fluent controls for free:

| Control | What the accent paints |
|---|---|
| unstyled selection (no ControlTheme) | checked/selected background + indicator |
| `CalendarDatePicker` / `Calendar` | selected day |
| `ScrollBar` | (thumb stays neutral — see bucket 2) |
| caret / text selection in unstyled text surfaces | accent |

**Status: covered.** `Slider`, `ProgressBar`, and `TabControl`/`TabItem` were here
in 1.3.x (accent-tinted only); as of 1.4.0 they are **full ControlThemes** (bucket
3) — M3 templates, not just an accent tint. **`ToggleButton` and `TreeView`/`TreeViewItem`
left this bucket in 1.5.0** — also now full ControlThemes (the accent-only chrome
was the wrong M3 shape / had no state layer). The tonal spread was confirmed to read
well in the consumer's runtime pass — we flatten `Light1..3`/`Dark1..3` toward brand
rather than Fluent's wide tonal fan, so gradients/hover tints are subtler, which is
the intended M3 flatter accent language.

---

## Bucket 3 — replaced by CCSWE ControlThemes

| Control | File |
|---|---|
| `Button` (Filled/Tonal/Elevated/Outlined/Text/Icon) | `Controls/Button.axaml` |
| `TextBox` (Filled/Outlined) + `DataValidationErrors` (error text → M3 Error) | `Controls/TextBox.axaml` |
| `CheckBox` | `Controls/CheckBox.axaml` |
| `RadioButton` | `Controls/RadioButton.axaml` |
| `ToggleSwitch` | `Controls/ToggleSwitch.axaml` |
| `ListBox` / `ListBoxItem` | `Controls/ListBox.axaml` |
| `ComboBox` / `ComboBoxItem` | `Controls/ComboBox.axaml` |
| `Menu` / `MenuItem` / `ContextMenu` / `MenuFlyoutPresenter` / `Separator` | `Controls/Menu.axaml` |
| `Expander` | `Controls/Expander.axaml` |
| `Slider` (H + V) | `Controls/Slider.axaml` |
| `ProgressBar` (determinate + indeterminate) | `Controls/ProgressBar.axaml` |
| `TabControl` / `TabItem` | `Controls/TabControl.axaml` |
| `TabStrip` / `TabStripItem` | `Controls/TabStrip.axaml` |
| `TreeView` / `TreeViewItem` | `Controls/TreeView.axaml` |
| `AutoCompleteBox` (+ in-control suggestion popup) | `Controls/AutoCompleteBox.axaml` |
| `ToggleButton` (Outlined → Filled-Tonal on `:checked`) | `Controls/ToggleButton.axaml` |
| `NumericUpDown` / `ButtonSpinner` (M3 field + spinners) | `Controls/NumericUpDown.axaml` |
| Card (`Border.Card` Elevated/Filled/Outlined) | `Controls/Card.axaml` |
| Navigation drawer — `ListBox.NavigationDrawer` destinations (keyed `M3NavigationDrawerItem` via `ItemContainerTheme`) + `DrawerPage` pane/scrim/width defaults (layered as a `Style`, not a template replacement — see CONVENTIONS) | `Controls/DrawerPage.axaml` |

These ignore Fluent's resources entirely (full templates built from tokens), so
no override is needed — and our CheckBox/Radio/Switch deliberately do **not**
rely on the accent remap.

**All Tier-1 five (TabStrip, TreeView, AutoCompleteBox, ToggleButton,
NumericUpDown) passed the consumer's Dark + Light runtime visual pass — ⚠️ →
✅.** TabStrip, TreeView, ToggleButton and NumericUpDown confirmed on 1.5.1; the pass
also surfaced an `AutoCompleteBox` **empty-dropdown** runtime defect (bare
`SelectingItemsControl` part has no default theme → renders nothing), fixed at source
in **1.5.2** by emitting a `ListBox` for `PART_SelectingItemsControl`. Chevron rotation
+ 24dp indent (TreeView), spinner stepping + `:focus-within` 2px Primary
(NumericUpDown), tonal `:checked` (ToggleButton), suggestion-popup rows
(AutoCompleteBox, 1.5.2), and indicator parity with TabControl (TabStrip) all read
on-brand. The whole bucket-3 set is now both compile- and visually-confirmed.

### Popup chrome is themed in-control (resolves the deferred `SystemControl*` question)

ComboBox dropdowns, `MenuFlyoutPresenter`, `ContextMenu`, and the MenuItem submenu
each paint their **own** popup surface (`SurfaceContainer`, 4px radius, elevation
shadow) inside their ControlTheme. So popup chrome is brand-correct **without**
remapping Fluent's broad `SystemControlBackgroundAltHighBrush` /
`SystemControlForegroundBaseHighBrush` base brushes — those stay deferred (now
*moot* for menus/combobox; only relevant if a future stock control's popup isn't
themed in bucket 3). In-control theming beats a global base-surface remap: zero
blast radius.

---

## Bucket 2 — neutral chrome needing explicit overrides

These keys are **not** accent-driven and otherwise keep Fluent's neutral palette,
which clashes with our slate/gray ramps. `FluentOverrides.axaml` maps them to
token roles, keyed per `ThemeVariant`.

> **Verified against `Avalonia.Themes.Fluent` 12.0.3.** Fluent ships only a
> compiled DLL (no source axaml), but the resource `x:Key`s survive as UTF-16
> string literals in the assembly's `#US` heap (they back runtime
> `DynamicResource` lookups) and can be enumerated:
> `strings -e l -n 4 <Avalonia.Themes.Fluent.dll> | grep -E 'ToolTip|ScrollBar|SystemControl'`
> (`-e l` for 16-bit LE is required — plain ASCII `strings` finds nothing). So
> each row below is a yes/no fact against 12.0.3, not a guess. **Keys are a moving
> target across Avalonia versions — re-verify when you bump Fluent.**

| Surface | Fluent key | Token role | Status (12.0.3) |
|---|---|---|---|
| Accent base | `SystemAccentColor` (+ `Light1..3`/`Dark1..3`) | `Primary` (+ tonal) | **verified · LIVE** |
| ToolTip background | `ToolTipBackground` (brush) | `InverseSurface` | **verified · LIVE** |
| ToolTip foreground | `ToolTipForeground` (brush) | `InverseOnSurface` | **verified · LIVE** |
| ToolTip border | `ToolTipBorderBrush` (brush) | `OutlineVariant` | **verified · LIVE** |
| ScrollBar thumb (resting) | `ScrollBarThumbBackgroundColor` (**Color**) | `OutlineVariant` | **verified · LIVE** |
| ScrollBar thumb (hover) | `ScrollBarThumbFillPointerOver` (brush) | `Outline` | **verified · LIVE** |
| Generic surface base | `SystemControlBackgroundAltHighBrush` (brush) | `Surface` | verified · **deferred** |
| Generic on-surface base | `SystemControlForegroundBaseHighBrush` (brush) | `OnSurface` | verified · **deferred** |

**ScrollBar naming asymmetry (a real Fluent quirk).** There is **no**
`ScrollBarThumbFill` brush. The *resting* thumb is colored by a **Color**
resource — `ScrollBarThumbBackgroundColor` — while only the *state* thumbs are
brushes: `ScrollBarThumbFillPointerOver` / `…Pressed` / `…Disabled`. Override the
resting thumb with a `<Color>`, the hover thumb with a `<SolidColorBrush>`.

**Two keys verified but deferred.** `SystemControlBackgroundAltHighBrush` and
`SystemControlForegroundBaseHighBrush` exist, but they are *generic* base
surface/text brushes consumed by many Fluent controls (popups, flyouts, menus,
base text), not just the one surface named. Remapping them is a wide blast radius
that a key-existence check can't sign off — it needs a per-control visual pass.
Shipped **commented** in `FluentOverrides.axaml` until then. The ToolTip/ScrollBar
keys are narrowly scoped and ship **live**.

---

## Open questions / next cycle

- **Runtime visual sign-off** of the now-live ToolTip + ScrollBar surfaces in
  Dark and Light (hover a tooltip; rest/hover a scrollbar thumb) — keys are
  verified present and the build is green; the on-screen check is the final step.
- **The two deferred `SystemControl*` keys** — a per-control pass to decide
  whether brand base surfaces help or regress Fluent popups/menus/flyouts.
- **ComboBox & Menu** — now bucket 3 (themed, incl. their popup surfaces). The
  remaining unthemed dropdown-bearing controls (e.g. `DropDownButton` /
  `SplitButton` flyouts) would follow the same in-control popup-surface pattern.
- **Focus adorner** color: Avalonia draws focus with a pen, not a single themed
  brush — revisit if the default focus ring clashes on brand surfaces.
- **High-contrast**: when HC schemes are emitted, this audit re-runs per HC
  variant (Fluent has its own HC resource set).
- **Re-verify on Fluent bumps** — the key spellings are version-specific (this
  pass: 12.0.3). Re-run the `strings -e l` enumeration when Avalonia updates.
