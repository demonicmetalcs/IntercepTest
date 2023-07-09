using Microsoft.CodeAnalysis;

namespace Interceptest;

[Generator]
public class InterceptsLocationAttributeGenerator : ISourceGenerator
{
    //todo replace when InterceptsLocationAttribute is available in dot net core
    private static readonly string s_attributesSource = (@"namespace System.Runtime.CompilerServices;

        [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
        public sealed class InterceptsLocationAttribute : Attribute
        {
            public InterceptsLocationAttribute(string filePath, int line, int character)
            {
            }
        }
        ");

    public void Execute(GeneratorExecutionContext context)
    {
        context.AddSource("InterceptestInterceptsLocationAttribute", s_attributesSource);
    }

    public void Initialize(GeneratorInitializationContext context)
    {
        
    }
}
