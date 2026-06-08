using Avalonia;
using Avalonia.Headless;
using CCSWE.Avalonia.Material;

[assembly: AvaloniaTestApplication(typeof(CCSWE.Avalonia.Material.UnitTests.TestApp))]

namespace CCSWE.Avalonia.Material.UnitTests;

/// <summary>
/// The headless test application. It applies the library's <see cref="MaterialTheme"/> (not Fluent) so every
/// control under test is themed exactly as a consuming app would see it.
/// </summary>
public sealed class TestApp : Application
{
    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<TestApp>().UseHeadless(new AvaloniaHeadlessPlatformOptions());

    public override void Initialize()
    {
        Styles.Add(new MaterialTheme());
    }
}
