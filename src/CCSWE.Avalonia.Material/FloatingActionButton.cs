using Avalonia;
using Avalonia.Controls;
using JetBrains.Annotations;

namespace CCSWE.Avalonia.Material;

/// <summary>
/// A Material 3 floating action button: a high-emphasis, persistent affordance for the primary action.
/// A FAB is an action trigger, so it derives from <see cref="Button"/> - inheriting its Command/Click,
/// keyboard, and pointer behavior along with the standard <c>:pointerover</c>/<c>:pressed</c>/<c>:disabled</c>
/// pseudo-classes. <see cref="Size"/> and <see cref="Color"/> are orthogonal axes the control theme styles
/// via <c>[Size=…]</c> / <c>[Color=…]</c> selectors.
/// </summary>
[PublicAPI]
public class FloatingActionButton : Button
{
    public static readonly StyledProperty<FloatingActionButtonColor> ColorProperty =
        AvaloniaProperty.Register<FloatingActionButton, FloatingActionButtonColor>(nameof(Color));

    public static readonly StyledProperty<FloatingActionButtonSize> SizeProperty =
        AvaloniaProperty.Register<FloatingActionButton, FloatingActionButtonSize>(nameof(Size), FloatingActionButtonSize.Regular);

    /// <summary>The M3 color mapping (container + on-color). Defaults to <see cref="FloatingActionButtonColor.Primary"/>.</summary>
    public FloatingActionButtonColor Color
    {
        get => GetValue(ColorProperty);
        set => SetValue(ColorProperty, value);
    }

    /// <summary>The M3 size variant. Defaults to <see cref="FloatingActionButtonSize.Regular"/>.</summary>
    public FloatingActionButtonSize Size
    {
        get => GetValue(SizeProperty);
        set => SetValue(SizeProperty, value);
    }
}
