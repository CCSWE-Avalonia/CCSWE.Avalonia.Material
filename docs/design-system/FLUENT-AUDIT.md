# FluentTheme override audit — RETIRED

> **This document is obsolete.** As of the no-base migration, CCSWE.Avalonia.Material
> no longer uses `FluentTheme` (or any base theme) — it is a **standalone Material 3
> theme** that supplies the whole control surface itself. There are no Fluent
> `SystemControl*`/`SystemAccentColor*` keys to remap, so the entire override audit and
> its three-bucket strategy (and the former `FluentOverrides.axaml`) no longer apply.

See the current contract in the repo root [`CLAUDE.md`](../../CLAUDE.md):

- **DS emits the token layer only** (`Tokens.axaml`, `Typography.axaml`, `Motion.axaml`,
  `Fonts.axaml`); the **library owns all control themes** (`Controls/*`), the
  `MaterialTheme` entry, and the interim `Base/*` infrastructure.
- Control themes reference the **global M3 roles directly** — no base-theme key remapping.
- North star: **Material 3 / Android fidelity**.

Kept as a tombstone so the history of the override strategy is discoverable; do not
add to it.
