using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
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

    private void OnThemeToggleChanged(object? sender, RoutedEventArgs e)
    {
        if (Application.Current is { } app && sender is ToggleSwitch toggle)
        {
            app.RequestedThemeVariant = toggle.IsChecked == true
                ? ThemeVariant.Light
                : ThemeVariant.Dark;
        }
    }

    // Collapse the labeled drawer (360dp) to the icon rail (80dp) and back. IsRail drives
    // the list/brand/toggle visibility via bindings; here we set the pane width and flip
    // the chevron so it points the way it will move.
    private void OnToggleRail(object? sender, RoutedEventArgs e)
    {
        if (DataContext is MainWindowViewModel vm)
        {
            vm.IsRail = !vm.IsRail;
            Shell.DrawerLength = vm.IsRail ? 80 : 360;
            CollapseChevron.RenderTransform = new RotateTransform(vm.IsRail ? 180 : 0);
        }
    }
}
