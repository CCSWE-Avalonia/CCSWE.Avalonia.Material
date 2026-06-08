using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless.NUnit;
using CCSWE.Avalonia.Material;
using NUnit.Framework;

namespace CCSWE.Avalonia.Material.UnitTests;

/// <summary>
/// The theme smoke harness: renders a representative slice of the control surface under
/// <see cref="MaterialTheme"/> and asserts nothing threw and no binding/resource/template warnings were
/// logged. This is the net for the class of runtime bugs the XAML compiler cannot catch (missing
/// <c>DynamicResource</c> keys, broken templates, failed bindings).
/// </summary>
[SuppressMessage("ReSharper", "InconsistentNaming")]
public class MaterialThemeTests
{
    private static IEnumerable<(string Name, Func<Control> Factory)> Controls =>
    [
        ("Button.Filled", () => new Button { Content = "OK", Classes = { "Filled" } }),
        ("Button.Outlined", () => new Button { Content = "OK", Classes = { "Outlined" } }),
        ("FloatingActionButton.Regular.Primary", () => new FloatingActionButton { Content = "+" }),
        ("FloatingActionButton.Small.Secondary", () => new FloatingActionButton { Content = "+", Size = FloatingActionButtonSize.Small, Color = FloatingActionButtonColor.Secondary }),
        ("FloatingActionButton.Large.Tertiary", () => new FloatingActionButton { Content = "+", Size = FloatingActionButtonSize.Large, Color = FloatingActionButtonColor.Tertiary }),
        ("FloatingActionButton.Regular.Surface", () => new FloatingActionButton { Content = "+", Color = FloatingActionButtonColor.Surface }),
        ("CircularProgressIndicator.Determinate", () => new CircularProgressIndicator { Value = 60 }),
        ("CircularProgressIndicator.Indeterminate", () => new CircularProgressIndicator { IsIndeterminate = true }),
        ("Card", () => new Card { Content = "Card" }),
        ("Divider.Plain", () => new Divider()),
        ("Divider.Header", () => new Divider { Header = "Section" }),
        ("CheckBox", () => new CheckBox { Content = "Check" }),
        ("RadioButton", () => new RadioButton { Content = "Radio" }),
        ("ToggleButton", () => new ToggleButton { Content = "Toggle" }),
        ("ToggleSwitch", () => new ToggleSwitch { Content = "Switch" }),
        ("TextBox", () => new TextBox { Text = "Text" }),
        ("ComboBox", () => new ComboBox { ItemsSource = new[] { "One", "Two" }, SelectedIndex = 0 }),
        ("Slider", () => new Slider { Minimum = 0, Maximum = 100, Value = 50 }),
        ("ProgressBar", () => new ProgressBar { Value = 60 }),
        ("ListBox", () => new ListBox { ItemsSource = new[] { "One", "Two" } }),
        ("TabControl", () => new TabControl { Items = { new TabItem { Header = "Tab", Content = "Body" } } }),
        ("Expander", () => new Expander { Header = "Header", Content = "Body" }),
        ("Menu", () => new Menu { Items = { new MenuItem { Header = "File" } } }),
        ("CalendarDatePicker", () => new CalendarDatePicker()),
        ("CommandBar", () => new CommandBar()),
    ];

    [AvaloniaTest]
    public void All_themed_controls_render_without_errors()
    {
        var failures = new List<string>();

        foreach (var (name, factory) in Controls)
        {
            using var sink = CapturingLogSink.Install();
            Window? window = null;

            try
            {
                window = new Window { Content = factory(), SizeToContent = SizeToContent.WidthAndHeight };
                window.Show();
            }
            catch (Exception ex)
            {
                failures.Add($"{name}: threw {ex.GetType().Name}: {ex.Message}");
            }
            finally
            {
                window?.Close();
            }

            failures.AddRange(sink.Entries.Select(entry => $"{name}: {entry}"));
        }

        Assert.That(failures, Is.Empty, "Themed controls produced errors:" + Environment.NewLine + string.Join(Environment.NewLine, failures));
    }
}
