using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Styling;
using CCSWE.Avalonia.Material.Demo.ViewModels;

namespace CCSWE.Avalonia.Material.Demo.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        // VERIFY-ONLY: jump to a page on startup when DEMO_PAGE is set, so headless
        // screenshots can capture each gallery page (synthetic input doesn't reach the
        // window under WSLg). No effect when the variable is unset.
        if (int.TryParse(System.Environment.GetEnvironmentVariable("DEMO_PAGE"), out var page))
        {
            NavList.SelectedIndex = page;
        }
    }

    // Sun/moon theme toggle. The glyph shows the CURRENT theme (moon = dark, sun = light);
    // a click flips the app variant and swaps the glyph to match.
    private void OnThemeToggleClick(object? sender, RoutedEventArgs e)
    {
        if (Application.Current is not { } app)
        {
            return;
        }

        var goingLight = app.RequestedThemeVariant != ThemeVariant.Light;
        app.RequestedThemeVariant = goingLight ? ThemeVariant.Light : ThemeVariant.Dark;
        ThemeIcon.Data = (Geometry)this.FindResource(goingLight ? "IconSun" : "IconMoon")!;
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
}
