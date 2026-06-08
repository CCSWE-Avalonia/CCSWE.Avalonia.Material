# Debugging / visually verifying the demo under WSLg (screenshot-driven)

The Demo (`CCSWE.Avalonia.Material.Demo`) is the visual verification harness, and it runs under
**WSLg** (`DISPLAY=:0`), so a headless agent can launch it, **drive it with synthetic input**, and
screenshot it. This is the only reliable way to catch **runtime layout/XAML bugs** — a clean
`dotnet build` does *not* catch them:

- bad bindings, missing animators, unresolved `DynamicResource` keys, and layout overflow surface
  only at runtime;
- "can't scroll to the bottom", "shadow clipped", "state layer touches the edge", wrong alignment
  are **visual** — you have to *see* the rendered window.

> **Methodology — isolate, don't guess.** When a layout/visual bug appears, don't stack speculative
> fixes. Reduce to the smallest repro and change **one** property at a time, screenshotting after
> each. (That's how the CommandBar vertical-padding and the FAB shadow issues were pinned.) One
> variable per screenshot.

For a fully deterministic, no-display alternative (CI), use headless `CaptureRenderedFrame` — see
[§7](#7-headless-alternative-no-display).

---

## 1. Prerequisites

WSLg is live when `DISPLAY=:0` and these tools exist (all present on this machine):

- `xdotool` — find/activate windows, warp the pointer, synthesize clicks + scroll-wheel events
- ImageMagick `import` / `convert` — capture a specific X window to PNG; check it isn't blank
- `dotnet` — build/run the app

Quick check: `DISPLAY=:0 xdotool getdisplaygeometry` should print the screen size.

---

## 2. Build & launch

```bash
dotnet build src/CCSWE.Avalonia.Material.slnx -c Release -v q --nologo

# Launch in the BACKGROUND (run_in_background) so the agent keeps control.
# DEMO_PAGE jumps straight to a gallery page on startup (nav order: 0 Typography, 1 Buttons,
# 2 Inputs, 3 Selection, 4 Collections, 5 Feedback, 6 Tabs, 7 Containers, 8 Coverage).
DISPLAY=:0 DEMO_PAGE=1 dotnet run --project src/CCSWE.Avalonia.Material.Demo -c Release
```

Teardown between runs:

```bash
pkill -f '[C]CSWE.Avalonia.Material.Demo'
```

The `[C]` bracket keeps the regex from matching the `pkill` process's own argv (it contains the
literal `[C]CSWE…`, which the pattern doesn't match). This command "fails" with **exit 144** when it
kills the background task — that's the expected teardown, not an error. **Do not** run a bare
`kill`/`pkill` in the *same* command that captures: the signal hits that command's own process group
and kills your capture too.

---

## 3. Find the window (the reliable way)

`wmctrl` / `_NET_CLIENT_LIST` are unreliable under WSLg ("Cannot get client list properties"). Use
`xdotool search` and filter by the **exact** window name — the substring can match stale windows
from earlier launches, and ids change between runs:

```bash
export DISPLAY=:0
timeout 60 xdotool search --sync --name "CCSWE.Avalonia.Material.Demo" >/dev/null 2>&1   # wait for it
wid=""
for w in $(xdotool search --name "CCSWE.Avalonia.Material.Demo" 2>/dev/null); do
  [ "$(xdotool getwindowname "$w" 2>/dev/null)" = "CCSWE.Avalonia.Material.Demo" ] && wid=$w && break
done
echo "wid=$wid"
xdotool windowactivate --sync "$wid"; xdotool windowraise "$wid"
```

The window must be **front-most** for `import` to grab real pixels; an occluded/duplicate window
grabs black. If multiple ids match, you left stale windows open — `pkill` (above) and relaunch.

---

## 4. Wait for it to actually paint

The window exists (step 3) **~1s before it renders**, so capturing immediately grabs a blank frame.
Foreground `sleep` is blocked in this sandbox — gate on real content instead of a fixed delay
(`read -t` is the allowed settle; `sleep 0.5` works only if your shell permits it):

```bash
for i in $(seq 1 30); do
  read -t 1 _ </dev/null || true
  import -window "$wid" /tmp/shot.png 2>/dev/null
  m=$(convert /tmp/shot.png -format "%[mean]" info: 2>/dev/null | cut -d. -f1)
  [ "${m:-0}" -gt 200 ] && break        # painted (a dark M3 page is ~5000–6000; ~0 = blank placeholder)
done
```

---

## 5. Click and scroll (synthetic input — it works)

Synthetic input **does** reach WSLg windows, but the pointer must be **physically warped** to
absolute screen coordinates first (`--window` targeting is not enough for wheel events). The nav
drawer occupies the left ~360px; the gallery content is to the right.

```bash
# Click a nav destination or a control (absolute coords; left drawer ≈ x<360, content ≈ x>380):
xdotool mousemove 90 218; xdotool click 1          # e.g. a left-nav item
# Scroll a tall gallery page to the bottom — warp into the content area, then wheel.
xdotool mousemove 700 400                          # button 5 = wheel down, 4 = wheel up
xdotool click --repeat 200 --delay 5 5             # 200 ticks → guaranteed bottom
```

Use a high repeat count (200+) so you reach the true extent — that distinguishes "content ends here"
from "scrolling is stuck". A gap below the scrollbar thumb after a big scroll = a real bug. After a
*manual* scroll, re-warp and re-issue the ticks; don't trust the prior position.

(Prefer `DEMO_PAGE` to reach a page; use clicks for in-page interaction — toggles, theme switch
bottom-left, drawer↔rail collapse — and the wheel for below-the-fold content.)

---

## 6. Capture and read back

```bash
import -window "$wid" /tmp/shot.png
```

Then **Read `/tmp/shot.png`** to see the rendered UI. Capture at each step of an isolation experiment
and compare. Always sanity-check with the `%[mean]` value (§4) — ~0 means you grabbed a blank/black
placeholder, not a real frame.

---

## 7. Headless alternative (no display)

When you don't want a live window (CI, determinism), render off-screen with Avalonia's Skia
rasterizer — bypasses X11/Wayland entirely. `src/CCSWE.Avalonia.Material.UnitTests` is a working
`Avalonia.Headless.NUnit` harness; the only change for a screenshot is disabling the stub renderer:

```csharp
AppBuilder.Configure<TestApp>()
    .UseHeadless(new AvaloniaHeadlessPlatformOptions { UseHeadlessDrawing = false });
// ...
var window = new Window { Content = view, Width = 1280, Height = 800 };
window.Show();
using var frame = window.CaptureRenderedFrame();   // WriteableBitmap
frame!.Save("/tmp/shot.png");                       // then Read it
```

Give the window an explicit size (headless windows are 0×0 otherwise); set
`Application.Current.RequestedThemeVariant = ThemeVariant.Light` before capture to shoot Light.

---

## 8. Footguns (all hit at least once)

| Symptom | Cause / fix |
|---|---|
| `pkill -f CCSWE…Demo` exits 144 / kills the capture | pattern matches the `pkill`/capture process itself — use `pkill -f '[C]CSWE.Avalonia.Material.Demo'`, and never `kill` in the same command that captures |
| grab is black / 233-byte 1024×768 grayscale (mean 0) | grabbed before paint, or window not front-most (stale duplicate windows) — poll on `%[mean]` (§4); ensure one window, `windowactivate --sync` + `windowraise` |
| `Cannot get client list properties (_NET_CLIENT_LIST)` | `wmctrl` fails under WSLg; use `xdotool search --name` |
| `wid=` empty / wrong window | filter by exact `getwindowname`, not the substring |
| wheel ticks do nothing | pointer wasn't warped onto the content — `xdotool mousemove X Y` first, then `click 5` |
| `import -window root` fails ("unable to read X window image root") | grab a specific window id, not the root |
| heredocs / foreground `sleep` blocked | use the Write tool / `printf`; gate waits with `timeout` + `xdotool --sync` and `read -t` |

---

## 9. Known layout rules these tools proved

(Discovered by consumers debugging against this library — worth honoring in the page-shell themes.)

- **Page inset goes on the scrolled content's `Margin`, never `ScrollViewer.Padding`.** Avalonia's
  `ScrollContentPresenter` leaves the bottom padding *outside* the scrollable extent, so padding makes
  the last items permanently unreachable. Put the inset on the inner `ItemsControl`/`StackPanel`
  `Margin`; the `ScrollViewer` stays padding-free and the scrollbar stays at the window edge.
- **A scrollable screen in `DrawerPage` must host its `ScrollViewer` in a `Panel`**, not a
  `DockPanel` — a `Panel` force-fills the `ScrollViewer` to a bounded viewport height; a `DockPanel`
  doesn't, so the scroll area runs taller than the window and can't reach bottom.
