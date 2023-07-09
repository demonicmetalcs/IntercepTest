using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Interceptest;

[Generator]
public class InterceptestGenerator : ISourceGenerator
{

    private static readonly string _generateNamespace = "Interceptest";
    private static readonly string _generateClass = "InterceptestGenerated";



    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new InterceptestMockAttributeSyntaxReceiver());
    }
    public void Execute(GeneratorExecutionContext context)
    {
        if (!(context.SyntaxReceiver is InterceptestMockAttributeSyntaxReceiver receiver))
        {
            return;
        }
        //context.AddSource("InterceptsLocation", Template());


        var syntaxFactory = SyntaxFactory.CompilationUnit();
        syntaxFactory = syntaxFactory.AddUsings(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System.Runtime.CompilerServices")), SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("SampleProject")));
        var @namespace = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName(_generateNamespace).NormalizeWhitespace());
        var classDeclaration = SyntaxFactory.ClassDeclaration(_generateClass)
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword),SyntaxFactory.Token(SyntaxKind.StaticKeyword));


        foreach(var candidateMethod in receiver.CandidateMethods)
        {
            var attribute = candidateMethod.AttributeLists.Select(al => al.Attributes.First(a => ((IdentifierNameSyntax)a.Name).Identifier.ValueText == "InterceptestMockAttribute" || ((IdentifierNameSyntax)a.Name).Identifier.ValueText == "InterceptestMock")).First();


            var methodDeclaration = SyntaxFactory.MethodDeclaration(SyntaxFactory.ParseTypeName("int"), "InterceptorMethod")
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword))
            .WithParameterList(SyntaxFactory.ParameterList(SyntaxFactory.SeparatedList(
                        new List<ParameterSyntax>{
                            SyntaxFactory.Parameter(SyntaxFactory.Identifier("p"))
                            .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.ThisKeyword)))
                            .WithType(SyntaxFactory.IdentifierName("ServiceToMock")),
                            SyntaxFactory.Parameter(SyntaxFactory.Identifier("x"))
                            .WithType(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.IntKeyword)))})))
            .AddAttributeLists(SyntaxFactory.AttributeList(SyntaxFactory.SeparatedList(new List<AttributeSyntax>
            {
                SyntaxFactory.Attribute(SyntaxFactory.ParseName("InterceptsLocation"),
                    SyntaxFactory.AttributeArgumentList(SyntaxFactory.SeparatedList(new List<AttributeArgumentSyntax>
                    {
                        SyntaxFactory.AttributeArgument(SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression,SyntaxFactory.Literal("D:\\Github\\Interceptest\\src\\Sample\\SimpleSample\\SampleProject\\ControllerToTest.cs"))),
                        SyntaxFactory.AttributeArgument(SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression,SyntaxFactory.Literal(15))),
                        SyntaxFactory.AttributeArgument(SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression,SyntaxFactory.Literal(35)))
                    })))
            })));

            methodDeclaration = methodDeclaration.AddBodyStatements(candidateMethod.Body);

            classDeclaration = classDeclaration.AddMembers(methodDeclaration);
            @namespace = @namespace.AddMembers(classDeclaration);
            syntaxFactory = syntaxFactory.AddMembers(@namespace);
            context.AddSource("Interceptest", syntaxFactory.NormalizeWhitespace().ToFullString());

        }


        
    }

    private string GetInterceptorFilePath(SyntaxTree tree, Compilation compilation)
    {
        return compilation.Options.SourceReferenceResolver?.NormalizePath(tree.FilePath, baseFilePath: null) ?? tree.FilePath;
    }
}

