# `Base/` — interim infrastructure layer (library-owned)

These files are the no-base theme's **interim infrastructure layer**, forked once from
**`Avalonia.Themes.Simple` @ tag `12.0.4`** and recolored to the M3 token system. They are
**hand-authored / library-owned** — **not** emitted by the CCSWE design system (the DS emits
only the token layer: `Tokens` / `Typography` / `Motion` / `Fonts`). Do **not** add this
folder to the DS regenerate pipeline.

## The folder rule (`Controls/` vs `Base/`)

- **`Controls/*.axaml`** — a control with a **from-scratch M3 `ControlTheme`** (M3 anatomy,
  state layers, shape, motion; references the M3 tokens directly). Library-owned, hand-authored.
- **`Base/*.axaml`** — the **`Avalonia.Themes.Simple`-derived layer**: verbatim/near-verbatim
  structural forks AND recolored-but-still-Simple-skeleton infra (e.g. `Window`, `SplitView`,
  popup/overlay hosts, the date-time spinner pickers).

A control **leaves `Base/` → `Controls/` only when it is given a from-scratch M3 theme.**
Merely recoloring a Simple fork to M3 tokens keeps it in `Base/` (it's still the Simple
skeleton). This is why, e.g., `DatePicker`/`TimePicker` are M3-recolored but remain in `Base/`,
while the from-scratch `Calendar` grid moved to `Controls/`.

## What's here now

- **Shims** — `BaseAliases.axaml` / `SimplePalette.axaml` / `Strings.axaml`. Glue that defines
  the non-token keys the remaining forks reference (layout metrics, `FontSizeNormal`,
  `ContentControlThemeFontFamily`, `ScrollBar*`/`CaptionButton*` sizes, invariant strings) and —
  historically — aliased the Simple `Theme*` palette onto the M3 tokens. **No control theme
  references the `Theme*` color/opacity palette anymore;** the shims persist only for the
  layout/metric/string keys the kept infra forks still consume.
- **Recolored infra forks** — `Window`, `WindowDrawnDecorations`, `WindowNotificationManager`,
  `PopupRoot`, `OverlayPopupHost`, `EmbeddableControlRoot`, `AdornerLayer`, `ThemeVariantScope`,
  `TransitioningContentControl`, `PathIcon`, `TextSelectionHandle`, `SplitView`. Structural
  plumbing recolored to M3 roles; kept as forks (pure-plumbing may stay near-verbatim).
- **Recolored date-time spinner pickers** — `DatePicker`, `TimePicker`, `DateTimePickerShared`.
  Simple spinner skeleton recolored to M3 (Surface field, `SurfaceContainerHigh` popup,
  `SecondaryContainer` selection band).

See `MIGRATION.md` for the per-control status table.

## Ownership & maintenance

- We **own and may hand-edit** these (M3 recolor on the visible chrome; reconciliation on
  Avalonia upgrades).
- **On every Avalonia upgrade:** diff each remaining fork against the same file in the matching
  Avalonia tag and reconcile contract changes (`PART_*` names, new controls). This is the
  permanent cost of owning the base; it shrinks as forks are hand-rolled to `Controls/`.
- **End state:** no forks, no shims — a pure M3 component theme on Avalonia core, fed brand
  tokens by the DS.
