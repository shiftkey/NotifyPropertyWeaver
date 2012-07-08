using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Mono.Cecil;


[Export, PartCreationPolicy(CreationPolicy.Shared)]
public class TypeEqualityFinder
{
    ModuleReader moduleReader;
    MsCoreReferenceFinder msCoreReferenceFinder;
    TypeResolver typeResolver;
    Dictionary<string, MethodReference> methodCache;

    [ImportingConstructor]
    public TypeEqualityFinder(ModuleReader moduleReader, MsCoreReferenceFinder msCoreReferenceFinder, TypeResolver typeResolver)
    {
        methodCache = new Dictionary<string, MethodReference>();

        this.moduleReader = moduleReader;
        this.msCoreReferenceFinder = msCoreReferenceFinder;
        this.typeResolver = typeResolver;
    }
    

    public MethodReference Find(TypeReference typeDefinition)
    {
        MethodReference methodReference;
        var fullName = typeDefinition.FullName;
        if (methodCache.TryGetValue(fullName, out methodReference))
        {
            return methodReference;
        }

        var equality = GetEquality(typeDefinition);
        methodCache.Add(fullName, equality);
        return equality;
    }

    MethodReference GetEquality(TypeReference typeDefinition)
    {

        if (typeDefinition.IsGenericParameter)
        {
            return null;
        }
        if (typeDefinition.Namespace.StartsWith("System.Collections"))
        {
            return null;
        }
        if (typeDefinition.IsArray)
        {
            return null;
        }
        if (typeDefinition.IsGenericInstance)
        {
            if (typeDefinition.FullName.StartsWith("System.Nullable"))
            {
                var typeWrappedByNullable = ((GenericInstanceType) typeDefinition).GenericArguments.First();
                var genericInstanceMethod = new GenericInstanceMethod(msCoreReferenceFinder.NullableEqualsMethod);
                genericInstanceMethod.GenericArguments.Add(typeWrappedByNullable);
                return moduleReader.Module.Import(genericInstanceMethod);
            }
        }
        var equality = GetStaticEquality(typeDefinition);
        if (equality != null)
        {
            return moduleReader.Module.Import(equality);
        }
        return null;
    }

    MethodReference GetStaticEquality(TypeReference typeReference)
    {
        var typeDefinition = typeResolver.Resolve(typeReference);
        if (typeDefinition.IsInterface)
        {
            return null;
        }

        return FindNamedMethod(typeDefinition);
    }

    public static MethodReference FindNamedMethod(TypeDefinition typeDefinition)
    {
        var equalsMethod = FindNamedMethod(typeDefinition, "Equals");
        if (equalsMethod == null)
        {
            return FindNamedMethod(typeDefinition, "op_Equality");
        }
        return equalsMethod;
    }

    static MethodReference FindNamedMethod(TypeDefinition typeDefinition, string methodName)
    {
        return typeDefinition.Methods.FirstOrDefault(x => x.Name == methodName &&
                                                          x.IsStatic &&
                                                          x.HasParameters &&
                                                          x.ReturnType.Name=="Boolean" &&
                                                          x.Parameters.Count == 2 &&
                                                          x.Parameters[0].ParameterType == typeDefinition &&
                                                          x.Parameters[1].ParameterType == typeDefinition);
    }
}