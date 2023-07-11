using Microsoft.CodeAnalysis;

[Generator]
public class IntercepTestProjectAttributeGenerator : ISourceGenerator
{
    //todo replace when InterceptsLocationAttribute is available in dot net core
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