# CCSWE Avalonia Bundle — Conventions

Naming and placement rules the Avalonia emitter follows. Mirrors the Android
bundle's `CONVENTIONS.md`, adapted to AXAML / .NET. Read before editing an
emitter or adding a resource.

---

## Resource naming

### Color roles — paired `Color` + `SolidColorBrush`

Every M3 semantic role emits **two** resources:

| Resource | Key | Why |
|---|---|---|
| `SolidColorBrush` | bare PascalCase role (`Primary`, `OnSurfaceVariant`, `SurfaceContainerHigh`) | The common case — most Avalonia APIs (`Background`, `Foreground`, `BorderBrush`) want an `IBrush`. The bare name is the one you reach for. |
| `Color` | role + `Color` suffix (`PrimaryColor`, `OnSurfaceVariantColor`) | For the APIs that want a raw `Color` — `Pen`, `GradientStop`, `BoxShadow`, interop. The brush references this via `{StaticResource}`. |

This is the convention the consumer proposed in the reciprocal handoff and it
is now **pinned**. Rationale for the bare-name-is-brush direction: brush usage
outnumbers color usage at call sites ~10:1 in stock Avalonia markup, so the
shorter key goes to the common case.

Within a `ThemeDictionary`, all `Color`s are declared **before** the
`SolidColorBrush`es — `{StaticResource}` resolves top-down within a dictionary.

### Metrics

| Type | Key pattern | Examples |
|---|---|---|
| `CornerRadius` | `CornerRadius<ShapeStep>` | `CornerRadiusSmall`, `CornerRadiusExtraLarge`, `CornerRadiusFull` |
| spacing `double` | `Spacing<webKey>` | `Spacing4` (=16), `Spacing6` (=24) |
| spacing `Thickness` | `Spacing<webKey>Thickness` | `Spacing4Thickness`, `Spacing6Thickness` |

`Spacing<webKey>` keeps the web token's numbering (`spacing.4 = 16`), not the
pixel value, so web and desktop call the same step the same name.

### Control themes

`ControlTheme` resources are keyed `M3<Variant><Control>` —
`M3FilledButton`, `M3OutlinedButton`, `M3IconButton`. The `M3` prefix is this
bundle's equivalent of Android's `Core` prefix: it marks a control theme that
carries CCSWE design opinion (the M3 mapping of roles → template), as opposed
to a stock Fluent control theme. Each is paired with a convenience class
selector (`Button.Filled` → `Theme="{StaticResource M3FilledButton}"`).

### Style classes (type scale)

`TextBlock` type roles are PascalCase class selectors matching the M3 role name
exactly: `TextBlock.DisplayLarge`, `TextBlock.LabelMedium`. Applied via
`Classes="HeadlineSmall"`.

---

## File placement

| File | Holds | Kind |
|---|---|---|
| `Tokens.axaml` | color roles (Dark/Light) + metrics + disabled brushes | `ResourceDictionary` |
| `Fonts.axaml` | `FontFamily` resources | `ResourceDictionary` |
| `Motion.axaml` | duration (`sys:TimeSpan`) + easing (`SplineEasing`) resources | `ResourceDictionary` |
| `Typography.axaml` | type-scale style classes | `Styles` |
| `Controls/<Control>.axaml` | per-control `ControlTheme`s (in `Styles.Resources`) + class selectors | `Styles` |
| `Theme.axaml` | merges all of the above | `Styles` |
| `FluentOverrides.axaml`, `App.sample.axaml` | FluentTheme remap, app wiring | hand-authored, co-located at root |

All files emit at the **library project root** (no `Themes/` wrapper, no
`library-glue/` folder — the "emitted vs hand-authored" split is a docs concept,
not a directory). `Controls/` is kept as a grouping subfolder. The one-stop
include is `Theme.axaml` (not `CcsweTheme.axaml` — the assembly name already
carries `Ccswe`). Control files: `Button.axaml`, `TextBox.axaml`,
`CheckBox.axaml`, `RadioButton.axaml`, `ToggleSwitch.axaml`, `ListBox.axaml`,
`ComboBox.axaml`, `Menu.axaml`, `Expander.axaml`, `Card.axaml`,
`Slider.axaml`, `ProgressBar.axaml`, `TabControl.axaml`, `TabStrip.axaml`,
`TreeView.axaml`, `AutoCompleteBox.axaml`, `ToggleButton.axaml`,
`NumericUpDown.axaml`.

**Popup chrome is themed in-control.** Controls with popups (ComboBox dropdown,
MenuFlyout, ContextMenu, MenuItem submenu) paint their own popup surface
(`SurfaceContainer` + radius + elevation shadow) inside their ControlTheme rather
than relying on a global remap of Fluent's base `SystemControl*` brushes. This
keeps blast radius at zero and is the bundle's standing answer for popup surfaces
(see `FLUENT-AUDIT.md`). `Card` is the exception to the ControlTheme rule — there
is no stock `Card`, so it ships as `Border.Card` *style classes*, not a
ControlTheme. **`Card` is a reserved class name** as of 1.3.0 — a consumer's
pre-existing `Border.Card` style will be overridden by the theme (`Card` alone =
Elevated); rename any local card class to avoid the clash.

**Resources vs. Styles.** Color/brush/metric/font values are *resources*
(`ResourceDictionary`) — they are looked up. Type classes and control themes
are *styles* (`Styles`) — they are applied by selector. `Theme.axaml`
bridges the two: it merges the resource dictionaries into its `Styles.Resources`
and `StyleInclude`s the style files.

**Every `ControlTheme` goes inside `<Styles.Resources>`** — a `ControlTheme` is a
keyed resource and is invalid as a direct child of `<Styles>` (build error
`AVLN3000: Unable to find suitable setter or adder ... for ControlTheme`). This
holds whether the file keys on `{x:Type Control}` (default theme) or on a named
key + class selector (multi-variant). Emit consistently.

---

## Token resolution at emit time

Same model as the CSS and Kotlin emitters: resolve `ref`/`value` from
`tokens.upstream-1.1.0.json` at generation time and emit concrete hex. A role
with `{ "ref": "surface.950" }` emits the resolved `#FF020617`; a role with
`{ "value": "#172238" }` emits it directly. The Avalonia bundle consumes the
**master** semantic layer verbatim — it does **not** apply the Android-side
overrides (e.g. Android remaps `inverseSurface` onto the slate ramp; desktop
keeps the master's gray-ramp value).

### Color format

Avalonia `Color` literals are emitted as `#AARRGGBB` with an explicit `FF`
alpha (`#FF659EC7`), never `#RGB` or `#RRGGBB` — explicit alpha avoids any
ambiguity and matches how `Color.Parse` round-trips.

---

## Resolved cross-platform questions (this cycle)

| Question (from the consumer) | Resolution |
|---|---|
| **letterSpacing** em→DIP | Emit the upstream value **as an absolute DIP, verbatim**. `TextBlock.LetterSpacing` is an absolute DIP offset; the upstream "em" label is a misnomer — the figures are M3's published per-role tracking, applied as absolute by every consumer (Android passes them to Compose as `.sp`). `em × fontSize` is what produced the too-wide tracking and is **wrong**. |
| **spacing** scale | Adopt the **web 4px scale verbatim** (`0,4,8,…,96`), emitted as `double` + `Thickness`. Desktop and web now share one neutral-density ramp. Adaptive desktop tiers deferred. |
| **resource naming** | `Color` + `Brush` pair, bare-PascalCase brush / `…Color` color. Pinned. |
| **fonts** | Embed DM Sans + Plus Jakarta Sans as OFL **static-weight** TTFs, referenced via `avares://` `FontFamily`. See `FONTS.md`. |
| **HC schemes / variant accents** | Encoded upstream, **not emitted** — no desktop consumer yet. |

---

## DynamicResource vs. StaticResource

This was a correctness landmine in the first cut and the rule is now firm:

- **Inside emitted `ControlThemes`, every token reference uses `DynamicResource`** —
  color roles, metrics, AND motion. Two reasons: (1) a `ControlTheme` setter that
  binds a role brush with `StaticResource` **freezes** that brush at parse time,
  so the control keeps its Dark colors when the `ThemeVariant` flips to Light —
  a real bug; `DynamicResource` re-resolves on the switch. (2) `DynamicResource`
  resolves against the merged application resources at runtime, so cross-file
  refs (a `ControlTheme` in `Controls/Button.axaml` reaching a brush in
  `Tokens.axaml`) resolve reliably regardless of merge/parse order. This is the
  same choice Avalonia's own Fluent theme makes.
- **`StaticResource` only for same-file structural refs**: `BasedOn="{StaticResource
  M3ButtonBase}"` and the convenience `Theme="{StaticResource M3FilledButton}"`
  assignments — both reference `ControlTheme` resources in the *same file's*
  `Styles.Resources`, resolved at parse time, and neither varies by theme.
- **Consumer ad-hoc markup**: prefer `DynamicResource` for role brushes/colors
  (so they track variant switches); `StaticResource` is acceptable for the
  theme-invariant metrics (`CornerRadius*`, `Spacing*`, `Motion*`).

---

## Motion (`Motion.axaml`)

- 10 durations emitted as `sys:TimeSpan` resources (`MotionDurationShort1` …
  `MotionDurationLong2`); both `Animation.Duration` and `Transition.Duration` are
  `TimeSpan`-typed in Avalonia, so they bind directly.
- 6 easings emitted as `SplineEasing` resources (`MotionEasing*`) built from the
  upstream cubic-bezier control-point arrays (`X1 Y1 X2 Y2`).
- Reference via `DynamicResource` inside `ControlThemes`/transitions (same rule
  as above); `StaticResource` is fine in a standalone `<Animation>` in app markup.

---

## Control themes — full templates, own part names

The emitted `ControlThemes` (`Controls/*.axaml`) ship **complete templates** with
their own part names rather than `BasedOn` Fluent's control themes — so they
don't depend on Fluent's internal resource keys (which aren't a stable contract;
see `FLUENT-AUDIT.md`). Where Avalonia's control logic requires specific lookup
names, those are honored: `PART_BorderElement` / `PART_ScrollViewer` /
`PART_TextPresenter` / `PART_Watermark` (TextBox), and the standard
`:checked` / `:indeterminate` / `:focus-within` / `:error` pseudo-classes.

Default-themed controls (CheckBox, RadioButton, ToggleSwitch) key their
`ControlTheme` on `{x:Type Control}` so they apply with no class. Multi-variant
controls (Button, TextBox) ship keyed `ControlTheme`s + convenience class
selectors (`Button.Filled`, `TextBox.Outlined`).

### Required template parts (Avalonia 12)

Some controls mark template parts `[TemplatePart(IsRequired)]` and **fail to
build** if the template omits them (`AVLN2205: Required template part … must be
defined`). `ToggleSwitch` requires `PART_SwitchKnob` + `PART_MovingKnobs`. When a
custom M3 template drives visuals through its own elements, still include the
required parts — as inert `IsVisible="False"` placeholders if the control's own
positioning logic isn't used. Audit every emitted template against its control's
Avalonia 12 part contract.

### Neutralize framework-default content the ControlTheme would surface

When a ControlTheme emits a presenter for a control property that Avalonia
**pre-populates with a non-empty default**, neutralize that default in the theme or it
renders unbidden. `ToggleSwitch` defaults `OnContent="On"` / `OffContent="Off"`, so once
the theme adds `PART_On/OffContentPresenter` (1.5.3) **every** M3 switch leaked that text —
a switch with `Content="Light mode"` read **"Light modeOff"** (1.5.3 regression, fixed 1.5.4).
The theme sets `OnContent=""` / `OffContent=""` so the presenters render nothing unless a
consumer explicitly sets them (M3 switches carry no on/off labels). **Generalize:** any
framework default that would surface through an emitted presenter (`OnContent`/`OffContent`,
etc.) must be reset to empty in the ControlTheme.

### `Watermark` is obsolete — emit `PlaceholderText` (standing no-go)

`Watermark` is deprecated in Avalonia 12 on **`TextBox`** and **`AutoCompleteBox`**;
always emit `PlaceholderText` on those. This regressed twice — `TextBox` (fixed
1.2.0) then `AutoCompleteBox` (fixed 1.5.1, `AVLN5001`) — so treat it as a hard
emitter rule: **flag any emitted control template that references `Watermark`
(attribute or `TemplateBinding`) on a type where Avalonia 12 deprecated it.** The
replacement is always `PlaceholderText`. **Exception:** `NumericUpDown.Watermark` is
**not** deprecated — `PlaceholderText="{TemplateBinding Watermark}"` on its inner
field compiles clean and is left as-is. So target the deprecated *declaring types*
(don't blanket-replace), or prefer `PlaceholderText` wherever the control exposes it.

### Clone only the setters the target type actually has

When cloning a ControlTheme onto a sibling control, drop setters for properties the
new target doesn't declare. The slip to avoid (1.5.1, `AVLN2000`): `TreeViewItem`
was cloned from `ListBoxItem` and inherited `VerticalContentAlignment` — but
`ListBoxItem` is a `ContentControl` while `TreeViewItem` is a `HeaderedItemsControl`
→ `ItemsControl`, which has **no** `Vertical/HorizontalContentAlignment`. Clones
across a different base class must shed the base-only setters (and any matching
`TemplateBinding`); align the header `ContentPresenter` directly instead. Also:
`TextBox.Watermark` is obsolete in 12 — emit `PlaceholderText` (see above).

**Same trap on a host `Style`, not just a cloned `ControlTheme` (1.7.1, `AVLN2000`):**
`Style Selector="ListBox.NavigationRail"` set `HorizontalContentAlignment="Center"`
directly on the `ListBox`. `ListBox` is a `SelectingItemsControl` → `ItemsControl`
— `Horizontal/VerticalContentAlignment` is a `ContentControl` property it doesn't
have, so it hard-fails the build (same class as the `TreeViewItem` slip above). Push
content alignment **onto the item** instead: the `M3NavigationRailItem`
(`ListBoxItem`, a `ContentControl`) sets `HorizontalContentAlignment="Center"` and
the template centers via `HorizontalAlignment`/`TextAlignment`. The sibling
`ListBox.NavigationDrawer` host correctly never set it.

**Emitter lint:** flag any `Horizontal/VerticalContentAlignment` setter emitted onto
an `ItemsControl`-derived target (`ListBox`, `TabStrip`, `ItemsControl`, `TreeView`,
`Menu`, …) — whether in a cloned `ControlTheme` or a host `Style`. These properties
exist only on `ContentControl`/`ItemsControl`-items, never the items host.

### Disabled fills — themed alpha brush, never element `Opacity`

A disabled *fill* (M3: container `OnSurface` @ 12%, content @ 38%) must be a
**brush with alpha**, not element `Opacity` on a Border that also contains the
content — element opacity composites onto children, dimming the label twice and
ghosting it. And do **not** use an inline `<SolidColorBrush Color="{DynamicResource
…Color}">` inside a `Setter` — `DynamicResource` on a nested object's property
doesn't reliably resolve and the brush falls back to transparent (the fill/border
*vanish*). Instead reference a pre-built themed brush emitted in `Tokens.axaml`:
`OnSurface12` (disabled container fill / outline) and `OnSurface38` (disabled
content), each `<SolidColorBrush Color="{StaticResource OnSurfaceColor}"
Opacity="…">` inside the theme dictionary, consumed as a plain
`Value="{DynamicResource OnSurface12}"`.

For disabled **content** specifically: set the ContentPresenter's
`TextElement.Foreground` to `OnSurface38` (a *recolor*), don't dim the variant's
own colored foreground with `Opacity="0.38"`. Dimming the colored brush reads as a
faint brand-tinted label on the dark scheme (a disabled Text/Outlined button became
faint brand-blue), and compounds to ~14% if combined with the alpha token. The M3
intent is a single neutral `OnSurface @ 38%` for disabled content across *all*
variants, independent of the enabled foreground. (1.4.1.)

**This is an emitter-wide primitive, applied uniformly to every control — not a
per-control decision.** As of 1.5.3 no emitted ControlTheme uses a root
`:disabled { Opacity=0.38 }`; each recolors its own elements (container/fill/outline →
`OnSurface12`, content/label/glyph/indicator → `OnSurface38`). The earlier whole-element
dim made a disabled *checked/selected* control (checkbox, switch, tab, list row) read as
a faded brand tint instead of neutral M3 gray. **Emitter lint: flag any root `Opacity`
setter inside a `:disabled` selector.** Where the visible chrome is an *inner* control
(AutoCompleteBox's field, NumericUpDown's spinner), let that inner control's own disabled
recolor carry it and emit **no** outer dim (an outer dim would double-apply). To recolor
text whose presenter sets its own `Foreground` (an embedded field), target the
`TextPresenter`/`TextBlock` directly — an outer `TextElement.Foreground` won't reach it.
**When that target is a `TextPresenter`, set `TextElement.Foreground`, never the bare
`Foreground`** — a `TextPresenter` has no `Foreground` AvaloniaProperty (`Property="Foreground"`
throws `AVLN3000: Foreground is not an AvaloniaProperty`); it renders text via the attached
`TextElement.Foreground`. Only true `TextElement`s (`TextBlock`) and `TemplatedControl`s expose
a real `Foreground`. (Same idiom already used on `ContentPresenter` disabled-label recolors;
the 1.5.3 disabled pass just missed applying it to the `TextPresenter` targets — fixed 1.5.4.)
**Emitter lint:** flag `Property="Foreground"` in any style whose selector targets a
`TextPresenter` (or a `ContentPresenter` content recolor) — it must be `TextElement.Foreground`.

### State changes must not reflow content — overlay vs. container borders

A `:checked` / `:focus-within` / `:error` selector that changes **`BorderThickness`** is
safe **only** when the border does not participate in the content's layout. Two shapes:

- **Overlay border (safe):** the border element is a *sibling* of the content (both inside
  a `Panel`/`Grid`, overlapping), so the content is positioned by its own margin/padding
  and thickening the border is **paint-only** — nothing moves. `TextBox` (Outlined/Filled),
  `ComboBox`, and (as of 1.5.3) `ButtonSpinner`/`NumericUpDown` use this; it's why their
  1→2px focus emphasis doesn't shift the field.
- **Container border (unsafe):** the content sits *inside* the border element, so the
  border's thickness insets the content; changing it by state **reflows/jumps** the control.

**Rule:** any ControlTheme with a `BorderThickness` setter inside a state selector must use
the **overlay** structure — *or* keep thickness constant and change only `BorderBrush`.
The two valid fixes (1.5.3): `ToggleButton`'s `:checked` pill keeps 1px and recolors the
border to `Transparent` (constant-thickness container); the spinner field restructured to
the overlay shape so its visible resting border can thicken on focus. (Grep every
`BorderThickness` setter under a state selector and verify the target isn't a container border.)

### Slider — active fill on the RepeatButton background, not nested content

The M3 slider rail is layered: a dedicated full-length inactive-rail `Border`
behind `PART_Track` (using `SurfaceContainerHighest` — `SurfaceVariant` is
near-invisible on the dark `Surface`), with the **active fill painted by the
`DecreaseButton`'s own `Background`** (a small themed RepeatButton with an explicit
4dp cross-axis size + a trivial `Border` template), and a transparent
`IncreaseButton` so the rail shows through the inactive side. Do **not** rely on
nested RepeatButton *content* to be the fill — it collapses on the cross axis and
won't render over the rail (cost two cycles, 1.4.1→1.4.2). Track stacks after the
rail so fill + thumb sit on top. Same pattern, axis-swapped, for vertical.

### DataValidationErrors — theme the message to M3 `Error`

Any TextBox template that wraps its content in `<DataValidationErrors>` must also
emit a `DataValidationErrors` `ControlTheme` (rides inside `Controls/TextBox.axaml`)
that recolors the error message to the M3 **`Error`** brush (bodySmall: 12/16,
0.4 tracking, below the field). Without it the message presenter falls back to
Fluent's default error brush (yellow), mismatching the red `:error` border /
indicator. The `:error` *border* color is a separate selector on
`PART_BorderElement` / `PART_Indicator` — both must be `Error` for a coherent M3
error state. (1.4.2.)

### Composite controls — assemble from emitted patterns, opt parts out of defaults

Tier-1 controls (1.5.0) are built by reusing patterns the bundle already ships
rather than reinventing chrome. Two cross-cutting rules came out of it:

- **A nested control that is a template *part* must opt out of its own default
  ControlTheme.** Once `ToggleButton.axaml` themes `{x:Type ToggleButton}` as the
  M3 pill, every ToggleButton inherits it — including the `TreeViewItem`'s
  `PART_ExpandCollapseChevron`. Parts give themselves a **local `Template`** (the
  chevron) or a keyed `Theme="{StaticResource …}"` (the `NumericUpDown` spinner
  RepeatButtons via `CcsweSpinnerButton`) so they don't pick up the pill. The
  default ControlTheme's `/template/` state selectors then simply don't match the
  part's own template — no conflict, but the opt-out must be explicit.
- **Embed a borderless field, don't nest two borders.** A field-bearing composite
  (`NumericUpDown`) draws its single M3 Outlined box on the **outer** chrome owner
  (the `ButtonSpinner`'s `PART_BorderElement`, `:focus-within` → 2px `Primary`),
  and the inner `TextBox` uses a transparent, borderless theme
  (`CcsweEmbeddedTextBox`) — **not** `Classes="Outlined"`, which would double the
  border. `AutoCompleteBox` is the opposite case: it has no outer chrome owner, so
  its inner `PART_TextBox` *does* take `Classes="Outlined"` and owns the box.
- **An items-host *part* that must DISPLAY items has to be a templated,
  container-generating type — use `ListBox`, never a bare `SelectingItemsControl`.**
  The abstract/base items types (`SelectingItemsControl`, `ItemsControl` as a literal
  element) have **no default `ControlTheme`** in FluentTheme, and Avalonia does no
  hierarchical ControlTheme fallback — so a bare one gets no template and renders an
  **empty** host (compiles clean; only a runtime visual pass catches it). For
  `AutoCompleteBox`'s `PART_SelectingItemsControl`, emit a `ListBox` (`Background`
  transparent, `BorderThickness="0"` so the popup's `SurfaceContainer` reads through):
  `ListBox` *is-a* `SelectingItemsControl`, satisfies the part cast /
  `SelectingItemsControlSelectionAdapter`, has a working template, and generates the
  `ListBoxItem` rows the M3 list-item treatment relies on. (1.5.2.)

- **A composite control whose chrome lives in an *inner* control must forward its
  validation/`:error` state to that inner control** — the outer control's pseudo-classes
  don't reach the inner one. `NumericUpDown` carries the `Value` validation, but the visible
  border is the inner `ButtonSpinner`'s, so the outer `:error` never matched. Fix (1.5.3):
  forward it as a bound style class —
  `Classes.error="{Binding (DataValidationErrors.HasErrors), RelativeSource={RelativeSource TemplatedParent}}"`
  on the inner control — and have the inner control's error rule match both `:error` and
  `.error`, placed **last** among the border rules so red wins over the focus color. Applies
  to any future spinner/picker that delegates its border to a child.

- **A "current selection" summary surface must forward BOTH the item and its template.**
  A control that renders the selected item separately from its item containers (`ComboBox`'s
  closed field) must bind `Content="{TemplateBinding SelectionBoxItem}"` **and**
  `ContentTemplate="{TemplateBinding SelectionBoxItemTemplate}"`. Forwarding only the value
  silently drops a custom `ItemTemplate` on the collapsed field (it falls back to
  `ToString()`) while the dropdown rows render correctly — the two views disagree. Null-safe:
  with no `ItemTemplate`, `SelectionBoxItemTemplate` is null and behavior is unchanged. (1.5.3.)

### TreeView indentation — Avalonia's public `MarginMultiplierConverter`

Per-level indentation is a left `Margin` on the row bound to `TreeViewItem.Level`
through `Avalonia.Controls.Converters.MarginMultiplierConverter` (public, assembly
`Avalonia.Controls`) — the same converter Fluent uses. Declare it as a keyed
resource (`Indent="24" Left="True"` for M3's 24dp/level) and reference it
`StaticResource` (theme-invariant, same-file). This keeps indentation token-driven
with **no custom C# converter** to ship — important, since the DS emits axaml only.
The chevron is a `PART_ExpandCollapseChevron` ToggleButton (`IsChecked` two-way to
`IsExpanded`); rotate it via the TreeViewItem's `:expanded` pseudo-class targeting
the part directly (`^:expanded /template/ ToggleButton#PART_ExpandCollapseChevron`),
not a selector reaching into the chevron's own nested template. (1.5.0.)

### Theming a control you don't template — `Style`, not a replacing `ControlTheme`

To set token-driven **property defaults** on a control whose template you do *not*
own (FluentTheme already ships it), use a **`Style Selector="Type"`**, not a
`ControlTheme x:Key="{x:Type Type}"`. A `ControlTheme` keyed on the type **replaces**
the framework's whole theme — template included — and without a `BasedOn` to the base
theme's (often internal, unreferenceable) key you lose the template and the control
renders blank. A type `Style` **layers** setters over the existing template,
non-destructively: instance/local values still win, so M3 defaults stay overridable.
This is how `Controls/DrawerPage.axaml` colors the pane (`DrawerBackground` =
`SurfaceContainerLow`), wires the modal scrim (`BackdropBrush` = `Scrim32`), and sets
the M3 pane/rail widths (`DrawerLength=360` / `CompactDrawerLength=80`) without
re-emitting `DrawerPage`'s 9-part template. **Reserve a (keyed or `{x:Type}`)
`ControlTheme` for when you actually emit the template.** Before choosing, verify the
property names and whether the parts are `IsRequired` against the Avalonia source —
`DrawerPage`'s parts are all non-required (it null-guards every `Find<>`), so layering
needs none of them. (1.6.0.)

### Item-treatment variants — keyed `ControlTheme` + `ItemContainerTheme`

A second look for an items control's containers (a nav-drawer destination vs. a plain
list row) ships as a **keyed** `ControlTheme` (`x:Key="M3NavigationDrawerItem"`,
`TargetType="ListBoxItem"`) applied by the host via **`ItemContainerTheme`** — never a
second `{x:Type ListBoxItem}` (that key is owned by `ListBox.axaml`; a duplicate
default collides and one silently wins). The host opts in with a class selector
(`ListBox.NavigationDrawer`) whose `Setter` points `ItemContainerTheme` at the keyed
theme. Same pattern for any `ItemsControl` variant. (1.6.0.)

### Multi-element item state — recolor on separate targets, and `Content`/`Tag` to split a `ListBoxItem`

Two rules from the nav **rail** (`M3NavigationRailItem`, 1.7.0), the drawer item's
compact sibling:

- **When an item's active state colors two sub-elements *differently*, you can't reuse a
  single `TextElement.Foreground` cascade — recolor each on its own target.** The
  drawer item gets away with one `^:selected /template/ ContentPresenter` foreground
  setter because icon **and** label both go `OnSecondaryContainer` (a `PathIcon` paints
  from the inherited `Foreground`, so one cascade does both). The rail diverges: active
  icon = `OnSecondaryContainer` but active **label = `OnSurface`**. So the rail template
  keeps the icon in a `ContentPresenter` (recolored via `TextElement.Foreground`) and
  the label in a **separate** named `TextBlock` (recolored via its own `Foreground`),
  each with its own `^:selected` setter. Don't force a single cascade when the spec
  colors the parts apart.
- **To split one `ListBoxItem` into two themed slots without C#, use `Content` + `Tag`.**
  `ListBoxItem` has a single `Content`; the DS emits axaml only (no code-behind to add a
  second content property). When a variant needs the theme to position/recolor two
  consumer-supplied pieces independently (rail: icon inside the indicator, label below
  it), put the primary in `Content` and the secondary string in `Tag`, and bind
  `Text="{TemplateBinding Tag}"` in the template. `Tag` is stringly-typed (no
  `HeaderTemplate`) — fine for a short label; flag the contract in HANDOFF so consumer
  markup is stable. (1.7.0.)

### Layering a `Style` over a template you don't own — only reaches *existing* properties

The `Style Selector="Type"` layering technique (above) can set **only properties the
control actually declares**. It cannot inject template structure. So when a feature
needs chrome the control exposes no property for, layering can't deliver it — and
reaching into the framework template's internal parts by name
(`Style Selector="T /template/ Border#PART_…"`) to fake it is a **standing no-go**: it
depends on Fluent's internal part names, which aren't a stable contract (the same reason
the bundle ships full templates instead of `BasedOn` Fluent — see "Control themes — full
templates"). The worked case (1.7.0): the M3 docked nav drawer/rail wants a 1dp
`OutlineVariant` **trailing-edge divider**, but `DrawerPage` exposes **no** border
property (verified: pane/header/footer `Background` + `*Foreground` + `BackdropBrush` +
lengths only; its `SplitView` base has `PaneBackground`, no pane-border brush). Decision:
**request the missing property upstream** (`DrawerBorderBrush`/`DrawerBorderThickness`)
so a future `Style` setter delivers it as one container-level concern — *don't* reach
into the pane-root part, and *don't* let each consumer hand-roll it differently (drift).
Interim, document **one** consumer-side pattern (a 1dp `Border` between pane and content,
docked modes only) in HANDOFF. **Rule:** if a `Style`-layered control lacks the property
you need, the answer is an upstream property request + a single documented interim — not
a `/template/` reach into framework internals. (1.7.0.)

### A `Style` setter loses to the framework `ControlTheme`'s setter for the same property — remap the token instead

The flip side of the rule above. Layering a `Style` over a template you don't own reaches
only properties the control declares — but even for those, a `Style` setter takes effect
**only if Fluent's own `ControlTheme` does not already set that property.** Avalonia value
precedence puts a **ControlTheme `Setter` above an app-level `Style` `Setter`**; a local
(per-instance) value beats both, but a `Style` setter is *not* a local value. So
`Style Selector="DrawerPage"` wins `DrawerBackground` (Fluent's ControlTheme sets no such
setter) yet **silently loses `Background`** (Fluent's `DrawerPage` ControlTheme sets it to
`SystemControlPageBackgroundAltHighBrush`). The worked case (1.7.2→1.7.3): a
`Style`-set `Background="{DynamicResource Surface}"` to fix DrawerPage's black content slot
was a **no-op** — verified by pixel-sampling, the slot stayed `#000000`. Because the Fluent
template binds the content presenter to its `Background` via `{DynamicResource …}`, the fix
is to **remap the underlying token, not the property**:
`SystemControlPageBackgroundAltHighBrush` → `Surface` in `FluentOverrides.axaml` (Dark+Light)
resolves regardless of precedence. **Rule:** before adding a `Property=` setter to a
`Style`-layered control, check whether Fluent's ControlTheme already sets that property
(read the Avalonia source). If it does, a `Style` setter is dead code — remap the
`DynamicResource` token the framework setter points at, in `FluentOverrides.axaml`, scoping
to the narrowest-blast-radius key (here the *Page*-specific
`SystemControlPageBackgroundAltHighBrush`, ref'd only by the 4 `*Page` shells — **not** the
broad `SystemControlBackgroundAltHighBrush`). (1.7.3.)
