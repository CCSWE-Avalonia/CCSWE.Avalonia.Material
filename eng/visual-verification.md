# Visually verifying the demo (headless / agent-driven)

The Demo (`CCSWE.Avalonia.Material.Demo`) is the visual verification harness. An automated
agent (or a headless/WSL session) can't *watch* a live window — to verify appearance you
**render the UI to a PNG and read that file.** Two ways, in order of reliability.

## A. Live WSLg window grab (what to use on this dev box)

Launch the demo, find its X11 window id, screenshot it. Synthetic input doesn't reach WSLg
windows, so use the `DEMO_PAGE` env var to jump straight to a gallery page on startup (see
`MainWindow.axaml.cs`; the index matches the nav order — 0 Typography, 1 Buttons, 2 Inputs, …).

```bash
export DISPLAY=:0

# 1. Launch detached, pointed at the page you want.
setsid env DEMO_PAGE=1 dotnet run --project src/CCSWE.Avalonia.Material.Demo -c Release >/tmp/demo.log 2>&1 &

# 2. Wait for the window (--sync blocks until it exists).
WID=$(timeout 150 xdotool search --sync --onlyvisible --name "CCSWE.Avalonia.Material.Demo" | tail -1)

# 3. Bring it fully to the front (import only grabs real pixels from a front-most window).
xdotool windowactivate --sync "$WID"; xdotool windowraise "$WID"

# 4. Poll until it has PAINTED. The window exists ~1s before it renders, so a fixed sleep
#    grabs a blank frame. Grab once a second until the image has content, then stop.
for i in $(seq 1 30); do
  read -t 1 _ </dev/null || true
  import -window "$WID" /tmp/shot.png 2>/dev/null
  m=$(convert /tmp/shot.png -format "%[mean]" info: 2>/dev/null | cut -d. -f1)
  echo "t=${i}s mean=${m:-0}"
  [ "${m:-0}" -gt 200 ] && break       # real content (a dark M3 page is ~5000–6000; ~0 = blank)
done
# 5. Read /tmp/shot.png with the agent's Read tool.
```

### Why a grab comes back black (233-byte / mean 0)

It's window **state/timing**, NOT a GPU/compositor limitation — no software rendering or Xvfb
is needed (that's a dead end). Two causes:
- **Grabbed before first paint** — use the poll loop above, not a fixed delay.
- **Stale duplicate windows** — earlier launches left windows open and you grabbed an occluded
  one. `xdotool search --name "CCSWE.Avalonia.Material.Demo"` should list exactly one id; close
  extras with `xdotool windowclose <id>`.

### Gotchas
- **Never `kill`/`pkill` in the same shell command that captures** — the signal hits the
  command's own process group and kills the capture (exit code 144). Capture the pid
  (`APP=$!`) and kill it in a separate step, or close the window with `xdotool windowclose`.
- `wmctrl` is non-functional under WSLg (no `_NET_CLIENT_LIST`); `import -window root` fails.
  Use `xdotool` + `import -window <id>`.
- Tools needed: `xdotool`, ImageMagick (`import`/`convert`/`identify`).

## B. Headless render to PNG (deterministic; no display, CI-friendly)

Render off-screen with Avalonia's Skia rasterizer — bypasses X11/Wayland entirely. The
`src/tests/CCSWE.Avalonia.Material.UnitTests` project is a ready-made `Avalonia.Headless.NUnit`
harness; the only change for a screenshot is disabling the stub renderer:

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
`Application.Current.RequestedThemeVariant = ThemeVariant.Light` before capture to shoot Light
(default is Dark).
