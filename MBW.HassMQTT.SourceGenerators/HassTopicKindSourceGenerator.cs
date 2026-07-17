#nullable enable

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace MBW.HassMQTT.SourceGenerators;

[Generator(LanguageNames.CSharp)]
public sealed class HassTopicKindSourceGenerator : IIncrementalGenerator
{
    private const string DiscoveryDocumentInterface = "MBW.HassMQTT.DiscoveryModels.Interfaces.IHassDiscoveryDocument";
    private const string TopicSuffix = "Topic";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValuesProvider<string> topicNames = context.SyntaxProvider
            .CreateSyntaxProvider(
                static (node, _) => node is PropertyDeclarationSyntax property &&
                                    property.Identifier.ValueText.EndsWith(TopicSuffix, StringComparison.Ordinal),
                static (syntaxContext, cancellationToken) => GetTopicName(syntaxContext, cancellationToken))
            .Where(static name => name is not null)
            .Select(static (name, _) => name!);

        IncrementalValueProvider<ImmutableArray<string>> collectedTopicNames = topicNames.Collect();
        context.RegisterSourceOutput(collectedTopicNames, static (productionContext, names) =>
        {
            productionContext.CancellationToken.ThrowIfCancellationRequested();
            productionContext.AddSource(
                "HassTopicKind_generated.cs",
                SourceText.From(Render(names, productionContext.CancellationToken), Encoding.UTF8));
        });
    }

    private static string? GetTopicName(GeneratorSyntaxContext context, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        IPropertySymbol? property = context.SemanticModel.GetDeclaredSymbol(
            (PropertyDeclarationSyntax)context.Node,
            cancellationToken) as IPropertySymbol;

        if (property is null || !ImplementsDiscoveryDocument(property.ContainingType, cancellationToken))
            return null;

        string name = property.Name.Substring(0, property.Name.Length - TopicSuffix.Length);
        return name.Length == 0 || string.Equals(name, "Publish", StringComparison.Ordinal) ? null : name;
    }

    private static bool ImplementsDiscoveryDocument(INamedTypeSymbol type, CancellationToken cancellationToken)
    {
        foreach (INamedTypeSymbol implementedInterface in type.AllInterfaces)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (string.Equals(
                    implementedInterface.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat),
                    DiscoveryDocumentInterface,
                    StringComparison.Ordinal))
                return true;
        }

        return false;
    }

    private static string Render(ImmutableArray<string> topicNames, CancellationToken cancellationToken)
    {
        SortedSet<string> sortedTopicNames = new(StringComparer.Ordinal);
        foreach (string topicName in topicNames)
        {
            cancellationToken.ThrowIfCancellationRequested();
            sortedTopicNames.Add(topicName);
        }

        StringBuilder builder = new();
        builder.Append("using System.Runtime.Serialization;\n\n");
        builder.Append("namespace MBW.HassMQTT.DiscoveryModels.Enum\n");
        builder.Append("{\n");
        builder.Append("  public enum HassTopicKind\n");
        builder.Append("  {\n");

        foreach (string topicName in sortedTopicNames)
        {
            cancellationToken.ThrowIfCancellationRequested();
            builder.Append("    [EnumMember(Value = \"");
            AppendSnakeCase(builder, topicName);
            builder.Append("\")]\n");
            builder.Append("    ").Append(topicName).Append(",\n");
        }

        builder.Append("  }\n");
        builder.Append("}\n");
        return builder.ToString();
    }

    private static void AppendSnakeCase(StringBuilder builder, string value)
    {
        for (int index = 0; index < value.Length; index++)
        {
            char character = value[index];
            if (character is >= 'A' and <= 'Z')
            {
                if (index > 0)
                    builder.Append('_');

                builder.Append((char)(character + ('a' - 'A')));
            }
            else
            {
                builder.Append(character);
            }
        }
    }
}
