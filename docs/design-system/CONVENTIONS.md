# CCSWE Avalonia Bundle — Conventions

Naming and placement rules the Avalonia **token** emitter follows. Mirrors the
Android bundle's `CONVENTIONS.md`, adapted to AXAML / .NET. Read before editing the
emitter or adding a resource.

> **v2 — tokens only.** The DS emits `Tokens` / `Typography` / `Motion` / `Fonts`
> and nothing else. The **library** (`CCSWE.Avalonia.Material`) is a standalone
> Material 3 theme (no `FluentTheme`/`SimpleTheme` base): it hand-authors every M3
> control theme + the `MaterialTheme` entry + an interim `Base/` infra layer. The
> control-template conventions that used to live here (full-template emit, required
> parts, state-reflow, disabled recolor, popup chrome, the `Style`-vs-`ControlTheme`
> and FluentOverrides token-remap rules, …) are **retired from the DS** — they are
> framework-coupled and now belong to the library. The token / resource-naming and
> `DynamicResource` rules below still hold.

---

## Resource naming

### Color roles — paired `Color` + `SolidColorBrush`

Every M3 semantic role emits **two** resources:

| Resource | Key | Why |
|---|---|---|
| `SolidColorBrush` | bare PascalCase role (`Primary`, `OnSurfaceVariant`, `SurfaceContainerHigh`) | The common case — most Avalonia APIs (`Background`, `Foreground`, `BorderBrush`) want an `IBrush`. The bare name is the one you reach for. |
| `Color` | role + `Color` suffix (`PrimaryColor`, `OnSurfaceVariantColor`) | For the APIs that want a raw `Color` — `Pen`, `GradientStop`, `BoxShadow`, interop. The brush references this via `{StaticResource}`. |

Rationale for the bare-name-is-brush direction: brush usage outnumbers color usage
at call sites ~10:1 in stock Avalonia markup, so the shorter key goes to the common
case. **Pinned.**

Within a `ThemeDictionary`, all `Color`s are declared **before** the
`SolidColorBrush`es — `{StaticResource}` resolves top-down within a dictionary.

Two derived alpha brushes ship in each `ThemeDictionary`, built from `OnSurfaceColor`
/ `ScrimColor` with an explicit `Opacity`: **`OnSurface12`** (disabled container fill /
outline @ 12%), **`OnSurface38`** (disabled content @ 38%), and **`Scrim32`** (modal
backdrop @ 32%). These are the M3 state primitives the library's control themes consume
— emitted as tokens so the library never inlines an alpha.

### Status roles — `Success` / `Warning` / `Info`

Beyond the canonical M3 ColorScheme, three **status** roles ship in each
`ThemeDictionary`, each a four-resource quad on the `Error*` pattern: the bare role
(`Success`), `On<Role>` (`OnSuccess`), `<Role>Container` (`SuccessContainer`), and
`On<Role>Container` (`OnSuccessContainer`) — paired `Color` + `SolidColorBrush` like
every other role. They cover the M3 "custom color roles" the library's notification /
banner / validation / badge surfaces need (it had only `Error*` before).

These are a **desktop decision**, not (yet) part of the upstream semantic layer, which
carries only `error`. Sourcing is recorded in `tokens.local.json` (`statusRoles`) and
flagged upstream to canonicalize: **Success** ← the Android green variant accent,
**Warning** ← the yellow variant accent (both `primary→Role` quad mappings), and **Info**
← the Primary family (brand blue) **deliberately** — there is no blue variant and the
brand primary is already blue, so `Info == Primary` on purpose. When upstream adopts the
roles, the emitter consumes them verbatim like the rest and the desktop-side sourcing
retires.

### Metrics

| Type | Key pattern | Examples |
|---|---|---|
| `CornerRadius` | `CornerRadius<ShapeStep>` | `CornerRadiusSmall`, `CornerRadiusExtraLarge`, `CornerRadiusFull` |
| spacing `double` | `Spacing<webKey>` | `Spacing4` (=16), `Spacing6` (=24) |
| spacing `Thickness` | `Spacing<webKey>Thickness` | `Spacing4Thickness`, `Spacing6Thickness` |
| type-size `double` | `FontSize<Role>` | `FontSizeDisplayLarge` (=57), `FontSizeLabelSmall` (=11) |

`Spacing<webKey>` keeps the web token's numbering (`spacing.4 = 16`), not the pixel
value, so web and desktop call the same step the same name. `FontSize<Role>` uses the
exact M3 role name (`FontSizeTitleLarge`) — one `x:Double` per type role, the single
source of truth for type sizes (`Typography.axaml` references them, see below).

### Style classes (type scale)

`TextBlock` type roles are PascalCase class selectors matching the M3 role name
exactly: `TextBlock.DisplayLarge`, `TextBlock.LabelMedium`. Applied via
`Classes="HeadlineSmall"`.

---

## File placement

| File | Holds | Kind |
|---|---|---|
| `Tokens.axaml` | color roles (Dark/Light) + disabled/scrim brushes + metrics (`CornerRadius` / `Spacing` / `FontSize`) | `ResourceDictionary` |
| `Fonts.axaml` | `FontFamily` resources | `ResourceDictionary` |
| `Motion.axaml` | duration (`sys:TimeSpan`) + easing (`SplineEasing`) resources | `ResourceDictionary` |
| `Typography.axaml` | type-scale style classes | `Styles` |

All four emit at the **library project root** (no `Themes/` wrapper). That is the
entire emitted surface — the library owns `MaterialTheme`, `Controls/`, and `Base/`.

**Resources vs. Styles.** Color/brush/metric/font/type-size values are *resources*
(`ResourceDictionary`) — they are looked up. Type classes are *styles* (`Styles`) —
applied by selector. The library's `MaterialTheme` bridges the two: it merges the
resource dictionaries into its `Styles.Resources` and `StyleInclude`s `Typography.axaml`
alongside its own control themes.

---

## Token resolution at emit time

Same model as the CSS and Kotlin emitters: resolve `ref`/`value` from
`tokens.upstream-1.1.0.json` at generation time and emit concrete values. A role
with `{ "ref": "surface.950" }` emits the resolved `#FF020617`; a role with
`{ "value": "#172238" }` emits it directly. The Avalonia bundle consumes the
**master** semantic layer verbatim — it does **not** apply the Android-side overrides
(e.g. Android remaps `inverseSurface` onto the slate ramp; desktop keeps the master's
gray-ramp value).

### Color format

Avalonia `Color` literals are emitted as `#AARRGGBB` with an explicit `FF` alpha
(`#FF659EC7`), never `#RGB` or `#RRGGBB` — explicit alpha avoids any ambiguity and
matches how `Color.Parse` round-trips.

### Type sizes

The upstream `typography.role.fontSize` values are unitless DIPs and emit directly as
the `FontSize<Role>` `x:Double`s. `LineHeight` / `FontWeight` / `LetterSpacing` are
per-role and stay inline in `Typography.axaml` (not tokenized).

---

## Resolved cross-platform questions

| Question (from the consumer) | Resolution |
|---|---|
| **letterSpacing** em→DIP | Emit the upstream value **as an absolute DIP, verbatim**. `TextBlock.LetterSpacing` is an absolute DIP offset; the upstream "em" label is a misnomer — the figures are M3's published per-role tracking, applied as absolute by every consumer (Android passes them to Compose as `.sp`). `em × fontSize` is what produced the too-wide tracking and is **wrong**. |
| **spacing** scale | Adopt the **web 4px scale verbatim** (`0,4,8,…,96`), emitted as `double` + `Thickness`. Desktop and web now share one neutral-density ramp. |
| **type sizes** | Emit the 15-role M3 scale as `FontSize<Role>` `x:Double` tokens (single source of truth); `Typography.axaml` references them. |
| **resource naming** | `Color` + `Brush` pair, bare-PascalCase brush / `…Color` color. Pinned. |
| **fonts** | Embed DM Sans + Plus Jakarta Sans as OFL variable (or static) TTFs, referenced via `avares://` `FontFamily`. See `FONTS.md`. |
| **HC schemes** | Encoded upstream, **not emitted** — no desktop HC toggle yet. |
| **variant accents** (red/yellow/green) | Not consumed wholesale. The green + yellow accents are re-keyed into the **status roles** (Success/Warning/Info) — see below; full per-screen variant theming stays a per-platform (Android) concern. |
| **status roles** (Success/Warning/Info) | A desktop decision this cycle: emitted into `Tokens.axaml` on the `Error*` quad pattern (Success←green accent, Warning←yellow accent, Info=Primary family), **flagged upstream** to canonicalize. Values + sourcing in `tokens.local.json` `statusRoles`. |

---

## DynamicResource vs. StaticResource

- **Cross-file token references resolve with `DynamicResource`.** `Typography.axaml`'s
  `FontSize` setters reference the `FontSize<Role>` tokens in `Tokens.axaml` via
  `DynamicResource` — even though type sizes are theme-invariant. A `DynamicResource`
  resolves against the merged application resources at runtime, so the cross-file lookup
  is reliable regardless of include/parse order; a `StaticResource` would require the
  target resource to be in scope at parse time of `Typography.axaml`, which it is not.
  (The library's control themes follow the same rule for **all** token references —
  color roles especially, where `StaticResource` would *freeze* the brush at parse time
  and the control would keep its Dark colors when the variant flips to Light.)
- **`StaticResource` only for same-file refs** — e.g. a `SolidColorBrush` referencing
  its `…Color` within the same `ThemeDictionary`.
- **Consumer ad-hoc markup**: prefer `DynamicResource` for role brushes/colors (so they
  track variant switches); `StaticResource` is acceptable for the theme-invariant
  metrics (`CornerRadius*`, `Spacing*`, `FontSize*`, `Motion*`).

---

## Motion (`Motion.axaml`)

- 10 durations emitted as `sys:TimeSpan` resources (`MotionDurationShort1` …
  `MotionDurationLong2`); both `Animation.Duration` and `Transition.Duration` are
  `TimeSpan`-typed in Avalonia, so they bind directly.
- 6 easings emitted as `SplineEasing` resources (`MotionEasing*`) built from the
  upstream cubic-bezier control-point arrays (`X1 Y1 X2 Y2`).
- Reference via `DynamicResource` inside control themes/transitions (library);
  `StaticResource` is fine in a standalone `<Animation>` in app markup.
