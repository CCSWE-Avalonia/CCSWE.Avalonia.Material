using System;
using System.Globalization;
using Avalonia.Data.Converters;
using CCSWE.Avalonia.Material.Demo.Views.Pages;

namespace CCSWE.Avalonia.Material.Demo.Converters;

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
                3 => new DateTimePage(),
                4 => new SelectionPage(),
                5 => new CollectionsPage(),
                6 => new FeedbackPage(),
                7 => new TabsPage(),
                8 => new ContainersPage(),
                9 => new IconsPage(),
                _ => null,
            }
            : null;

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}
