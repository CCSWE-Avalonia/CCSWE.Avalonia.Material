using System;
using System.Globalization;
using Avalonia.Data.Converters;
using CCSWE.Avalonia.Theme.Demo.Views.Pages;

namespace CCSWE.Avalonia.Theme.Demo.Converters;

/// <summary>
/// Maps the nav selection index to a freshly-built gallery page for the
/// TransitioningContentControl host. Pages are plain UserControls, so each
/// inherits the window DataContext (the VM) - unlike a SelectingItemsControl,
/// which reparents item DataContext and broke bound pages under the old Carousel.
/// </summary>
public sealed class NavIndexToPageConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value is int index
            ? index switch
            {
                0 => new TypographyPage(),
                1 => new ButtonsPage(),
                2 => new InputsPage(),
                3 => new SelectionPage(),
                4 => new CollectionsPage(),
                5 => new FeedbackPage(),
                6 => new TabsPage(),
                7 => new ContainersPage(),
                8 => new CoveragePage(),
                _ => null,
            }
            : null;

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
