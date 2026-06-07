using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using JetBrains.Annotations;

namespace CCSWE.Avalonia.Material;

/// <summary>
/// A Material 3 divider: a thin horizontal rule that separates content. An optional <see cref="Header"/>
/// turns it into a leading section header - the label sits on the rule's axis with the hairline filling
/// the remaining width to its right (Avalonia ships no equivalent, so this is a custom control type).
/// When <see cref="Header"/> is unset the divider collapses to a plain full-width rule; the collapse is
/// handled entirely by the control theme (the header presenter hides and its column measures to zero), so
/// the type itself is just the <see cref="Header"/> / <see cref="HeaderTemplate"/> surface.
/// </summary>
[PublicAPI]
public class Divider : TemplatedControl
{
    public static readonly StyledProperty<object?> HeaderProperty =
        AvaloniaProperty.Register<Divider, object?>(nameof(Header));

    public static readonly StyledProperty<IDataTemplate?> HeaderTemplateProperty =
        AvaloniaProperty.Register<Divider, IDataTemplate?>(nameof(HeaderTemplate));

    /// <summary>The optional leading section-header label. When <see langword="null"/> the divider is a plain rule.</summary>
    public object? Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    /// <summary>The template used to display the <see cref="Header"/>.</summary>
    public IDataTemplate? HeaderTemplate
    {
        get => GetValue(HeaderTemplateProperty);
        set => SetValue(HeaderTemplateProperty, value);
    }
}
