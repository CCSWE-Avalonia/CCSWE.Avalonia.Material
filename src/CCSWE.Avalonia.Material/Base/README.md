# `Base/` — library-owned infrastructure layer (NOT design-system output)

These files are the **no-base theme's infrastructure layer**. They are **hand-authored /
library-owned**, the same ownership class as the old `FluentOverrides.axaml` — **not**
emitted by the CCSWE design system. Do **not** add this folder to the DS regenerate
pipeline, and do not treat these as consume-verbatim.

## What's here

- **`BaseAliases.axaml`** — hand-authored glue. Defines the `Theme*`/metric/font keys the
  forked templates reference and aliases them onto the M3 token system (`Tokens.axaml`).
  The no-base analog of `FluentOverrides.axaml`, but additive (defines keys) rather than
  corrective (patches a vendor).

- **`*.axaml` control templates** — **forked once from `Avalonia.Themes.Simple` @ tag
  `12.0.4`** (`src/Avalonia.Themes.Simple/Controls/*.xaml`). These are the structural
  controls a base theme must supply that are *not* a pure function of tokens: window /
  popup / overlay hosting, scrollbars, flyout presenter, tooltip, adorner layer, icon
  primitive, validation, etc.

## Ownership & maintenance

- We **own and may hand-edit** these (M3 polish on the visible chrome; reconciliation on
  Avalonia upgrades).
- **Strategy: keep them byte-close to upstream Simple.** Recolor via `BaseAliases.axaml`,
  not by editing each fork. This keeps `diff <fork> vs Avalonia.Themes.Simple/<file>` clean,
  so reconciling on each Avalonia version bump stays cheap. Edit a fork directly only when
  real M3 restyling requires it.
- **On every Avalonia upgrade:** diff each fork against the same file in the matching
  Avalonia tag and reconcile contract changes (`PART_*` names, new controls). This is the
  permanent cost of owning the base — it replaces FluentTheme's free upkeep.

## vs. `Controls/`

`Controls/*.axaml` are **DS-emitted** M3 ControlThemes (regenerate, don't edit). `Base/*`
are **library-owned** structural forks. New DS-emitted styled controls (Calendar,
DropDownButton, page shells, …) belong in `Controls/`, never here.

> Note: during the no-base spike, `Base/DrawerPage.axaml` (forked template) coexists with
> `Controls/DrawerPage.axaml` (the DS-emitted M3 Style-layer on top). When DrawerPage is
> later promoted to a full DS-emitted ControlTheme, `Base/DrawerPage.axaml` is dropped.
