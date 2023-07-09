using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using Microsoft.CodeAnalysis.CSharp;

namespace Interceptest;

public class InterceptestMockAttributeSyntaxReceiver : ISyntaxReceiver
{
    public List<LocalFunctionStatementSyntax> CandidateMethods { get; } = new List<LocalFunctionStatementSyntax>();
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
    }
}