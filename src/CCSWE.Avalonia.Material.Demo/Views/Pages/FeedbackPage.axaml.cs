using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Interactivity;

namespace CCSWE.Avalonia.Material.Demo.Views.Pages;

public partial class FeedbackPage : UserControl
{
    private WindowNotificationManager? _notificationManager;

    public FeedbackPage()
    {
        InitializeComponent();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _notificationManager = new WindowNotificationManager(TopLevel.GetTopLevel(this))
        {
            Position = NotificationPosition.BottomRight,
            MaxItems = 3
        };
    }

    private void Show(string title, string message, NotificationType type)
    {
        _notificationManager?.Show(new Notification(title, message, type));
    }

    private void OnShowInformation(object? sender, RoutedEventArgs e) =>
        Show("Information", "Pixel 8 emulator is now connected.", NotificationType.Information);

    private void OnShowSuccess(object? sender, RoutedEventArgs e) =>
        Show("Success", "Build deployed to the device.", NotificationType.Success);

    private void OnShowWarning(object? sender, RoutedEventArgs e) =>
        Show("Warning", "Battery is below 15% on the connected device.", NotificationType.Warning);

    private void OnShowError(object? sender, RoutedEventArgs e) =>
        Show("Error", "adb server failed to start on port 5037.", NotificationType.Error);
}
