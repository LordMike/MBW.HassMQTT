using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading;
using MBW.HassMQTT.SourceGenerators;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Xunit;

namespace MBW.HassMQTT.Tests;

public class HassTopicKindSourceGeneratorTests
{
    private const string DiscoveryInterface = """
        namespace MBW.HassMQTT.DiscoveryModels.Interfaces
        {
            public interface IHassDiscoveryDocument { }
        }
        """;

    [Fact]
    public void Generates_topics_from_direct_and_inherited_discovery_documents()
    {
        const string source = """
            namespace Example
            {
                using MBW.HassMQTT.DiscoveryModels.Interfaces;

                public abstract class DiscoveryBase : IHassDiscoveryDocument
                {
                    public string PublishTopic { get; set; }
                    public string StateTopic { get; set; }
                }

                public sealed class Sensor : DiscoveryBase
                {
                    public string JsonAttributesTopic { get; set; }
                    public string ImageTopic { get; set; }
                    public string NotATopicSuffix { get; set; }
                }

                public sealed class Unrelated
                {
                    public string CommandTopic { get; set; }
                }
            }
            """;

        GeneratorRunResult result = Run(DiscoveryInterface, source);

        Assert.Empty(result.Diagnostics);
        Assert.Equal(Expected("Image", "JsonAttributes", "State"), GeneratedText(result));
    }

    [Fact]
    public void Deduplicates_topic_names()
    {
        const string source = """
            namespace Example
            {
                using MBW.HassMQTT.DiscoveryModels.Interfaces;
                public sealed class First : IHassDiscoveryDocument { public string StateTopic { get; set; } }
                public sealed class Second : IHassDiscoveryDocument { public string StateTopic { get; set; } }
            }
            """;

        GeneratorRunResult result = Run(DiscoveryInterface, source);

        Assert.Empty(result.Diagnostics);
        Assert.Equal(Expected("State"), GeneratedText(result));
    }

    [Fact]
    public void Output_is_deterministic_across_input_order()
    {
        const string first = """
            namespace Example
            {
                using MBW.HassMQTT.DiscoveryModels.Interfaces;
                public sealed class First : IHassDiscoveryDocument { public string StateTopic { get; set; } }
            }
            """;
        const string second = """
            namespace Example
            {
                using MBW.HassMQTT.DiscoveryModels.Interfaces;
                public sealed class Second : IHassDiscoveryDocument { public string ActionTopic { get; set; } }
            }
            """;

        string forward = GeneratedText(Run(DiscoveryInterface, first, second));
        string reverse = GeneratedText(Run(second, first, DiscoveryInterface));

        Assert.Equal(Expected("Action", "State"), forward);
        Assert.Equal(forward, reverse);
    }

    [Theory]
    [InlineData("")]
    [InlineData("namespace Example { public sealed class Incomplete { public string StateTopic { get; set; } } }")]
    [InlineData("namespace MBW.HassMQTT.DiscoveryModels.Interfaces { public interface IHassDiscoveryDocument { } } namespace Example { using MBW.HassMQTT.DiscoveryModels.Interfaces; public sealed class Empty : IHassDiscoveryDocument { public string Topic { get; set; } } }")]
    public void Empty_inputs_generate_a_valid_empty_enum(string source)
    {
        GeneratorRunResult result = Run(source);

        Assert.Empty(result.Diagnostics);
        Assert.Equal(Expected(), GeneratedText(result));
    }

    [Fact]
    public void Honors_cancellation()
    {
        CSharpCompilation compilation = CreateCompilation(DiscoveryInterface);
        GeneratorDriver driver = CSharpGeneratorDriver.Create(new HassTopicKindSourceGenerator().AsSourceGenerator());
        using CancellationTokenSource cancellation = new();
        cancellation.Cancel();

        Assert.Throws<OperationCanceledException>(() => driver.RunGenerators(compilation, cancellation.Token));
    }

    private static GeneratorRunResult Run(params string[] sources)
    {
        CSharpCompilation compilation = CreateCompilation(sources);
        GeneratorDriver driver = CSharpGeneratorDriver.Create(new HassTopicKindSourceGenerator().AsSourceGenerator());
        driver = driver.RunGeneratorsAndUpdateCompilation(compilation, out Compilation outputCompilation, out ImmutableArray<Diagnostic> diagnostics);
        Assert.Empty(diagnostics);
        Assert.DoesNotContain(outputCompilation.GetDiagnostics(), static diagnostic => diagnostic.Severity == DiagnosticSeverity.Error);
        GeneratorDriverRunResult runResult = driver.GetRunResult();
        return Assert.Single(runResult.Results);
    }

    private static CSharpCompilation CreateCompilation(params string[] sources) =>
        CSharpCompilation.Create(
            "GeneratorTests",
            sources.Select(static source => CSharpSyntaxTree.ParseText(SourceText.From(source))),
            References,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

    private static string GeneratedText(GeneratorRunResult result) =>
        Assert.Single(result.GeneratedSources).SourceText.ToString();

    private static string Expected(params string[] names)
    {
        System.Text.StringBuilder builder = new();
        builder.Append("using System.Runtime.Serialization;\n\n");
        builder.Append("namespace MBW.HassMQTT.DiscoveryModels.Enum\n{\n  public enum HassTopicKind\n  {\n");
        foreach (string name in names)
        {
            string snakeCase = string.Concat(name.Select((character, index) =>
                char.IsUpper(character) ? $"{(index > 0 ? "_" : "")}{char.ToLowerInvariant(character)}" : character.ToString()));
            builder.Append($"    [EnumMember(Value = \"{snakeCase}\")]\n    {name},\n");
        }

        return builder.Append("  }\n}\n").ToString();
    }

    private static ImmutableArray<MetadataReference> References { get; } =
        ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!)
        .Split(Path.PathSeparator)
        .Select(static path => (MetadataReference)MetadataReference.CreateFromFile(path))
        .ToImmutableArray();
}
