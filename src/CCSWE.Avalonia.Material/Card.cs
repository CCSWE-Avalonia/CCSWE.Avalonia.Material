using System;
using System.Linq;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.VisualTree;
using JetBrains.Annotations;

namespace CCSWE.Avalonia.Material;

/// <summary>
/// A Material 3 card: a surface container with Elevated / Filled / Outlined variants (applied via
/// <c>Classes</c>). A card is a container, not a button, so it derives from <see cref="ContentControl"/>;
/// the button-style interactivity (<see cref="Command"/>/<see cref="CommandParameter"/>/<see cref="Click"/>)
/// is ported from Avalonia's <c>Button</c>. When the card is effectively clickable it shows the M3 hover /
/// pressed state layers and raises <see cref="Click"/> / executes <see cref="Command"/>.
/// </summary>
[PublicAPI]
public class Card : ContentControl
{
    public static readonly RoutedEvent<RoutedEventArgs> ClickEvent =
        RoutedEvent.Register<Card, RoutedEventArgs>(nameof(Click), RoutingStrategies.Bubble);

    public static readonly StyledProperty<object?> CommandParameterProperty =
        AvaloniaProperty.Register<Card, object?>(nameof(CommandParameter));

    public static readonly StyledProperty<ICommand?> CommandProperty =
        AvaloniaProperty.Register<Card, ICommand?>(nameof(Command));

    /// <summary>
    /// Overrides whether the card is interactive. When <see langword="null"/> (the default), clickability
    /// is derived from whether a <see cref="Command"/> is set - mirroring M3 Android, where assigning a
    /// click listener auto-enables <c>clickable</c>. A non-null value forces it on or off.
    /// </summary>
    public static readonly StyledProperty<bool?> IsClickableProperty =
        AvaloniaProperty.Register<Card, bool?>(nameof(IsClickable));

    private EventHandler? _canExecuteChangedHandler;
    private bool _commandCanExecute = true;
    private bool _isPressed;

    public Card()
    {
        UpdateClickable();
    }

    private EventHandler CanExecuteChangedHandler => _canExecuteChangedHandler ??= OnCommandCanExecuteChanged;

    public ICommand? Command
    {
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public object? CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    public bool? IsClickable
    {
        get => GetValue(IsClickableProperty);
        set => SetValue(IsClickableProperty, value);
    }

    /// <summary>Effective clickability: the <see cref="IsClickable"/> override, else derived from <see cref="Command"/>.</summary>
    private bool IsEffectivelyClickable => IsClickable ?? Command is not null;

    protected override bool IsEnabledCore => base.IsEnabledCore && _commandCanExecute;

    public event EventHandler<RoutedEventArgs>? Click
    {
        add => AddHandler(ClickEvent, value);
        remove => RemoveHandler(ClickEvent, value);
    }

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);

        if (Command is { } command)
        {
            command.CanExecuteChanged += CanExecuteChangedHandler;
        }

        UpdateCanExecute();
    }

    protected virtual void OnClick()
    {
        if (!IsEffectivelyEnabled || !IsEffectivelyClickable)
        {
            return;
        }

        var e = new RoutedEventArgs(ClickEvent);
        RaiseEvent(e);

        if (!e.Handled && Command is { } command && command.CanExecute(CommandParameter))
        {
            command.Execute(CommandParameter);
            e.Handled = true;
        }
    }

    protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromLogicalTree(e);

        if (Command is { } command)
        {
            command.CanExecuteChanged -= CanExecuteChangedHandler;
        }
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);

        if (!IsEffectivelyClickable)
        {
            return;
        }

        switch (e.Key)
        {
            case Key.Enter:
                OnClick();
                e.Handled = true;
                break;
            case Key.Space when IsFocused:
                SetPressed(true);
                e.Handled = true;
                break;
        }
    }

    protected override void OnKeyUp(KeyEventArgs e)
    {
        base.OnKeyUp(e);

        if (e.Key == Key.Space && _isPressed && IsFocused)
        {
            SetPressed(false);
            OnClick();
            e.Handled = true;
        }
    }

    protected override void OnLostFocus(FocusChangedEventArgs e)
    {
        base.OnLostFocus(e);
        SetPressed(false);
    }

    protected override void OnPointerCaptureLost(PointerCaptureLostEventArgs e)
    {
        base.OnPointerCaptureLost(e);
        SetPressed(false);
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);

        if (!IsEffectivelyClickable)
        {
            return;
        }

        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            SetPressed(true);
            e.Handled = true;
        }
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);

        if (!_isPressed || e.InitialPressMouseButton != MouseButton.Left)
        {
            return;
        }

        SetPressed(false);
        e.Handled = true;

        // Only a release that lands back over the card counts as a click (mirrors Button's bounds test).
        if (this.GetVisualsAt(e.GetPosition(this)).Any(c => this == c || this.IsVisualAncestorOf(c)))
        {
            OnClick();
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == CommandProperty)
        {
            var (oldValue, newValue) = change.GetOldAndNewValue<ICommand?>();
            if (((ILogical)this).IsAttachedToLogicalTree)
            {
                if (oldValue is not null)
                {
                    oldValue.CanExecuteChanged -= CanExecuteChangedHandler;
                }

                if (newValue is not null)
                {
                    newValue.CanExecuteChanged += CanExecuteChangedHandler;
                }
            }

            UpdateCanExecute();
            UpdateClickable();
        }
        else if (change.Property == CommandParameterProperty)
        {
            UpdateCanExecute();
        }
        else if (change.Property == IsClickableProperty)
        {
            UpdateClickable();
        }
    }

    private void OnCommandCanExecuteChanged(object? sender, EventArgs e) => UpdateCanExecute();

    private void SetPressed(bool value)
    {
        if (_isPressed == value)
        {
            return;
        }

        _isPressed = value;
        PseudoClasses.Set(":pressed", value);
    }

    private void UpdateCanExecute()
    {
        var canExecute = Command is null || Command.CanExecute(CommandParameter);
        if (canExecute != _commandCanExecute)
        {
            _commandCanExecute = canExecute;
            UpdateIsEffectivelyEnabled();
        }
    }

    private void UpdateClickable()
    {
        var clickable = IsEffectivelyClickable;

        Focusable = clickable;
        PseudoClasses.Set(":clickable", clickable);

        if (!clickable)
        {
            SetPressed(false);
        }
    }
}
