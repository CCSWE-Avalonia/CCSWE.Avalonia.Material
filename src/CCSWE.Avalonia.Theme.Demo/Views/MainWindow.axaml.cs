using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Styling;
using CCSWE.Avalonia.Theme.Demo.ViewModels;

namespace CCSWE.Avalonia.Theme.Demo.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
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
