# Fonts — desktop byte-shipping guidance

Desktop apps can use **neither** of the existing CCSWE font-delivery
mechanisms: web loads via Google Fonts `@import`, Android via the
downloadable-fonts provider. An Avalonia desktop app must **ship font bytes**
embedded in the assembly. This note pins which families, where they go, how to
acquire them, and the license posture.

## Families to embed

| Family | Role | Upstream source (OFL) | File |
|---|---|---|---|
| **Plus Jakarta Sans** | brand — display / headline / titleLarge | `google/fonts` → `ofl/plusjakartasans` | `PlusJakartaSans[wght].ttf` (variable) |
| **DM Sans** | body — everything else | `google/fonts` → `ofl/dmsans` | `DMSans[opsz,wght].ttf` (variable) |

**Mono (DM Mono) / code (JetBrains Mono) are not vendored** — there is no
monospace surface in desktop UI yet. Add them the same way if a code/data
surface appears.

## Acquire with the bundled script

The bundle ships fetch scripts in `Assets/Fonts/` — run the one for your OS from
that folder. They pull the two variable TTFs **and** each `OFL.txt`:

```bash
cd CCSWE.Avalonia.Theme/Assets/Fonts
./fetch-fonts.sh           # macOS / Linux
# or
pwsh ./fetch-fonts.ps1     # Windows / cross-platform PowerShell
```

> **Variable fonts confirmed working** (consumer integration, Avalonia 12.0.3):
> the upstream variable TTFs weight-match by internal family name exactly as
> `Fonts.axaml` references them — `Plus Jakarta Sans` and `DM Sans`. (DM Sans's
> variable file carries an `opsz` axis; the family still resolves to `DM Sans`.)
> Variable is the recommended delivery — one file per family, no instancing step.

**Optional — static cuts.** If you prefer static per-weight instances (more
predictable on exotic rasterizers), instance them and keep the family name
constant across weights so Avalonia still weight-matches:

```
fonttools varLib.instancer "DMSans[opsz,wght].ttf" wght=400 -o DMSans-Regular.ttf
fonttools varLib.instancer "DMSans[opsz,wght].ttf" wght=500 -o DMSans-Medium.ttf
fonttools varLib.instancer "DMSans[opsz,wght].ttf" wght=700 -o DMSans-Bold.ttf
```

The M3 type scale uses: brand 600 (display/headline/title), body 400 (body),
body 500 (title-small / labels), 700 for emphasis.

## Where they go

```
CCSWE.Avalonia.Theme/Assets/Fonts/
├── fetch-fonts.sh / fetch-fonts.ps1   (acquisition scripts, shipped)
├── PlusJakartaSans[wght].ttf          (downloaded)
├── PlusJakartaSans-OFL.txt            (downloaded)
├── DMSans[opsz,wght].ttf              (downloaded)
└── DMSans-OFL.txt                     (downloaded)
```

Include the TTFs as `AvaloniaResource` in the csproj (see `HANDOFF.md`). The
emitted `Fonts.axaml` references them by **family name**, not file name:

```xml
<FontFamily x:Key="BrandFontFamily">avares://CCSWE.Avalonia.Theme/Assets/Fonts/#Plus Jakarta Sans</FontFamily>
<FontFamily x:Key="BodyFontFamily">avares://CCSWE.Avalonia.Theme/Assets/Fonts/#DM Sans</FontFamily>
```

The `#Family Name` fragment must match the font's internal family name exactly,
and **all files for one family share the directory** — Avalonia picks the weight
per requested `FontWeight`. Keep the family name constant across any static cuts
(don't ship "DM Sans Medium" as a distinct family) or Avalonia won't weight-match.

## License posture

Both families are **SIL Open Font License 1.1** — free to bundle and
redistribute inside an application, including commercial. The fetch scripts also
download each `OFL.txt`; keep them in the repo (or aggregate third-party licenses
in the app's about/licenses screen) to satisfy the OFL redistribution clause. No
UI attribution is required.
