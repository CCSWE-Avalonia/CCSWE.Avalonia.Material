using System.Diagnostics.CodeAnalysis;
using System.Windows.Input;
using Avalonia.Headless.NUnit;
using CCSWE.Avalonia.Material;
using Moq;
using NUnit.Framework;

namespace CCSWE.Avalonia.Material.UnitTests;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public class CardTests
{
    public class When_No_Command_Is_Set : CardTests
    {
        [AvaloniaTest]
        public void It_is_not_clickable()
        {
            var sut = new Card();

            Assert.That(sut.Classes, Does.Not.Contain(":clickable"));
        }

        [AvaloniaTest]
        public void It_is_not_focusable()
        {
            var sut = new Card();

            Assert.That(sut.Focusable, Is.False);
        }
    }

    public class When_A_Command_Is_Set : CardTests
    {
        [AvaloniaTest]
        public void It_becomes_clickable()
        {
            var command = new Mock<ICommand>();
            command.Setup(c => c.CanExecute(It.IsAny<object?>())).Returns(true);

            var sut = new Card { Command = command.Object };

            Assert.That(sut.Classes, Does.Contain(":clickable"));
        }

        [AvaloniaTest]
        public void It_becomes_focusable()
        {
            var command = new Mock<ICommand>();
            command.Setup(c => c.CanExecute(It.IsAny<object?>())).Returns(true);

            var sut = new Card { Command = command.Object };

            Assert.That(sut.Focusable, Is.True);
        }
    }

    public class When_IsClickable_Override_Is_Set : CardTests
    {
        [AvaloniaTest]
        public void It_is_clickable_without_a_command()
        {
            var sut = new Card { IsClickable = true };

            Assert.That(sut.Classes, Does.Contain(":clickable"));
        }

        [AvaloniaTest]
        public void It_is_not_clickable_when_forced_off_with_a_command()
        {
            var command = new Mock<ICommand>();
            command.Setup(c => c.CanExecute(It.IsAny<object?>())).Returns(true);

            var sut = new Card { Command = command.Object, IsClickable = false };

            Assert.That(sut.Classes, Does.Not.Contain(":clickable"));
        }
    }
}
