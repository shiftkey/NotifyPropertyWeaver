using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Mono.Cecil;

[Export, PartCreationPolicy(CreationPolicy.Shared)]
public class TypeResolver
{
    Dictionary<string, TypeDefinition> definitions;

    [ImportingConstructor]
    public TypeResolver()
    {
        definitions = new Dictionary<string, TypeDefinition>();
    }

    public TypeDefinition Resolve(TypeReference reference)
    {
        TypeDefinition definition;
        if (definitions.TryGetValue(reference.FullName, out definition))
        {
            return definition;
        }
        return definitions[reference.FullName] = InnerResolve(reference);
    }

    static TypeDefinition InnerResolve(TypeReference reference)
    {
        try
        {
            return reference.Resolve();
        }
        catch (Exception exception)
        {
            throw new Exception(string.Format("Could not resolve '{0}'.", reference.FullName), exception);
        }
    }
}