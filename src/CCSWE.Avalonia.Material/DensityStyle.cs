using JetBrains.Annotations;

namespace CCSWE.Avalonia.Material;

/// <summary>
/// Controls the layout density of <see cref="MaterialTheme"/>. Material 3 defines no
/// compact density of its own; <see cref="Compact"/> is a library-owned, desktop-oriented
/// option modelled on Avalonia's <c>FluentTheme.DensityStyle</c>.
/// </summary>
[PublicAPI]
public enum DensityStyle
{
    /// <summary>The default M3 sizing (full-size touch targets).</summary>
    Normal,

    /// <summary>A denser, desktop-oriented sizing that shrinks heights and paddings.</summary>
    Compact
}
