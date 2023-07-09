using Microsoft.CodeAnalysis;

namespace IntercepTest;

[Generator]
public class IntercepTestMockGenerator : ISourceGenerator
{
    private static readonly string s_attributesSource = (@"namespace IntercepTest;

        [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
        public sealed class IntercepTestMockAttribute : Attribute
        {
            public IntercepTestMockAttribute(System.Type typeToMock, string functionToMock, System.Type callingType, string callingFunction)
            {
            }
        }
        ");

    public void Initialize(GeneratorInitializationContext context){}

    public void Execute(GeneratorExecutionContext context)
    {
        context.AddSource("IntercepTestMockAttribute", s_attributesSource);
    }    
}
