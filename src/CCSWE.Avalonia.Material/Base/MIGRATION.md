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
| DrawerPage | 🔶 Partial | ✓ (demo shell) | `Controls/DrawerPage` Style-layer over forked `Base/DrawerPage` template; promote to full ControlTheme |
| ScrollBar | ✅ M3 | ✓ 2026-06-05 | thin pill thumb, no arrows; `Base/ScrollBar` deleted |
| AutoCompleteBox, CheckBox, ComboBox, Card, Expander, ListBox, Menu, NumericUpDown, ProgressBar, RadioButton, Slider, TabControl, TabStrip, ToggleButton, ToggleSwitch, TreeView | ✅ M3 | mostly ✓ | owned `{x:Type}` (Card via style classes); no `Base/` fork |
| Window, WindowDrawnDecorations | 🏗 Infra | ✓ (boots) | `Base/Window` recolored to M3 tokens; chrome stays forked |
| PopupRoot, OverlayPopupHost, EmbeddableControlRoot, AdornerLayer, ThemeVariantScope, TransitioningContentControl, PathIcon, RepeatButton, TextSelectionHandle, WindowNotificationManager | ⬜ Forked | runs | structural infra; recolor/keep as forks; M3 polish low priority |
| ScrollViewer, FlyoutPresenter, ToolTip, DataValidationErrors | ⬜ Forked | runs | visible chrome — M3 polish wanted |
| DropDownButton, SplitButton, HyperlinkButton, CommandBar | ⬜ Forked | — | button-family gaps — **next up**, reuse Button patterns |
| GroupBox, Label, SelectableTextBlock, PipsPager, GridSplitter, RefreshVisualizer, RefreshContainer, NotificationCard, ItemsControl, HeaderedContentControl | ⬜ Forked | — | styleable gaps |
| ContentPage, CarouselPage, TabbedPage, NavigationPage, Carousel | ⬜ Forked | — | page shells |
| Calendar, CalendarButton, CalendarDayButton, CalendarItem, CalendarDatePicker, DatePicker, TimePicker, DateTimePickerShared | ⬜ Forked | — | date family — **deferred ("maybe later")** |

## How "done" is defined

A control is ✅ **M3** when: (1) a hand-authored `Controls/<X>.axaml` provides its M3 theme
(owning `{x:Type X}` where applicable), (2) the `Base/<X>.axaml` fork is **deleted**, and
(3) it's verified in the Demo. The `Base/` folder is the live "pending/partial" queue; this
table records the **verified** status the folder split can't show (and flags the 🔶 partials
that exist in both folders).
