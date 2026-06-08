using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using JetBrains.Annotations;

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
/// Set <see cref="DensityStyle"/> to <see cref="Material.DensityStyle.Compact"/> for a
/// denser, desktop-oriented layout.
/// </summary>
public class MaterialTheme : Styles, IResourceNode
{
    public static readonly DirectProperty<MaterialTheme, DensityStyle> DensityStyleProperty =
        AvaloniaProperty.RegisterDirect<MaterialTheme, DensityStyle>(
            nameof(DensityStyle), o => o.DensityStyle, (o, v) => o.DensityStyle = v);

    private readonly ResourceDictionary _compactStyles;
    private DensityStyle _densityStyle;

    public MaterialTheme(IServiceProvider? serviceProvider = null)
    {
        AvaloniaXamlLoader.Load(serviceProvider, this);

        _compactStyles = (ResourceDictionary)GetAndRemove("CompactStyles");

        object GetAndRemove(string key)
        {
            var value = Resources[key]
                        ?? throw new KeyNotFoundException($"Key {key} was not found in the resources");
            Resources.Remove(key);
            return value;
        }
    }

    /// <summary>
    /// Gets or sets the layout density of the theme. Defaults to
    /// <see cref="Material.DensityStyle.Normal"/>.
    /// </summary>
    [PublicAPI]
    public DensityStyle DensityStyle
    {
        get => _densityStyle;
        set => SetAndRaise(DensityStyleProperty, ref _densityStyle, value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == DensityStyleProperty)
        {
            Owner?.NotifyHostedResourcesChanged(ResourcesChangedEventArgs.Create());
        }
    }

    bool IResourceNode.TryGetResource(object key, ThemeVariant? theme, out object? value)
    {
        // Compact overrides shadow the base dimension keys; check them first.
        if (_densityStyle == DensityStyle.Compact
            && _compactStyles.TryGetResource(key, theme, out value))
        {
            return true;
        }

        return base.TryGetResource(key, theme, out value);
    }
}
