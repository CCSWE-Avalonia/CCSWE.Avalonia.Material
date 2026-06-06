using System;
using System.Globalization;

namespace CCSWE.Avalonia.Material.Demo;

/// <summary>
/// Command-line options for the gallery "record" mode — a hands-free tour (smooth-scroll every
/// section, switch Dark→Light, repeat) used to capture the README GIF with an external recorder
/// such as ShareX. Enabled with <c>--record</c>; the start delay gives time to begin recording.
/// </summary>
public sealed class DemoOptions
{
    /// <summary>Pause at the top/bottom of each section and after switching theme.</summary>
    public TimeSpan Dwell { get; init; } = TimeSpan.FromMilliseconds(500);

    /// <summary>Repeat the whole Dark→Light tour until the app is closed.</summary>
    public bool Loop { get; init; }

    /// <summary>Run the hands-free tour on startup.</summary>
    public bool Record { get; init; }

    /// <summary>Scroll step in DIPs per ~16 ms frame (smaller = slower / smoother).</summary>
    public double ScrollSpeed { get; init; } = 10;

    /// <summary>Delay before the tour starts, so an external recorder can be armed first.</summary>
    public TimeSpan StartDelay { get; init; } = TimeSpan.FromSeconds(15);

    public static DemoOptions Parse(string[]? args)
    {
        args ??= [];

        // Seed from the property defaults so they're the single source of truth.
        var defaults = new DemoOptions();
        var dwell = defaults.Dwell;
        var loop = defaults.Loop;
        var record = defaults.Record;
        var scrollSpeed = defaults.ScrollSpeed;
        var startDelay = defaults.StartDelay;

        for (var i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "--record":
                    record = true;
                    break;
                case "--loop":
                    loop = true;
                    break;
                case "--start-delay" when TryNext(args, ref i, out var sd):
                    startDelay = TimeSpan.FromSeconds(sd);
                    break;
                case "--scroll-speed" when TryNext(args, ref i, out var ss):
                    scrollSpeed = ss;
                    break;
                case "--dwell" when TryNext(args, ref i, out var dw):
                    dwell = TimeSpan.FromMilliseconds(dw);
                    break;
            }
        }

        return new DemoOptions
        {
            Dwell = dwell,
            Loop = loop,
            Record = record,
            ScrollSpeed = scrollSpeed,
            StartDelay = startDelay,
        };
    }

    private static bool TryNext(string[] args, ref int i, out double value)
    {
        value = 0;
        if (i + 1 >= args.Length || !double.TryParse(args[i + 1], NumberStyles.Any, CultureInfo.InvariantCulture, out value))
        {
            return false;
        }

        i++;
        return true;
    }
}
