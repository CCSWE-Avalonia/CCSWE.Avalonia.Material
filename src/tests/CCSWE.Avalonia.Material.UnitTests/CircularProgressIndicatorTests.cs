using System.Diagnostics.CodeAnalysis;
using Avalonia.Headless.NUnit;
using CCSWE.Avalonia.Material;
using NUnit.Framework;

namespace CCSWE.Avalonia.Material.UnitTests;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public class CircularProgressIndicatorTests
{
    public class When_Value_Changes : CircularProgressIndicatorTests
    {
        [AvaloniaTest]
        public void It_computes_sweep_angle_from_value()
        {
            var sut = new CircularProgressIndicator { Minimum = 0, Maximum = 100 };

            sut.Value = 50;

            Assert.That(sut.SweepAngle, Is.EqualTo(180d).Within(0.0001));
        }

        [AvaloniaTest]
        public void It_computes_sweep_angle_over_a_custom_range()
        {
            var sut = new CircularProgressIndicator { Minimum = 0, Maximum = 200 };

            sut.Value = 50;

            Assert.That(sut.SweepAngle, Is.EqualTo(90d).Within(0.0001));
        }
    }

    public class When_Constructed : CircularProgressIndicatorTests
    {
        [AvaloniaTest]
        public void It_defaults_maximum_to_100()
        {
            var sut = new CircularProgressIndicator();

            Assert.That(sut.Maximum, Is.EqualTo(100d));
        }

        [AvaloniaTest]
        public void It_starts_at_zero_sweep()
        {
            var sut = new CircularProgressIndicator();

            Assert.That(sut.SweepAngle, Is.EqualTo(0d).Within(0.0001));
        }
    }

    public class When_IsIndeterminate_Is_Set : CircularProgressIndicatorTests
    {
        [AvaloniaTest]
        public void It_sets_the_indeterminate_pseudo_class()
        {
            var sut = new CircularProgressIndicator { IsIndeterminate = true };

            Assert.That(sut.Classes, Does.Contain(":indeterminate"));
        }

        [AvaloniaTest]
        public void It_clears_the_indeterminate_pseudo_class_when_false()
        {
            var sut = new CircularProgressIndicator { IsIndeterminate = true };

            sut.IsIndeterminate = false;

            Assert.That(sut.Classes, Does.Not.Contain(":indeterminate"));
        }
    }
}
