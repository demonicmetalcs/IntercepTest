using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;

namespace Interceptest;

[Generator]
public class InterceptestGenerator : ISourceGenerator
{

    private static readonly string _generateNamespace = "Interceptest";
    private static readonly string _generateClass = "InterceptestGenerated";

    public static string Template()
    {
        return @"
using SampleProject;
using ;
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
        //context.AddSource("InterceptsLocation", Template());



        var syntaxFactory = SyntaxFactory.CompilationUnit();
        syntaxFactory = syntaxFactory.AddUsings(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System.Runtime.CompilerServices")), SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("SampleProject")));
        var @namespace = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName(_generateNamespace).NormalizeWhitespace());
        var classDeclaration = SyntaxFactory.ClassDeclaration(_generateClass)
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword),SyntaxFactory.Token(SyntaxKind.StaticKeyword));





        var methodDeclaration = SyntaxFactory.MethodDeclaration(SyntaxFactory.ParseTypeName("void"), "InterceptorMethod")
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword))
            .WithParameterList(SyntaxFactory.ParameterList(SyntaxFactory.SeparatedList(
                        new List<ParameterSyntax>{
                            SyntaxFactory.Parameter(SyntaxFactory.Identifier("p"))
                            .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.ThisKeyword)))
                            .WithType(SyntaxFactory.IdentifierName("Test")),
                            SyntaxFactory.Parameter(SyntaxFactory.Identifier("name"))
                            .WithType(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.StringKeyword)))})))
            .AddAttributeLists(SyntaxFactory.AttributeList(SyntaxFactory.SeparatedList(new List<AttributeSyntax>
            {
                SyntaxFactory.Attribute(SyntaxFactory.ParseName("InterceptsLocation"),
                    SyntaxFactory.AttributeArgumentList(SyntaxFactory.SeparatedList(new List<AttributeArgumentSyntax>
                    {
                        SyntaxFactory.AttributeArgument(SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression,SyntaxFactory.Literal("D:\\Github\\Interceptest\\src\\Sample\\SimpleSample\\SampleProject\\Program.cs"))),
                        SyntaxFactory.AttributeArgument(SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression,SyntaxFactory.Literal(18))),
                        SyntaxFactory.AttributeArgument(SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression,SyntaxFactory.Literal(9)))
                    })))
            })));

        methodDeclaration = methodDeclaration.AddBodyStatements(SyntaxFactory.Block(
            SyntaxFactory.ExpressionStatement(SyntaxFactory.InvocationExpression(SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                            SyntaxFactory.IdentifierName("Console"),
                            SyntaxFactory.IdentifierName("WriteLine")),SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(new List<ArgumentSyntax>
                            {
                                SyntaxFactory.Argument(SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression,SyntaxFactory.Literal("Connor")))
                            }))))));

        classDeclaration = classDeclaration.AddMembers(methodDeclaration);
        @namespace = @namespace.AddMembers(classDeclaration);
        syntaxFactory = syntaxFactory.AddMembers(@namespace);
        context.AddSource("Interceptest", syntaxFactory.NormalizeWhitespace().ToFullString());
    }

    private string GetInterceptorFilePath(SyntaxTree tree, Compilation compilation)
    {
        return compilation.Options.SourceReferenceResolver?.NormalizePath(tree.FilePath, baseFilePath: null) ?? tree.FilePath;
    }
}

