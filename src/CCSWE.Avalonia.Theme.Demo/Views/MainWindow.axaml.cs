using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Styling;

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
}
