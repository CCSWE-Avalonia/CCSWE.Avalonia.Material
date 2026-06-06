using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace CCSWE.Avalonia.Material.Demo.Views;

/// <summary>
/// Base for the gallery's section pages: a <see cref="ContentControl"/> whose content lives in a
/// <see cref="ScrollViewer"/>. Exposes that <see cref="ScrollViewer"/> (template part
/// <c>PART_Scroll</c>) so the record-mode tour can drive a smooth scroll through each page.
/// </summary>
public class GalleryPage : ContentControl
{
    public ScrollViewer? Scroll { get; private set; }

    // Subclasses (ButtonsPage, etc.) must resolve to GalleryPage's ControlTheme rather than their
    // own type — otherwise they fall back to a bare ContentControl template (no PART_Scroll) and
    // the content overflows instead of scrolling. (UserControl does the same with its own type.)
    protected override Type StyleKeyOverride => typeof(GalleryPage);

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        Scroll = e.NameScope.Find<ScrollViewer>("PART_Scroll");

        if (Scroll is null)
        {
            throw new InvalidOperationException($"Scroll viewer is not found. {e.NameScope} is missing a template part named PART_Scroll.");
        }
    }
}
