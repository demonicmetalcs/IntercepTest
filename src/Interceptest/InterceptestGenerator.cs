using Microsoft.CodeAnalysis;

namespace Interceptest;

[Generator]
public class InterceptestGenerator : ISourceGenerator
{
    public static string Template()
    {
        return @"
using SampleProject;
using System.Runtime.CompilerServices;
namespace Interceptest
{
    public static class InterceptestGenerated
    {
        [InterceptsLocation(""D:\\Github\\Interceptest\\src\\Sample\\SimpleSample\\SampleProject\\Program.cs"", line: 18, character: 9)]
        public static void InterceptorMethod(this Test p, string name)
        {
            Console.WriteLine($""interceptor {name}"");
        }
    }
}";
    }

    public void Initialize(GeneratorInitializationContext context)
    {

    }
    public void Execute(GeneratorExecutionContext context)
    {
        context.AddSource("InterceptsLocation", Template());
    }
}

