using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml;
using CCSWE.Avalonia.Material.Demo.ViewModels;
using CCSWE.Avalonia.Material.Demo.Views;

namespace CCSWE.Avalonia.Material.Demo;

public partial class App : Application
{
    public override void Initialize()
    {
        // Selecting controls scroll the page to their initial selection on load (each raises a
        // RequestBringIntoView the page ScrollViewer honors regardless of Handled). Turn off
        // auto-scroll-to-selection for the gallery's control types so it stays anchored at the top.
        // (Must target the subclasses — the base SelectingItemsControl already owns the metadata.)
        ListBox.AutoScrollToSelectedItemProperty.OverrideDefaultValue<ListBox>(false);
        ComboBox.AutoScrollToSelectedItemProperty.OverrideDefaultValue<ComboBox>(false);
        TabControl.AutoScrollToSelectedItemProperty.OverrideDefaultValue<TabControl>(false);
        TabStrip.AutoScrollToSelectedItemProperty.OverrideDefaultValue<TabStrip>(false);

        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
