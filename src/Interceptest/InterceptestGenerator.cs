using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Immutable;
using Interceptest.Helpers;
using System.Reflection;

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

        var implementationTypeSetCache = new ImplementationTypeSetCache(context);
        var alltest = implementationTypeSetCache.ForAssembly(context.Compilation.Assembly);
        var syntaxFactory = SyntaxFactory.CompilationUnit();
        syntaxFactory = syntaxFactory.AddUsings(SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System.Runtime.CompilerServices")));
        var @namespace = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName(_generateNamespace).NormalizeWhitespace());
        var classDeclaration = SyntaxFactory.ClassDeclaration(_generateClass)
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword),SyntaxFactory.Token(SyntaxKind.StaticKeyword));


        foreach(var candidateMethod in receiver.CandidateMethods)
        {
            var attribute = candidateMethod.AttributeLists.Select(al => al.Attributes.First(a => ((IdentifierNameSyntax)a.Name).Identifier.ValueText == "InterceptestMockAttribute" || ((IdentifierNameSyntax)a.Name).Identifier.ValueText == "InterceptestMock")).First();
            
            //todo clean up
            var typeToMockExpression = (IdentifierNameSyntax)((TypeOfExpressionSyntax)attribute.ArgumentList.Arguments[0].Expression).Type;
            var functionToMockExpression = (MemberAccessExpressionSyntax)((InvocationExpressionSyntax)attribute.ArgumentList.Arguments[1].Expression).ArgumentList.Arguments[0].Expression;
            var typeToMock = alltest.First(t => t.Name == typeToMockExpression.Identifier.ValueText);
            var memberToMock = typeToMock.GetMembers().OfType<IMethodSymbol>().FirstOrDefault(m => m.Name == ((IdentifierNameSyntax)functionToMockExpression.Name).Identifier.ValueText);


            var callingTypeExpression = (IdentifierNameSyntax)((TypeOfExpressionSyntax)attribute.ArgumentList.Arguments[2].Expression).Type;
            var callingFunctionExpression = (MemberAccessExpressionSyntax)((InvocationExpressionSyntax)attribute.ArgumentList.Arguments[3].Expression).ArgumentList.Arguments[0].Expression;
            var callingType = alltest.First(t => t.Name == callingTypeExpression.Identifier.ValueText);
            var callingFunction = callingType.GetMembers().OfType<IMethodSymbol>().FirstOrDefault(m => m.Name == ((IdentifierNameSyntax)callingFunctionExpression.Name).Identifier.ValueText);
            var begin = callingFunction.DeclaringSyntaxReferences[0].Span.Start;
            var end = callingFunction.DeclaringSyntaxReferences[0].Span.End;
            var method = callingFunction.DeclaringSyntaxReferences[0].SyntaxTree.ToString().Substring(begin, end - begin + 1);

            var model = context.Compilation.GetSemanticModel(callingFunction.DeclaringSyntaxReferences[0].SyntaxTree);

            var syntaxReference = callingFunction.DeclaringSyntaxReferences.FirstOrDefault().GetSyntax() as MethodDeclarationSyntax;

            var location = receiver.MemberAccessExpressions[new MethodAccessExpressionKey(callingTypeExpression.Identifier.ValueText, ((IdentifierNameSyntax)callingFunctionExpression.Name).Identifier.ValueText, typeToMockExpression.Identifier.ValueText, ((IdentifierNameSyntax)functionToMockExpression.Name).Identifier.ValueText)].GetLineSpan();
                     

            var testMethod = candidateMethod.Parent.Parent;
            var testClass = candidateMethod.Parent.Parent.Parent as ClassDeclarationSyntax;

            var parameters = new List<ParameterSyntax>{
                            SyntaxFactory.Parameter(SyntaxFactory.Identifier("p"))
                            .WithModifiers(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.ThisKeyword)))
                            .WithType(SyntaxFactory.ParseTypeName(typeToMock.ToDisplayString(new SymbolDisplayFormat(typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces))))};

            parameters.AddRange(candidateMethod.ParameterList.Parameters);

            

            var methodDeclaration = SyntaxFactory.MethodDeclaration(SyntaxFactory.ParseTypeName("int"), "InterceptorMethod2")
            .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword), SyntaxFactory.Token(SyntaxKind.StaticKeyword)) 
            .WithParameterList(SyntaxFactory.ParameterList(SyntaxFactory.SeparatedList(parameters)))
            .AddAttributeLists(SyntaxFactory.AttributeList(SyntaxFactory.SeparatedList(new List<AttributeSyntax>
            {
                SyntaxFactory.Attribute(SyntaxFactory.ParseName("InterceptsLocation"),
                    SyntaxFactory.AttributeArgumentList(SyntaxFactory.SeparatedList(new List<AttributeArgumentSyntax>
                    {
                        SyntaxFactory.AttributeArgument(SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression,SyntaxFactory.Literal(callingFunction.DeclaringSyntaxReferences[0].SyntaxTree.FilePath))),
                        SyntaxFactory.AttributeArgument(SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression,SyntaxFactory.Literal(location.StartLinePosition.Line + 1))),
                        SyntaxFactory.AttributeArgument(SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression,SyntaxFactory.Literal(location.StartLinePosition.Character + 1)))
                    })))
            })));

            methodDeclaration = methodDeclaration.AddBodyStatements(candidateMethod.Body);

            classDeclaration = classDeclaration.AddMembers(methodDeclaration);
            @namespace = @namespace.AddMembers(classDeclaration);
            syntaxFactory = syntaxFactory.AddMembers(@namespace);
            context.AddSource("Interceptest", syntaxFactory.NormalizeWhitespace().ToFullString());

        }


        
    }
}

