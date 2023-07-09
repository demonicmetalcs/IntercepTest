using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Diagnostics;

namespace Interceptest;

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

public class InterceptestMockAttributeSyntaxReceiver : ISyntaxReceiver
{
    public List<LocalFunctionStatementSyntax> CandidateMethods { get; } = new List<LocalFunctionStatementSyntax>();
    public Dictionary<MethodAccessExpressionKey, Location> MemberAccessExpressions { get; } = new Dictionary<MethodAccessExpressionKey, Location>();

    /// <summary>
    /// Called for every syntax node in the compilation, we can inspect the nodes and save any information useful for generation
    /// </summary>
    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {

        // any method with at least one attribute is a candidate for property generation
        if (syntaxNode is LocalFunctionStatementSyntax localFunctionStatementSyntax)            
        {
            if (localFunctionStatementSyntax.AttributeLists.Select(al => al.Attributes.FirstOrDefault(a => ((IdentifierNameSyntax)a.Name).Identifier.ValueText == "InterceptestMockAttribute" || ((IdentifierNameSyntax)a.Name).Identifier.ValueText == "InterceptestMock")).FirstOrDefault() != null)
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