# Third-party notices

`CCSWE.Avalonia.Material` embeds the following third-party assets. Their license
texts ship in the NuGet package under `THIRD-PARTY-NOTICES/` and in the repo under
`src/CCSWE.Avalonia.Material/Assets/`.

## Fonts

### DM Sans
- License: SIL Open Font License 1.1
- Upstream: https://github.com/google/fonts/tree/main/ofl/dmsans
- License text: `Assets/Fonts/DMSans-OFL.txt`

### Plus Jakarta Sans
- License: SIL Open Font License 1.1
- Upstream: https://github.com/google/fonts/tree/main/ofl/plusjakartasans
- License text: `Assets/Fonts/PlusJakartaSans-OFL.txt`

The SIL Open Font License 1.1 permits bundling and redistribution (including
commercial) inside an application or library, provided the license text
accompanies the font binaries. See <https://openfontlicense.org>.

## Icons

### Google Material Symbols
- License: Apache License 2.0
- Upstream: https://github.com/google/material-design-icons
- License text: `Assets/Icons/MATERIAL-SYMBOLS-LICENSE.txt`

All theme iconography is sourced from Google Material Symbols (Outlined, weight
400) and vendored as `StreamGeometry` path data in `Icons.axaml` (keyed
`Material_Icon*`). The Apache License 2.0 permits bundling and redistribution
(including commercial). Only the icons the theme uses are vendored, not the set.
