# CCSWE Avalonia Design System

**Design System (Avalonia):** 2.0.0
**Tokens:** 1.1.0 · **Avalonia:** 12
**Sources of truth:** `tokens/tokens.upstream-1.1.0.json` (the master CCSWE cross-platform tokens — primitive ramps + four-scheme M3 semantic layer + 15-role type scale + shape + motion, consumed **verbatim**) and `tokens/tokens.local.json` (the Avalonia/.NET desktop translation layer — spacing scale, letterSpacing resolution, resource-naming convention, font delivery, which schemes to wire).

It is the desktop sibling of the **web** bundle (emits `tokens.css`) and the **Android** bundle (emits Kotlin/XML), all consuming one shared token set. As of **v2** it emits the **token layer only** — Avalonia `ResourceDictionary` + `Styles` files that drop into the **CCSWE.Avalonia.Material** library, which owns the actual M3 component themes.

---

## v2 — the token layer only (read this first)

The consuming desktop library, **CCSWE.Avalonia.Material**, moved to a **standalone Material 3 theme**: it no longer sits on top of Avalonia's `FluentTheme` (or `SimpleTheme`). It hand-authors every M3 control theme and an interim `Base/` infrastructure layer (forked from `Avalonia.Themes.Simple` 12.0.4, shrinking as controls are rolled to M3). It exposes one entry point — `<theme:MaterialTheme/>`, a `Styles` subclass.

So the emit-vs-own line moved. **There is no shared component model across platforms — only shared tokens.** Each platform themes its own framework-native components. This bundle now mirrors the **Android** design system exactly: it translates the shared tokens into a theme for a framework-native M3 component library and does **not** generate the component templates.

### What the DS emits (v2)

| File | Holds |
|---|---|
| **`Tokens.axaml`** | Dark/Light color roles (`ResourceDictionary.ThemeDictionaries`, paired `Color` + `SolidColorBrush`) + theme-invariant metrics: `CornerRadius*`, the 4px spacing scale (`double` + `Thickness`), and the **M3 type-size scale** `FontSize<Role>` (all 15 roles). |
| **`Typography.axaml`** | the 15-role M3 type scale as `TextBlock` style classes — sizes reference the `FontSize*` tokens as the single source of truth. |
| **`Motion.axaml`** | the M3 motion scale: 10 durations (`sys:TimeSpan`) + 6 easings (`SplineEasing`). |
| **`Fonts.axaml`** | `avares://` `FontFamily` resources for the two embedded families. |

That is the **entire emitted surface** — four files plus the font assets they reference.

### What the DS no longer emits (owned by the library)

- **`Controls/*.axaml`** — every M3 control theme. A control template is framework-coupled (`PART_*` names, pseudo-classes, Avalonia version quirks); it is not a pure function of the tokens, so the library hand-authors it.
- **`Theme.axaml`** — replaced by the library-owned `MaterialTheme` entry.
- **`FluentOverrides.axaml`** — deleted; there is no base theme to remap.

The **litmus test** is unchanged, but the line it draws has moved: if you can regenerate it from `tokens/` with **no human decision and no knowledge of the framework**, the DS emits it (colors, metrics, type sizes, font references, motion values). If it needs to know about control templates, part names, or framework wiring, the library owns it.

---

## Files

```
tokens/
  tokens.upstream-1.1.0.json   master tokens, consumed verbatim
  tokens.local.json            Avalonia-side desktop decisions
CCSWE.Avalonia.Material/        (emitted token layer — drop in verbatim at the library root)
  Tokens.axaml                 ThemeDictionaries Dark/Light + metrics + FontSize scale  ← single source of truth for values
  Fonts.axaml                  avares:// FontFamily resources
  Motion.axaml                 durations (TimeSpan) + easings (SplineEasing)
  Typography.axaml             15 M3 type roles as TextBlock classes (FontSize from Tokens.axaml)
  Assets/Fonts/                fetch-fonts.sh / .ps1 (acquisition) → TTFs + OFL.txt (see FONTS.md)
README.md  HANDOFF.md  CONVENTIONS.md  FONTS.md  CHANGELOG.md
```

(The design-system source also maintains an HTML `preview/` showcase generated
from `Tokens.axaml`, but it is a dev artifact and is **not** part of the
delivered package.)

---

## Usage

The library merges the emitted token files itself and exposes a single entry. A consuming app wires one line — no `FluentTheme`, no override dictionary:

```xml
<!-- App.axaml -->
<Application xmlns="https://github.com/avaloniaui"
             xmlns:theme="using:CCSWE.Avalonia.Material"
             RequestedThemeVariant="Dark"> <!-- Dark is the app default -->
  <Application.Styles>
    <theme:MaterialTheme />
  </Application.Styles>
</Application>
```

`MaterialTheme` (library-owned) merges `Tokens.axaml`, `Fonts.axaml`, `Motion.axaml`, and `Typography.axaml` into its resources and layers its hand-authored M3 control themes on top. The emitted `ThemeDictionaries` respond to `RequestedThemeVariant` automatically.

```xml
<!-- Any view -->
<StackPanel Spacing="12">
  <TextBlock Classes="HeadlineSmall" Text="Account" />
  <TextBox Classes="Outlined" Watermark="Display name" />
  <Button Classes="Filled" Content="Save" />
  <CheckBox Content="Remember this device" />
</StackPanel>
```

(The control classes — `Button.Filled`, `TextBox.Outlined`, the type-scale classes, etc. — are provided by the library's M3 control themes, not by this bundle. The bundle provides the tokens those themes consume.)

Reference any color role directly with `{DynamicResource Primary}` (brush) or `{DynamicResource PrimaryColor}` (color). Use `DynamicResource` for role brushes so they re-resolve when the theme variant flips; `StaticResource` is fine for the theme-invariant metrics (`CornerRadius*`, `Spacing*`, `FontSize*`, `Motion*`).

---

## North star

Match **Material 3 / Android** as closely as possible — color roles, type scale, shape, motion, and (in the library's control themes) component anatomy, naming, and state-layer opacities. Tokens stay aligned with the shared web/Android set; the desktop translation layer only resolves what the cross-platform core leaves open.

---

## No-drift discipline

`Tokens.axaml` is the single source of truth for color/metric/type-size values. The
design-system source keeps an HTML showcase whose styles are **generated by
parsing `Tokens.axaml`** — so the showcase can never drift from what the library
consumes. That showcase is a DS-side dev artifact and is not shipped in this
package; the point for consumers is simply that every value here traces back to
`Tokens.axaml`.

---

## Design decisions

The desktop translation layer (`tokens/tokens.local.json`) resolves the choices the cross-platform core leaves open: the resource-naming convention (paired `Color` + `SolidColorBrush` per role), `letterSpacing` resolution (emit the upstream value as an absolute DIP — not `em × fontSize`), the spacing scale (the web 4px scale, adopted verbatim), the type-size scale emitted as `FontSize<Role>` tokens, and font delivery (embed DM Sans + Plus Jakarta Sans as OFL static/variable TTFs). See `CONVENTIONS.md` for the full rationale and `CHANGELOG.md` for per-version history.
