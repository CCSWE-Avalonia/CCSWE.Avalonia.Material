using JetBrains.Annotations;

namespace CCSWE.Avalonia.Material;

/// <summary>
/// The Material 3 color mappings for a <see cref="FloatingActionButton"/>. M3 maps a FAB to one of
/// four container roles (it has no outlined variant); each pairs a container fill with its on-color.
/// </summary>
[PublicAPI]
public enum FloatingActionButtonColor
{
    /// <summary>PrimaryContainer fill, OnPrimaryContainer icon (default).</summary>
    Primary,

    /// <summary>SecondaryContainer fill, OnSecondaryContainer icon.</summary>
    Secondary,

    /// <summary>TertiaryContainer fill, OnTertiaryContainer icon.</summary>
    Tertiary,

    /// <summary>SurfaceContainerHigh fill, Primary icon.</summary>
    Surface,
}
