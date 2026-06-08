# M3 dimension conformance audit

A control-by-control audit of the **dimensions** (height, width, padding, margin, corner radius, outline/indicator thickness, icon/thumb sizes) defined in `src/CCSWE.Avalonia.Material/Controls/*.axaml` against the **Material 3** component specs (m3.material.io component anatomy + the Android M3 reference; cross-checked against Semi.Avalonia where the m3 page is JS-gated).

Scope: dimensions only — not color roles, motion, or type (covered elsewhere). M3 spec values are the published reference; where M3 publishes no equivalent (Avalonia-only controls), the verdict is **n/a** and we only note internal consistency.

Verdict key: **✓** matches M3 · **⚠** minor/intentional deviation · **✗** wrong, should fix · **n/a** no M3 equivalent.

> Disposition: discrepancies on controls touched by the compact-density work are fixed inline in that PR (the corrected value becomes the Normal base). Out-of-scope discrepancies are logged here as follow-ups and **not** fixed opportunistically, to keep the density change reviewable.

---

## Text-field family

| Control | Dimension | Current | M3 | Verdict | Note |
|---|---|---|---|---|---|
| TextBox | container height | 56 | 56 | ✓ | |
| TextBox | horizontal padding | 16,0 | 16 lead/trail | ✓ | |
| TextBox | outline (rest→focus) | 1→2 | 1→2 | ✓ | |
| TextBox | filled top radius | 4,4,0,0 | 4 (top) | ✓ | |
| TextBox | supporting-text margin | 16,4,16,0 | 16 start / 4 top | ✓ | |
| ComboBox (field) | height + padding | 56 + 16,0 | 56 + 16 | ✓ | exposed dropdown menu = text field |
| AutoCompleteBox | height | 56 | 56 | ✓ | text-field surface |
| NumericUpDown | height + padding | 56 + 16,0 | 56 + 16 | ✓ | text-field surface |

## Buttons

| Control | Dimension | Current | M3 | Verdict | Note |
|---|---|---|---|---|---|
| Button (common) | height | 40 | 40 | ✓ | |
| Button (filled/tonal/outlined/elevated) | padding | 24,0 | 24 (text-only) | ✓ | |
| Button (text) | padding | 12,0 | 12 | ✓ | |
| Button (icon) | size + padding | 40×40 + 8 | 40 + 8 (24 icon) | ✓ | |
| Button (outlined) | outline | 1 | 1 | ✓ | |
| ToggleButton | height + padding | 40 + 24,0 | 40 + 24 | ✓ | |
| SplitButton | height + padding | 40 + 20,0 | 40 | ⚠ | 20 padding (vs 24) to balance the divider+chevron; acceptable |
| DropDownButton | height + padding | 40 + 20,0,16,0 | 40 | ⚠ | asymmetric for trailing chevron; acceptable |
| HyperlinkButton | padding | 4,2 | n/a | n/a | M3 has no hyperlink button |
| RepeatButton | padding | 4 | n/a | n/a | infra control |
| FAB (standard) | size + radius | 56 + Large(16) | 56 + 16 | ✓ | |
| FAB (small) | size + radius | 40 + Medium(12) | 40 + 12 | ✓ | |
| FAB (large) | size + radius | 96 + ExtraLarge(28) | 96 + 28 | ✓ | |
| CommandBar | bar height | 40/64 | n/a | n/a | not an M3 component (WinUI lineage) |

## Selection controls

| Control | Dimension | Current | M3 | Verdict | Note |
|---|---|---|---|---|---|
| CheckBox | box / border / corner | 18 / 2 / 2 | 18 / 2 / 2 | ✓ | |
| CheckBox | state layer | 40 (r20) | 40 | ✓ | M3 state layer 40; recommended hit-target 48 |
| CheckBox | touch target | 40 | 48 (min) | ⚠ | 40 hit area is below M3's 48 recommendation |
| RadioButton | outer / inner dot | 20 / 10 | 20 / 10 | ✓ | |
| RadioButton | state layer | 40 (r20) | 40 | ✓ | |
| RadioButton | touch target | 40 | 48 (min) | ⚠ | same 40-vs-48 note as CheckBox |
| ToggleSwitch | track | 52×32 (r16, 2 outline) | 52×32 | ✓ | exact M3 switch — large by spec |
| ToggleSwitch | thumb off→on | 16→24 | 16→24 | ✓ | (28 pressed not implemented — minor) |

## Lists, menus, items, tabs

| Control | Dimension | Current | M3 | Verdict | Note |
|---|---|---|---|---|---|
| ListBox item | height + padding | 48 + 16,12 | 56 (one-line) + 16 | ⚠ | 48 is denser than M3 one-line list (56); intentional general-purpose list density — **confirm intent** |
| ComboBox item | height + padding | 48 + 16,10 | 48 + 12–16 | ✓ | menu list-item |
| Menu item | height | content-sized (pad 12,8) | 48 (min) | ✗ | no explicit `MinHeight` → rows can fall below 48dp; add 48 min |
| Menu item | horizontal padding | 12 | 12 | ✓ | |
| TreeView item | height + padding | 48 + 8,12,16,12 | 48 | ✓ | indent-aware padding |
| TabControl / TabStrip | tab height | 48 | 48 | ✓ | |
| TabControl / TabStrip | active indicator | 3 | 3 (primary) | ✓ | |
| TabbedPage | tab header min height | 48 (`TabbedPageTabItemHeaderMinHeight`) | 48 | ✓ | already tokenized |

## Sliders & progress

| Control | Dimension | Current | M3 | Verdict | Note |
|---|---|---|---|---|---|
| Slider | track / thumb / state layer | 4 / 20 / 40 | 4 / 20 / 40 | ✓ | classic M3 (pre-expressive) |
| ProgressBar (linear) | height | 4 | 4 | ✓ | |
| CircularProgressIndicator | size | 48 | 48 | ✓ | |

## Date family

| Control | Dimension | Current | M3 | Verdict | Note |
|---|---|---|---|---|---|
| CalendarDayButton | cell / selection | 40 (margin 1) | 48 cell / 40 select | ⚠ | selection circle 40 ✓; grid cell tighter than 48 |
| CalendarButton (month/year) | height | 42 (margin 2) | 40 (or 48) | ✗ | 42 is an odd one-off — align to 40 |
| CalendarDatePicker | icon / glyph | 32 / 20 | ~24 target | ⚠ | picker affordance; review with date pickers |
| Calendar / CalendarItem | grid metrics | various | — | n/a | composite; relies on day/month buttons above |

## Containers

| Control | Dimension | Current | M3 | Verdict | Note |
|---|---|---|---|---|---|
| Card | padding | 16 | 16 (common) | ✓ | |
| GroupBox | padding | 16 | n/a | n/a | not an M3 component |
| Expander | header padding | 16,14 | n/a | n/a | M3 expander is newer/unstable; 16,14 reasonable |
| Divider | thickness | 1 | 1 | ✓ | |
| ToolTip | padding | 8,5 | 8 / 4 (plain) | ⚠ | vertical 5 vs ~4; trivial |
| NotificationCard | min height + padding | 64 + 16,12 | 48 (snackbar single-line) | ⚠ | 64 taller than single-line snackbar; intentional for richer cards — confirm |
| FlyoutPresenter / Menu popup | padding | 4 / 0,4 | 8 (menu v-pad) | ⚠ | menu vertical padding 4 vs M3 8; trivial |

## Navigation & shells

| Control | Dimension | Current | M3 | Verdict | Note |
|---|---|---|---|---|---|
| DrawerPage (drawer item) | height | 56 | 56 | ✓ | |
| DrawerPage (rail width) | 80 | 80 | ✓ | nav rail |
| DrawerPage (modal width) | 360 | 360 (max) | ✓ | nav drawer |
| NavigationPage / TabbedPage / ContentPage / CarouselPage | shell metrics | various | n/a | n/a | page-shell composites, no direct M3 spec |
| PipsPager | pip / spacing | 12 / margins | n/a | n/a | WinUI lineage, no M3 equivalent |

## Chrome

| Control | Dimension | Current | M3 | Verdict | Note |
|---|---|---|---|---|---|
| ScrollBar | thickness | 8 (12 expanded) | — | n/a | platform chrome |
| GridSplitter | thickness | 1 | — | n/a | |
| RefreshVisualizer | size | 80 / 24 icon | — | n/a | pull-to-refresh, no M3 spec |

---

## Follow-ups (out-of-scope discrepancies to address separately)

1. **Menu item lacks a 48dp `MinHeight`** (`Controls/Menu.axaml`) — ✗ rows can render below the M3 48dp minimum. *Highest priority.*
2. **CalendarButton `MinHeight=42`** (`Controls/CalendarButton.axaml:14`) — ✗ odd value; align to 40 (or 48 to match day cell).
3. **CheckBox / RadioButton 40dp hit target vs M3's 48dp recommendation** — ⚠ deliberate (matches M3 state-layer 40); revisit if accessibility-critical.
4. **ListBox item 48dp vs M3 one-line list 56dp** — ⚠ confirm the denser default is intended (it reads as a compact list by default).
5. **NotificationCard 64dp vs snackbar single-line 48dp** — ⚠ confirm richer-card intent.
6. **Menu/Flyout vertical padding 4 vs M3 8; ToolTip 8,5 vs 8,4** — ⚠ trivial; batch if we ever normalize popup chrome.

*Items addressed by the compact-density PR (text-field 56, item 48, button 40, switch 52×32 — all ✓) need no follow-up.*
