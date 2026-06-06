# M3 migration status (Base/ forks → hand-rolled M3)

Single source of truth for Phase B: which controls are hand-rolled to M3, which are
interim forks, and which are verified in the Demo. Update this **per control** as you go.

**States**
- ✅ **M3** — hand-rolled M3 ControlTheme in `Controls/`, owns `{x:Type}`, no `Base/` fork left.
- 🔶 **Partial** — M3 themes exist for *classed/styled* use, but the plain `{x:Type}` default
  is still the `Base/` fork (e.g. a bare `<Button>` falls back to forked-Simple).
- 🏗 **Infra (recolored)** — structural infra; kept as a `Base/` fork but recolored to M3 tokens.
- ⬜ **Forked** — verbatim Simple fork, not yet touched.

**Verified** = rendered and visually checked in the Demo (note how).

## Status

| Control | State | Verified | Notes |
|---|---|---|---|
| Button | ✅ M3 | classed ✓ | keyed M3 variants + `{x:Type Button}` default = Filled; `Base/Button` deleted |
| TextBox | ✅ M3 | classed ✓ | keyed M3 variants + `{x:Type TextBox}` default = Outlined; `Base/TextBox` deleted |
| DrawerPage | ✅ M3 | ✓ (demo shell) | full `{x:Type DrawerPage}` ControlTheme: M3 template + SurfaceContainerLow pane/header/footer, Surface bars, Scrim32 backdrop, 360/80 widths, baked-in PART_PaneDivider (1dp OutlineVariant seam, placement-aware, hidden in Overlay); nav drawer + rail item themes; `Base/DrawerPage` deleted |
| ScrollBar | ✅ M3 | ✓ 2026-06-05 | thin pill thumb, no arrows; `Base/ScrollBar` deleted |
| AutoCompleteBox, CheckBox, ComboBox, Card, Expander, ListBox, Menu, NumericUpDown, ProgressBar, RadioButton, Slider, TabControl, TabStrip, ToggleButton, ToggleSwitch, TreeView | ✅ M3 | mostly ✓ | owned `{x:Type}` (Card via style classes); no `Base/` fork |
| Window, WindowDrawnDecorations | 🏗 Infra | ✓ (boots) | `Base/Window` recolored to M3 tokens; chrome stays forked |
| PopupRoot, OverlayPopupHost, EmbeddableControlRoot, ThemeVariantScope | 🏗 Infra | runs | recolored to M3 (Surface/OnSurface); pure plumbing, stay forked |
| TextSelectionHandle | 🏗 Infra | runs | selection grabber recolored to Primary; stays forked |
| SplitView, WindowDrawnDecorations | 🏗 Infra | runs | recolored to M3 (SurfaceContainerLow pane, Scrim light-dismiss; Outline window border, Primary fullscreen bar); stay forked |
| AdornerLayer, PathIcon, TransitioningContentControl, WindowNotificationManager | ⬜ Forked | runs | pure plumbing, no color refs; stay near-verbatim |
| FlyoutPresenter | ✅ M3 | — | M3 flyout/menu surface (SurfaceContainer + OutlineVariant, extra-small corners); fork deleted |
| RepeatButton | ✅ M3 | — | transparent w/ surface state-layer tints, OnSurfaceVariant, disabled 0.38; fork deleted |
| ScrollViewer | ✅ M3 | ✓ (every page) | transparent track + transparent corner filler (pairs with the thin overlay ScrollBar); dropped dead SimpleMenuScrollViewer; `Base/ScrollViewer` deleted |
| ToolTip | ✅ M3 | — | InverseSurface chip, ExtraSmall corners, fade-in on :open; `Base/ToolTip` deleted |
| HyperlinkButton, DropDownButton, SplitButton | ✅ M3 | — | hand-rolled M3 (link; Tonal pill + dropdown chevron / split); forks deleted |
| CommandBar (+ Button / ToggleButton / Separator) | ✅ M3 | — | SurfaceContainer toolbar, OnSurfaceVariant content, M3 state layers (8/12%), toggle→SecondaryContainer, overflow popup = M3 menu surface; `Base/CommandBar` deleted |
| GroupBox | ✅ M3 | — | M3 outlined container, header LabelLarge OnSurfaceVariant, MediumCorner; `Base/GroupBox` deleted |
| Label | ✅ M3 | — | OnSurface, BodyLarge, disabled 0.38; `Base/Label` deleted |
| SelectableTextBlock | ✅ M3 | — | OnSurface, selection PrimaryContainer/OnPrimaryContainer; `Base/SelectableTextBlock` deleted |
| GridSplitter | ✅ M3 | — | OutlineVariant divider, Outline on hover, Primary preview; `Base/GridSplitter` deleted |
| PipsPager | ✅ M3 | — | OutlineVariant pips, Primary selected, M3 icon nav buttons; `Base/PipsPager` deleted |
| ItemsControl, HeaderedContentControl | ✅ M3 | — | structural; OnSurface foreground default; forks deleted |
| RefreshContainer, RefreshVisualizer | ✅ M3 | — | RefreshVisualizer spinner → Primary, transparent bg; forks deleted |
| NotificationCard | ✅ M3 | — | neutral snackbar default + full status mapping (info/success/warning/error → *Container/On*Container, DS 2.1.0 status roles); `Base/NotificationCard` deleted |
| ContentPage, CarouselPage, Carousel | ✅ M3 | — | Surface page hosts; forks deleted |
| TabbedPage | ✅ M3 | — | M3 primary tabs: Primary indicator/label, OnSurfaceVariant unselected, surface state layers, 3dp rounded indicator; fork deleted |
| NavigationPage | ✅ M3 | — | M3 top app bar: SurfaceContainer bar, OnSurface TitleLarge header, circular back-button state layers, Primary focus ring; fork deleted |
| Calendar, CalendarButton, CalendarDayButton, CalendarItem, CalendarDatePicker, DatePicker, TimePicker, DateTimePickerShared | ⬜ Forked | — | date family — **deferred ("maybe later")** |

## How "done" is defined

A control is ✅ **M3** when: (1) a hand-authored `Controls/<X>.axaml` provides its M3 theme
(owning `{x:Type X}` where applicable), (2) the `Base/<X>.axaml` fork is **deleted**, and
(3) it's verified in the Demo. The `Base/` folder is the live "pending/partial" queue; this
table records the **verified** status the folder split can't show (and flags the 🔶 partials
that exist in both folders).
