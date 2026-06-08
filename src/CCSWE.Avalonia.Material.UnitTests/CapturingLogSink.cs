using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Logging;

namespace CCSWE.Avalonia.Material.UnitTests;

/// <summary>
/// An <see cref="ILogSink"/> that captures Warning/Error log entries while installed as
/// <see cref="Logger.Sink"/>, so a test can assert that rendering a themed control produced no
/// binding/resource/template failures. Platform/font noise is excluded so a clean theme yields zero entries.
/// </summary>
public sealed class CapturingLogSink : ILogSink, IDisposable
{
    // Area names are plain strings in Avalonia; literals avoid depending on which LogArea
    // constants exist in a given version. Platform/font/animation noise is filtered out.
    private static readonly FrozenSet<string> IgnoredAreas = new[]
    {
        "Animations",
        "Win32Platform",
        "X11Platform",
        "AndroidPlatform",
        "IOSPlatform",
        "FreeType",
        "Fonts",
    }.ToFrozenSet(StringComparer.Ordinal);

    private readonly List<string> _entries = [];
    private readonly ILogSink? _previous;

    private CapturingLogSink(ILogSink? previous)
    {
        _previous = previous;
    }

    public IReadOnlyList<string> Entries => _entries;

    /// <summary>Installs a fresh capturing sink as <see cref="Logger.Sink"/>; dispose to restore the previous sink.</summary>
    public static CapturingLogSink Install()
    {
        var sink = new CapturingLogSink(Logger.Sink);
        Logger.Sink = sink;
        return sink;
    }

    public void Dispose() => Logger.Sink = _previous;

    public bool IsEnabled(LogEventLevel level, string area) =>
        level >= LogEventLevel.Warning && !IgnoredAreas.Contains(area);

    public void Log(LogEventLevel level, string area, object? source, string messageTemplate) =>
        Record(level, area, messageTemplate, []);

    public void Log(LogEventLevel level, string area, object? source, string messageTemplate, params object?[] propertyValues) =>
        Record(level, area, messageTemplate, propertyValues);

    public void Log<T0>(LogEventLevel level, string area, object? source, string messageTemplate, T0 propertyValue0) =>
        Record(level, area, messageTemplate, [propertyValue0]);

    public void Log<T0, T1>(LogEventLevel level, string area, object? source, string messageTemplate, T0 propertyValue0, T1 propertyValue1) =>
        Record(level, area, messageTemplate, [propertyValue0, propertyValue1]);

    public void Log<T0, T1, T2>(LogEventLevel level, string area, object? source, string messageTemplate, T0 propertyValue0, T1 propertyValue1, T2 propertyValue2) =>
        Record(level, area, messageTemplate, [propertyValue0, propertyValue1, propertyValue2]);

    private void Record(LogEventLevel level, string area, string messageTemplate, object?[] values)
    {
        if (!IsEnabled(level, area))
        {
            return;
        }

        var message = values.Length == 0 ? messageTemplate : $"{messageTemplate} [{string.Join(", ", values.Select(v => v?.ToString()))}]";
        _entries.Add($"[{level}/{area}] {message}");
    }
}
