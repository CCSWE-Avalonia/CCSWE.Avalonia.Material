using System.Diagnostics.CodeAnalysis;
using System.Windows.Input;
using Avalonia.Automation.Peers;
using Avalonia.Automation.Provider;
using Avalonia.Headless.NUnit;
using CCSWE.Avalonia.Material;
using Moq;
using NUnit.Framework;

namespace CCSWE.Avalonia.Material.UnitTests;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public class FloatingActionButtonTests
{
    public class When_Constructed : FloatingActionButtonTests
    {
        [AvaloniaTest]
        public void It_defaults_to_regular_size()
        {
            var sut = new FloatingActionButton();

            Assert.That(sut.Size, Is.EqualTo(FloatingActionButtonSize.Regular));
        }

        [AvaloniaTest]
        public void It_defaults_to_primary_color()
        {
            var sut = new FloatingActionButton();

            Assert.That(sut.Color, Is.EqualTo(FloatingActionButtonColor.Primary));
        }
    }

    public class When_Invoked : FloatingActionButtonTests
    {
        [AvaloniaTest]
        public void It_executes_its_command()
        {
            var command = new Mock<ICommand>();
            command.Setup(c => c.CanExecute(It.IsAny<object?>())).Returns(true);

            var sut = new FloatingActionButton { Command = command.Object };
            var invoke = (IInvokeProvider)new ButtonAutomationPeer(sut);

            invoke.Invoke();

            command.Verify(c => c.Execute(It.IsAny<object?>()), Times.Once);
        }
    }
}
