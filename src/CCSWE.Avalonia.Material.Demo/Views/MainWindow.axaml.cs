using System;
using System.Linq;
using System.Net.Mime;
using System.Numerics;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;
using CCSWE.Avalonia.Material;
using CCSWE.Avalonia.Material.Demo.ViewModels;
using Vector = Avalonia.Vector;

namespace CCSWE.Avalonia.Material.Demo.Views;

public partial class MainWindow : Window
{
    private readonly DemoOptions? _options;
    private bool _tourStarted;

    public MainWindow() : this(null)
    {
    }

    public MainWindow(DemoOptions? options)
    {
        InitializeComponent();

        _options = options;

        // VERIFY-ONLY: jump to a page on startup when DEMO_PAGE is set, so headless
        // screenshots can capture each gallery page (synthetic input doesn't reach the
        // window under WSLg). No effect when the variable is unset.
        if (int.TryParse(Environment.GetEnvironmentVariable("DEMO_PAGE"), out var page))
        {
            NavList.SelectedIndex = page;
        }

        if (options is { Record: true })
        {
            Loaded += OnRecordLoaded;
        }
    }

    // Hands-free gallery tour used to capture the README GIF: after a start delay (to arm an
    // external recorder), walk every section top→bottom with a smooth slow scroll, switch
    // Dark→Light, and repeat the pass. Drives the same nav/theme controls a user would.
    private async void OnRecordLoaded(object? sender, RoutedEventArgs e)
    {
        if (_options is not { Record: true } || _tourStarted)
        {
            return;
        }

        _tourStarted = true;
        await RunTourAsync(_options);
    }

    // Sun/moon theme toggle. The glyph shows the CURRENT theme (moon = dark, sun = light).
    private void OnThemeToggleClick(object? sender, RoutedEventArgs e)
    {
        var goingLight = Application.Current?.RequestedThemeVariant != ThemeVariant.Light;
        SetTheme(goingLight ? ThemeVariant.Light : ThemeVariant.Dark);
    }

    // Normal/Compact density toggle. Flips MaterialTheme.DensityStyle, which re-resolves
    // every dimension DynamicResource live (no restart) across the whole control surface.
    private void OnDensityToggleClick(object? sender, RoutedEventArgs e)
    {
        if (Application.Current?.Styles.OfType<MaterialTheme>().FirstOrDefault() is not { } theme)
        {
            return;
        }

        theme.DensityStyle = theme.DensityStyle == DensityStyle.Compact
            ? DensityStyle.Normal
            : DensityStyle.Compact;
    }

    // Collapse the labeled drawer (360dp) to the icon rail (80dp) and back. IsRail drives
    // the list/brand/toggle visibility via bindings; here we set the pane width, flip the
    // chevron so it points the way it will move, and center the toggle in the narrow rail
    // (it's right-aligned beside the theme toggle in the expanded drawer).
    private void OnToggleRail(object? sender, RoutedEventArgs e)
    {
        if (DataContext is MainWindowViewModel vm)
        {
            vm.IsRail = !vm.IsRail;
            Shell.DrawerLength = vm.IsRail ? 80 : 360;
            CollapseChevron.RenderTransform = new RotateTransform(vm.IsRail ? 180 : 0);
            RailToggleBtn.HorizontalAlignment = vm.IsRail
                ? HorizontalAlignment.Center
                : HorizontalAlignment.Right;
        }
    }

    // Show a full-window countdown during the start delay (so an external recorder can be armed),
    // then hide it once two seconds remain so the overlay isn't captured in the recording.
    private async Task CountdownAsync(TimeSpan delay)
    {
        var remaining = (int)Math.Round(delay.TotalSeconds);

        CountdownOverlay.IsVisible = true;
        while (remaining > 2)
        {
            CountdownText.Text = remaining.ToString();
            await Task.Delay(1000);
            remaining--;
        }

        // Hide with two seconds left, then wait them out before the tour starts.
        CountdownOverlay.IsVisible = false;
        await Task.Delay(remaining * 1000);
    }

    private async Task RunTourAsync(DemoOptions options)
    {
        await CountdownAsync(options.StartDelay);

        do
        {
            foreach (var variant in new[] { ThemeVariant.Dark, ThemeVariant.Light })
            {
                SetTheme(variant);
                await Task.Delay(400);

                for (var i = 0; i < NavList.ItemCount; i++)
                {
                    NavList.SelectedIndex = i;
                    // Let the page build + lay out (PageHost's 150ms CrossFade then a layout pass).
                    await Task.Delay(300);

                    if ((PageHost.Content as GalleryPage)?.Scroll is { } scroll)
                    {
                        await SmoothScrollToBottomAsync(scroll, options);
                    }

                    await Task.Delay(options.Dwell);
                }
            }
        }
        while (options.Loop);

        // Settle back to a neutral state.
        SetTheme(ThemeVariant.Dark);
        NavList.SelectedIndex = 0;
    }

    private void SetTheme(ThemeVariant variant)
    {
        if (Application.Current is not { } app)
        {
            return;
        }

        app.RequestedThemeVariant = variant;
        ThemeIcon.Data = (Geometry)this.FindResource(variant == ThemeVariant.Light ? "IconSun" : "IconMoon")!;
    }

    private static async Task SmoothScrollToBottomAsync(ScrollViewer scroll, DemoOptions options)
    {
        await Task.Delay(options.Dwell);
        
        var max = Math.Max(0, scroll.Extent.Height - scroll.Viewport.Height);
        if (max <= 0)
        {
            // Page fits without scrolling - still hold it on screen for a comparable beat.
            await Task.Delay(options.Dwell);
            return;
        }

        var y = 0.0;
        while (y < max)
        {
            y = Math.Min(max, y + options.ScrollSpeed);
            scroll.Offset = new Vector(scroll.Offset.X, y);
            await Task.Delay(16);
        }

        await Task.Delay(options.Dwell);
    }
}
