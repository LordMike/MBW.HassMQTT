using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace MBW.HassMQTT.SourceGenerators
{
    [Generator]
    public class HassTopicKindSourceGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
//#if DEBUG
//            if (!Debugger.IsAttached)
//                Debugger.Launch();
//#endif

            ICollection<string> typeNames = context.Compilation.Assembly.TypeNames;

            HashSet<string> topicNames = new HashSet<string>(StringComparer.Ordinal);
            foreach (string typeName in typeNames)
            {
                INamedTypeSymbol p = context.Compilation.GetSymbolsWithName(typeName).FirstOrDefault() as INamedTypeSymbol;
                if (p == null)
                    continue;

                if (!p.AllInterfaces.Any(x => x.Name == "IHassDiscoveryDocument"))
                    continue;

                List<ISymbol> props = p.GetMembers()
                    .Where(s => s.Name.EndsWith("Topic") && s.Kind == SymbolKind.Property)
                    .ToList();

                foreach (ISymbol symbol in props)
                {
                    string name = symbol.Name.Replace("Topic", "");
                    topicNames.Add(name);
                }
            }

            // Remove some names
            topicNames.Remove("");
            topicNames.Remove("Publish");

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("using System.Runtime.Serialization;");
            sb.AppendLine("");
            sb.AppendLine("namespace MBW.HassMQTT.DiscoveryModels.Enum");
            sb.AppendLine("{");
            sb.AppendLine("  public enum HassTopicKind");
            sb.AppendLine("  {");

            foreach (string topicName in topicNames.OrderBy(s => s))
            {
                string value = Regex.Replace(topicName, "[A-Z]", match =>
                {
                    if (match.Index == 0)
                        return match.Value.ToLower();
                    return "_" + match.Value.ToLower();
                });

                sb.AppendLine($"    [EnumMember(Value = \"{value}\")]");
                sb.AppendLine($"    {topicName},");
            }

            sb.AppendLine("  }");
            sb.AppendLine("}");

            context.AddSource("HassTopicKind_generated.cs", SourceText.From(sb.ToString(), Encoding.UTF8));
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            // No initialization required for this one
        }
    }
}
