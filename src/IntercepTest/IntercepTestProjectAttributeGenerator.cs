using Microsoft.CodeAnalysis;

namespace IntercepTest;

[Generator]
public class IntercepTestProjectAttributeGenerator : ISourceGenerator
{
    private static readonly string s_attributesSource = (@"namespace System.Runtime.CompilerServices;

        [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
        public sealed class IntercepTestProjectAttribute : Attribute
        {
            public IntercepTestProjectAttribute(string filePath)
            {
            }
        }
        ");

    public void Execute(GeneratorExecutionContext context)
    {
        context.AddSource("IntercepTestInterceptsLocationAttribute", s_attributesSource);
    }

    public void Initialize(GeneratorInitializationContext context)
    {

    }
}