using System;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;

namespace CCSWE.Avalonia.Material;

/// <summary>
/// The CCSWE Material 3 theme for Avalonia. It supplies the whole control surface itself
/// (no base theme such as Fluent or Simple is required). Add it once to
/// <see cref="Avalonia.Application.Styles"/>:
/// <code>
/// &lt;Application xmlns:theme="using:CCSWE.Avalonia.Material"&gt;
///   &lt;Application.Styles&gt;
///     &lt;theme:MaterialTheme /&gt;
///   &lt;/Application.Styles&gt;
/// &lt;/Application&gt;
/// </code>
/// Dark is the default variant; select via <c>Application.RequestedThemeVariant</c>.
/// </summary>
public class MaterialTheme : Styles
{
    public MaterialTheme(IServiceProvider? serviceProvider = null)
    {
        AvaloniaXamlLoader.Load(serviceProvider, this);
    }
}
