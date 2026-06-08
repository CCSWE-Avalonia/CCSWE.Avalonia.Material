using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Headless.NUnit;
using Avalonia.Threading;
using NUnit.Framework;

namespace CCSWE.Avalonia.Material.UnitTests;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public class DensityTests
{
    private static MaterialTheme Theme => Application.Current!.Styles.OfType<MaterialTheme>().First();

    // Lay a control out in a window under the given density and return its rendered height.
    // No explicit InvalidateMeasure: this asserts the resource change auto-invalidates layout,
    // exactly as the runtime DensityStyle toggle relies on. Density is reset afterwards so the
    // shared application state does not leak between tests.
    private static double HeightUnder(Control control, DensityStyle density)
    {
        try
        {
            Theme.DensityStyle = DensityStyle.Normal;

            // Host in a StackPanel so the control takes its desired height instead of
            // stretching to fill the window.
            var window = new Window { Content = new StackPanel { Children = { control } } };
            window.Show();
            Dispatcher.UIThread.RunJobs();

            Theme.DensityStyle = density;
            Dispatcher.UIThread.RunJobs();

            return control.Bounds.Height;
        }
        finally
        {
            Theme.DensityStyle = DensityStyle.Normal;
        }
    }

    // Apply a control under the given density and return its resolved Padding (uniform left).
    private static double PaddingUnder(TemplatedControl control, DensityStyle density)
    {
        try
        {
            Theme.DensityStyle = DensityStyle.Normal;

            var window = new Window { Content = new StackPanel { Children = { control } } };
            window.Show();
            Dispatcher.UIThread.RunJobs();

            Theme.DensityStyle = density;
            Dispatcher.UIThread.RunJobs();

            return control.Padding.Left;
        }
        finally
        {
            Theme.DensityStyle = DensityStyle.Normal;
        }
    }

    private static Button Filled(bool enabled = true)
    {
        var button = new Button { Content = "x", IsEnabled = enabled };
        button.Classes.Add("Filled");
        return button;
    }

    // Lay out a single ListBoxItem inside a classed nav ListBox and return the item height.
    private static double NavItemHeightUnder(string listClass, DensityStyle density)
    {
        try
        {
            Theme.DensityStyle = DensityStyle.Normal;

            var item = new ListBoxItem { Content = "x" };
            var list = new ListBox();
            list.Classes.Add(listClass);
            list.Items.Add(item);

            var window = new Window { Content = list };
            window.Show();
            Dispatcher.UIThread.RunJobs();

            Theme.DensityStyle = density;
            Dispatcher.UIThread.RunJobs();

            return item.Bounds.Height;
        }
        finally
        {
            Theme.DensityStyle = DensityStyle.Normal;
        }
    }

    public class When_DensityStyle_Is_Normal : DensityTests
    {
        [AvaloniaTest]
        public void It_keeps_the_text_field_at_56() =>
            Assert.That(HeightUnder(new TextBox(), DensityStyle.Normal), Is.EqualTo(56));

        [AvaloniaTest]
        public void It_keeps_the_button_at_40() =>
            Assert.That(HeightUnder(new Button { Content = "x" }, DensityStyle.Normal), Is.EqualTo(40));

        [AvaloniaTest]
        public void It_keeps_the_numeric_stepper_at_56() =>
            Assert.That(HeightUnder(new NumericUpDown(), DensityStyle.Normal), Is.EqualTo(56));

        [AvaloniaTest]
        public void It_keeps_the_fab_at_56() =>
            Assert.That(HeightUnder(new FloatingActionButton(), DensityStyle.Normal), Is.EqualTo(56));

        [AvaloniaTest]
        public void It_keeps_the_nav_drawer_item_at_56() =>
            Assert.That(NavItemHeightUnder("NavigationDrawer", DensityStyle.Normal), Is.EqualTo(56));

        [AvaloniaTest]
        public void It_keeps_the_nav_rail_item_at_56() =>
            Assert.That(NavItemHeightUnder("NavigationRail", DensityStyle.Normal), Is.EqualTo(56));

        [AvaloniaTest]
        public void It_keeps_the_command_bar_at_48() =>
            Assert.That(HeightUnder(new CommandBar(), DensityStyle.Normal), Is.EqualTo(48));

        [AvaloniaTest]
        public void It_keeps_the_date_picker_at_56() =>
            Assert.That(HeightUnder(new DatePicker(), DensityStyle.Normal), Is.EqualTo(56));

        [AvaloniaTest]
        public void It_keeps_the_time_picker_at_56() =>
            Assert.That(HeightUnder(new TimePicker(), DensityStyle.Normal), Is.EqualTo(56));

        [AvaloniaTest]
        public void It_keeps_the_card_padding_at_16() =>
            Assert.That(PaddingUnder(new Card(), DensityStyle.Normal), Is.EqualTo(16));

        [AvaloniaTest]
        public void It_keeps_the_group_box_padding_at_16() =>
            Assert.That(PaddingUnder(new GroupBox(), DensityStyle.Normal), Is.EqualTo(16));
    }

    public class When_DensityStyle_Is_Compact : DensityTests
    {
        [AvaloniaTest]
        public void It_shrinks_the_text_field_to_48() =>
            Assert.That(HeightUnder(new TextBox(), DensityStyle.Compact), Is.EqualTo(48));

        [AvaloniaTest]
        public void It_shrinks_the_button_to_36() =>
            Assert.That(HeightUnder(new Button { Content = "x" }, DensityStyle.Compact), Is.EqualTo(36));

        [AvaloniaTest]
        public void It_shrinks_a_filled_class_button_to_36() =>
            Assert.That(HeightUnder(Filled(), DensityStyle.Compact), Is.EqualTo(36));

        [AvaloniaTest]
        public void It_shrinks_a_disabled_filled_class_button_to_36() =>
            Assert.That(HeightUnder(Filled(enabled: false), DensityStyle.Compact), Is.EqualTo(36));

        [AvaloniaTest]
        public void It_shrinks_the_numeric_stepper_to_48() =>
            Assert.That(HeightUnder(new NumericUpDown(), DensityStyle.Compact), Is.EqualTo(48));

        [AvaloniaTest]
        public void It_shrinks_the_fab_to_48() =>
            Assert.That(HeightUnder(new FloatingActionButton(), DensityStyle.Compact), Is.EqualTo(48));

        [AvaloniaTest]
        public void It_shrinks_the_nav_drawer_item_to_48() =>
            Assert.That(NavItemHeightUnder("NavigationDrawer", DensityStyle.Compact), Is.EqualTo(48));

        [AvaloniaTest]
        public void It_shrinks_the_nav_rail_item_to_48() =>
            Assert.That(NavItemHeightUnder("NavigationRail", DensityStyle.Compact), Is.EqualTo(48));

        [AvaloniaTest]
        public void It_shrinks_the_command_bar_to_40() =>
            Assert.That(HeightUnder(new CommandBar(), DensityStyle.Compact), Is.EqualTo(40));

        [AvaloniaTest]
        public void It_shrinks_the_date_picker_to_48() =>
            Assert.That(HeightUnder(new DatePicker(), DensityStyle.Compact), Is.EqualTo(48));

        [AvaloniaTest]
        public void It_shrinks_the_time_picker_to_48() =>
            Assert.That(HeightUnder(new TimePicker(), DensityStyle.Compact), Is.EqualTo(48));

        [AvaloniaTest]
        public void It_shrinks_the_card_padding_to_12() =>
            Assert.That(PaddingUnder(new Card(), DensityStyle.Compact), Is.EqualTo(12));

        [AvaloniaTest]
        public void It_shrinks_the_group_box_padding_to_12() =>
            Assert.That(PaddingUnder(new GroupBox(), DensityStyle.Compact), Is.EqualTo(12));
    }

    public class When_Compact_Class_Is_Applied : DensityTests
    {
        // The per-control .Compact class shrinks the control even while the theme stays Normal.
        [AvaloniaTest]
        public void It_shrinks_only_the_classed_text_field()
        {
            var classed = new TextBox();
            classed.Classes.Add("Compact");

            Assert.That(HeightUnder(classed, DensityStyle.Normal), Is.EqualTo(48));
            Assert.That(HeightUnder(new TextBox(), DensityStyle.Normal), Is.EqualTo(56));
        }
    }
}
