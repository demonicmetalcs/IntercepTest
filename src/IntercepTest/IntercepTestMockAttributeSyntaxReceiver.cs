using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.MSBuild;

namespace IntercepTest;

public record MethodAccessExpressionKey
{
    public MethodAccessExpressionKey(string containingClass,string containingMethod, string typeToCall, string methodName)
    {
        ContainingClass = containingClass;
        ContainingMethod = containingMethod;
        TypeToCall = typeToCall;
        MethodName = methodName;
    }

    public string ContainingClass { get; }
    public string ContainingMethod { get; }
    public string TypeToCall { get; }
    public string MethodName { get; }
}

public class IntercepTestMockAttributeSyntaxReceiver : ISyntaxReceiver
{
    public List<LocalFunctionStatementSyntax> CandidateMethods { get; } = new List<LocalFunctionStatementSyntax>();
    public Dictionary<MethodAccessExpressionKey, Location> MemberAccessExpressions { get; } = new Dictionary<MethodAccessExpressionKey, Location>();

    public List<LocalFunctionStatementSyntax> CandidateMethodsOtherProject { get; } = new List<LocalFunctionStatementSyntax>();

    /// <summary>
    /// Called for every syntax node in the compilation, we can inspect the nodes and save any information useful for generation
    /// </summary>
    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        //Debugger.Launch();



        // any method with at least one attribute is a candidate for property generation
        if (syntaxNode is LocalFunctionStatementSyntax localFunctionStatementSyntax)            
        {
            if (localFunctionStatementSyntax.AttributeLists.Select(al => al.Attributes.FirstOrDefault(a => ((IdentifierNameSyntax)a.Name).Identifier.ValueText == "IntercepTestMockAttribute" || ((IdentifierNameSyntax)a.Name).Identifier.ValueText == "IntercepTestMock")).FirstOrDefault() != null)
                CandidateMethods.Add(localFunctionStatementSyntax);
        }
        if (syntaxNode is MemberAccessExpressionSyntax memberAccessExpressionSyntax)
        {
            var classToCallexpression = memberAccessExpressionSyntax.Expression as IdentifierNameSyntax;
            if (classToCallexpression == null)
            {
                return;
            }
            var containingClass = GetContainingClass(memberAccessExpressionSyntax);
            var containingMethod = GetContainingMethod(memberAccessExpressionSyntax);
            var classToCallName = classToCallexpression.Identifier.ValueText;
            var methodName = ((IdentifierNameSyntax)memberAccessExpressionSyntax.Name).Identifier.ValueText;
            var classToCall = containingClass.Members.OfType<FieldDeclarationSyntax>().Select(f => f.Declaration.Variables.FirstOrDefault(v => v.Identifier.ValueText == classToCallName)).FirstOrDefault();
            if (classToCall == null) return;
            var classToCallType = ((VariableDeclarationSyntax)classToCall.Parent).Type;
            MemberAccessExpressions.Add(new MethodAccessExpressionKey(containingClass.Identifier.ValueText, containingMethod.Identifier.ValueText, ((IdentifierNameSyntax)classToCallType).Identifier.ValueText, methodName), memberAccessExpressionSyntax.Name.GetLocation());
        }

        if (syntaxNode is AttributeSyntax attributeSyntax && ((IdentifierNameSyntax)attributeSyntax.Name).Identifier.ValueText is "IntercepTestProjectAttribute" or "IntercepTestProject")
        {
            Project project = null;
            MSBuildWorkspace workspace = null;
            try
            {
                const string projectPath =
                    @"D:\Github\Interceptest\src\Sample\SimpleSample\SampleTestProject\SampleTestProject.csproj";
                workspace = MSBuildWorkspace.Create();
                project = workspace.OpenProjectAsync(projectPath).Result;

                var documents = project.Documents;


                foreach (var document in documents)
                {
                    //Debugger.Launch();
                    var tree = document.GetSyntaxRootAsync().Result;
                    var walker = new PrintASTWalker(this);

                    if (tree is not CompilationUnitSyntax compilationUnitSyntax) continue;
                    foreach (var memberDeclarationSyntax in compilationUnitSyntax.Members)
                    {
                        walker.DefaultVisit(memberDeclarationSyntax);
                    }
                }
            }
            catch (Exception e)
            {

            }
            finally
            {
                workspace?.Dispose();
            }
        }
    }

    

    private static ClassDeclarationSyntax GetContainingClass(SyntaxNode node)
    {
        if(node == null) return null;
        if(node is ClassDeclarationSyntax classDeclarationSyntax)
        {
            return classDeclarationSyntax;
        }
        return GetContainingClass(node.Parent);
    }
    private static MethodDeclarationSyntax GetContainingMethod(SyntaxNode node)
    {
        if (node == null) return null;
        if (node is MethodDeclarationSyntax methodDeclarationSyntax)
        {
            return methodDeclarationSyntax;
        }
        return GetContainingMethod(node.Parent);
    }
}

public class PrintASTWalker : CSharpSyntaxWalker
{
    private readonly IntercepTestMockAttributeSyntaxReceiver _intercepTestMockAttributeSyntaxReceiver;

    public PrintASTWalker(IntercepTestMockAttributeSyntaxReceiver intercepTestMockAttributeSyntaxReceiver) : base(SyntaxWalkerDepth.StructuredTrivia)
    {
        _intercepTestMockAttributeSyntaxReceiver = intercepTestMockAttributeSyntaxReceiver;
    }
    public override void Visit(SyntaxNode syntaxNode)
    {
        if (syntaxNode is LocalFunctionStatementSyntax localFunctionStatementSyntax)
        {
            _intercepTestMockAttributeSyntaxReceiver.CandidateMethodsOtherProject.Add(localFunctionStatementSyntax);
        }

        base.Visit(syntaxNode);
    }
}