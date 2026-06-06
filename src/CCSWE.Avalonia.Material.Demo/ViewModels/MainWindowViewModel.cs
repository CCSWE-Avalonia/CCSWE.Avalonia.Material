using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CCSWE.Avalonia.Material.Demo.ViewModels;

public partial class MainWindowViewModel : ObservableValidator
{
    private int _cardClickCount;
    public IReadOnlyList<string> DeviceNames { get; } =
    [
        "Pixel 8 (emulator)",
        "Pixel 8 Pro",
        "Galaxy S23 (wifi)",
        "Galaxy Tab S9",
        "Nexus 5X (usb)",
        "OnePlus 12",
        "Nothing Phone 2",
    ];

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [RegularExpression(@"^\d+$", ErrorMessage = "Port must be a number")]
    private string? _port = "8O8O";

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Range(typeof(decimal), "0", "65535", ErrorMessage = "Port must be 0–65535")]
    private decimal _portNumber = 99999;

    // Drawer (false) vs. compact rail (true) for the navigation shell.
    [ObservableProperty]
    private bool _isRail;

    [ObservableProperty]
    private string _cardClicks = "Cards clicked: 0";

    public MainWindowViewModel()
    {
        ValidateAllProperties();
    }

    [RelayCommand]
    private void CardClicked()
    {
        _cardClickCount++;
        CardClicks = $"Cards clicked: {_cardClickCount}";
    }
}
