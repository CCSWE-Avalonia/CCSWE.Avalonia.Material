using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Headless.NUnit;
using Avalonia.VisualTree;
using CCSWE.Avalonia.Material;
using NUnit.Framework;

namespace CCSWE.Avalonia.Material.UnitTests;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public class DividerTests
{
    private static ContentPresenter? GetHeaderPresenter(Divider divider) =>
        divider.GetVisualDescendants().OfType<ContentPresenter>().FirstOrDefault(c => c.Name == "PART_HeaderPresenter");

    private static Divider Render(Divider divider)
    {
        var window = new Window { Content = divider, Width = 400, Height = 50 };
        window.Show();
        return divider;
    }

    public class When_No_Header_Is_Set : DividerTests
    {
        [AvaloniaTest]
        public void It_defaults_header_to_null()
        {
            var sut = new Divider();

            Assert.That(sut.Header, Is.Null);
        }

        [AvaloniaTest]
        public void It_collapses_the_header_presenter()
        {
            var sut = Render(new Divider());

            var presenter = GetHeaderPresenter(sut);

            Assert.That(presenter, Is.Not.Null);
            Assert.That(presenter!.IsVisible, Is.False);
        }
    }

    public class When_A_Header_Is_Set : DividerTests
    {
        [AvaloniaTest]
        public void It_round_trips_the_header()
        {
            var sut = new Divider { Header = "Storage" };

            Assert.That(sut.Header, Is.EqualTo("Storage"));
        }

        [AvaloniaTest]
        public void It_shows_the_header_presenter()
        {
            var sut = Render(new Divider { Header = "Storage" });

            var presenter = GetHeaderPresenter(sut);

            Assert.That(presenter, Is.Not.Null);
            Assert.That(presenter!.IsVisible, Is.True);
        }
    }
}
