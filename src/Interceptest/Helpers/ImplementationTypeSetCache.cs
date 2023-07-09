using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Collections.Immutable;
using System;
using System.Linq;

namespace Interceptest.Helpers;

internal interface IImplementationTypeSetCache
{
    IImmutableSet<INamedTypeSymbol> All { get; }

    IImmutableSet<INamedTypeSymbol> ForAssembly(IAssemblySymbol assembly);
}

internal class ImplementationTypeSetCache : IImplementationTypeSetCache
{
    private readonly GeneratorExecutionContext _context;
    private readonly Lazy<IImmutableSet<INamedTypeSymbol>> _all;
    private IImmutableDictionary<IAssemblySymbol, IImmutableSet<INamedTypeSymbol>> _assemblyCache =
        ImmutableDictionary<IAssemblySymbol, IImmutableSet<INamedTypeSymbol>>.Empty;

    private readonly string _currentAssemblyName;

    internal ImplementationTypeSetCache(
        GeneratorExecutionContext context)
    {
        _context = context;
        _currentAssemblyName = context.Compilation.AssemblyName ?? "";
        _all = new Lazy<IImmutableSet<INamedTypeSymbol>>(
            () => context
                .Compilation
                .SourceModule
                .ReferencedAssemblySymbols
                .Prepend(_context.Compilation.Assembly)
                .SelectMany(ForAssembly)
                .ToImmutableHashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default));
    }

    public IImmutableSet<INamedTypeSymbol> All => _all.Value;
    public IImmutableSet<INamedTypeSymbol> ForAssembly(IAssemblySymbol assembly)
    {
        if (_assemblyCache.TryGetValue(assembly, out var set)) return set;

        var freshSet = GetImplementationsFrom(assembly);
        _assemblyCache = _assemblyCache.Add(assembly, freshSet);
        return freshSet;
    }

    private IImmutableSet<INamedTypeSymbol> GetImplementationsFrom(IAssemblySymbol assemblySymbol)
    {
        var internalsAreVisible =
            SymbolEqualityComparer.Default.Equals(_context.Compilation.Assembly, assemblySymbol)
            || assemblySymbol
                .GetAttributes()
                .Any(ad =>
                    ad.ConstructorArguments.Length == 1
                    && ad.ConstructorArguments[0].Value is string assemblyName
                    && Equals(assemblyName, _currentAssemblyName));

        return GetAllNamespaces(assemblySymbol.GlobalNamespace)
            .SelectMany(ns => ns.GetTypeMembers())
            .SelectMany(t => AllNestedTypesAndSelf(t))
            .Where(nts => nts is
            {
                IsAbstract: false,
                IsStatic: false,
                IsImplicitClass: false,
                IsScriptClass: false,
                TypeKind: TypeKind.Class or TypeKind.Struct or TypeKind.Structure,
                DeclaredAccessibility: Accessibility.Public or Accessibility.Internal or Accessibility.ProtectedOrInternal
            })
            .Where(nts =>
                !nts.Name.StartsWith("<"))
            .ToImmutableHashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);
    }

    private static IEnumerable<INamespaceSymbol> GetAllNamespaces(INamespaceSymbol root)
    {
        yield return root;
        foreach (var child in root.GetNamespaceMembers())
            foreach (var next in GetAllNamespaces(child))
                yield return next;
    }

    private static IEnumerable<INamedTypeSymbol> AllNestedTypesAndSelf(INamedTypeSymbol type)
    {
        yield return type;
        foreach (var typeMember in type.GetTypeMembers())
        {
            foreach (var nestedType in AllNestedTypesAndSelf(typeMember))
            {
                yield return nestedType;
            }
        }
    }


}
