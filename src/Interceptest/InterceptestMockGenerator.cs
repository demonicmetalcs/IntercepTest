using Microsoft.CodeAnalysis;

namespace Interceptest;

[Generator]
public class InterceptestMockGenerator : ISourceGenerator
{
    private static readonly string s_attributesSource = (@"namespace Interceptest;

        [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
        public sealed class InterceptestMockAttribute : Attribute
        {
            public InterceptestMockAttribute(System.Type typeToMock, string functionToMock, System.Type callingType, string callingFunction)
            {
            }
        }
        ");

    public void Initialize(GeneratorInitializationContext context){}

    public void Execute(GeneratorExecutionContext context)
    {
        context.AddSource("InterceptestMockAttribute", s_attributesSource);
    }    
}
