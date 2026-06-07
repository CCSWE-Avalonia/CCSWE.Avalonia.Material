using JetBrains.Annotations;

namespace CCSWE.Avalonia.Material;

/// <summary>
/// The Material 3 size variants for a <see cref="FloatingActionButton"/>. Maps to the M3 / Android
/// FAB sizes: <see cref="Small"/> (40dp), <see cref="Regular"/> (56dp), <see cref="Large"/> (96dp).
/// </summary>
[PublicAPI]
public enum FloatingActionButtonSize
{
    /// <summary>40dp container, 12dp corner radius - for secondary or grouped actions.</summary>
    Small,

    /// <summary>56dp container, 16dp corner radius - the standard primary action (default).</summary>
    Regular,

    /// <summary>96dp container, 28dp corner radius - a high-prominence action on large screens.</summary>
    Large,
}
