using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Avalonia;
using Avalonia.Headless.NUnit;
using Avalonia.Styling;
using NUnit.Framework;

namespace CCSWE.Avalonia.Material.UnitTests;

/// <summary>
/// Validates that every <c>{DynamicResource Key}</c> referenced by the library's XAML resolves against the
/// applied <see cref="MaterialTheme"/> in both theme variants. Unresolved <c>DynamicResource</c> keys are
/// silent at runtime (no exception, no log), so the render smoke test cannot catch them - this closes that
/// gap. By the library convention, <c>DynamicResource</c> is always a global token ref (a typo'd key would
/// otherwise ship as a silently-missing color/metric), while file-local refs use <c>StaticResource</c>.
/// </summary>
[SuppressMessage("ReSharper", "InconsistentNaming")]
public class ResourceReferenceTests
{
    private static readonly Regex DynamicResourceRef =
        new(@"\{DynamicResource\s+([A-Za-z_][A-Za-z0-9_]*)\s*\}", RegexOptions.Compiled);

    [AvaloniaTest]
    public void Every_dynamic_resource_key_resolves_in_both_variants()
    {
        var app = Application.Current!;
        var keys = CollectDynamicResourceKeys();

        Assert.That(keys, Is.Not.Empty, "No DynamicResource references were found - the embedded-XAML scan is broken.");

        var unresolved = new List<string>();

        foreach (var key in keys)
        {
            foreach (var variant in new[] { ThemeVariant.Dark, ThemeVariant.Light })
            {
                if (!app.TryGetResource(key, variant, out _))
                {
                    unresolved.Add($"{key} ({variant})");
                }
            }
        }

        Assert.That(unresolved, Is.Empty, "Unresolved DynamicResource keys:" + Environment.NewLine + string.Join(Environment.NewLine, unresolved));
    }

    private static IReadOnlyCollection<string> CollectDynamicResourceKeys()
    {
        // The library's raw .axaml are embedded into this test assembly (see the csproj) since the
        // compiled theme keeps no readable .axaml at runtime.
        var assembly = Assembly.GetExecutingAssembly();
        var keys = new SortedSet<string>(StringComparer.Ordinal);

        foreach (var name in assembly.GetManifestResourceNames().Where(n => n.EndsWith(".axaml", StringComparison.OrdinalIgnoreCase)))
        {
            using var stream = assembly.GetManifestResourceStream(name);
            if (stream is null)
            {
                continue;
            }

            using var reader = new StreamReader(stream);
            var text = reader.ReadToEnd();

            foreach (Match match in DynamicResourceRef.Matches(text))
            {
                keys.Add(match.Groups[1].Value);
            }
        }

        return keys;
    }
}
