using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using JetBrains.Annotations;

namespace CCSWE.Avalonia.Material;

/// <summary>
/// A Material 3 circular progress indicator: the ring counterpart to <see cref="ProgressBar"/>. It derives
/// from <see cref="RangeBase"/> - the value/range model shared with the linear bar - rather than from
/// <see cref="ProgressBar"/>, whose linear-specific surface (an indicator <c>Border</c> part, orientation,
/// progress text) does not apply to a ring. The determinate arc sweeps clockwise from 12 o'clock; the
/// indeterminate state spins a fixed-length arc. <see cref="SweepAngle"/> is the determinate sweep (in
/// degrees) the control theme binds the arc to - computed from <see cref="RangeBase.Value"/> over the
/// effective range, so no value converter is needed.
/// </summary>
[PseudoClasses(":indeterminate")]
[PublicAPI]
public class CircularProgressIndicator : RangeBase
{
    public static readonly StyledProperty<bool> IsIndeterminateProperty =
        AvaloniaProperty.Register<CircularProgressIndicator, bool>(nameof(IsIndeterminate));

    public static readonly DirectProperty<CircularProgressIndicator, double> SweepAngleProperty =
        AvaloniaProperty.RegisterDirect<CircularProgressIndicator, double>(nameof(SweepAngle), o => o.SweepAngle);

    private double _sweepAngle;

    static CircularProgressIndicator()
    {
        // RangeBase defaults Maximum to 1.0; keep the friendly 0-100 convention ProgressBar uses.
        MaximumProperty.OverrideDefaultValue<CircularProgressIndicator>(100d);
    }

    public CircularProgressIndicator()
    {
        UpdateSweepAngle();
    }

    /// <summary>When <see langword="true"/>, the ring spins as an indeterminate activity indicator.</summary>
    public bool IsIndeterminate
    {
        get => GetValue(IsIndeterminateProperty);
        set => SetValue(IsIndeterminateProperty, value);
    }

    /// <summary>The determinate sweep angle in degrees (0-360), derived from <see cref="RangeBase.Value"/>.</summary>
    public double SweepAngle
    {
        get => _sweepAngle;
        private set => SetAndRaise(SweepAngleProperty, ref _sweepAngle, value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == IsIndeterminateProperty)
        {
            PseudoClasses.Set(":indeterminate", IsIndeterminate);
        }
        else if (change.Property == ValueProperty || change.Property == MinimumProperty || change.Property == MaximumProperty)
        {
            UpdateSweepAngle();
        }
    }

    private void UpdateSweepAngle()
    {
        var range = Maximum - Minimum;
        var fraction = range > 0d ? (Value - Minimum) / range : 0d;
        SweepAngle = fraction * 360d;
    }
}
