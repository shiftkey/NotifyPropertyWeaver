using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Mono.Cecil;

[Export, PartCreationPolicy(CreationPolicy.Shared)]
public class MethodGenerifier
{
    ModuleReader moduleReader;

    [ImportingConstructor]
    public MethodGenerifier(ModuleReader moduleReader)
    {
        this.moduleReader = moduleReader;
    }

    public MethodReference GetMethodReference(Stack<TypeDefinition> typeDefinitions, MethodDefinition methodDefinition)
    {
        try
        {
            var methodReference = moduleReader.Module.Import(methodDefinition).GetGeneric();
            typeDefinitions.Pop();
            while (typeDefinitions.Count > 0)
            {
                var definition = typeDefinitions.Pop();
                methodReference = MakeGeneric(definition.BaseType, methodReference);
            }
            return methodReference;
        }
        catch (Exception exception)
        {
            throw new Exception(string.Format("Could not make method generic '{0}'.", methodDefinition.GetName()), exception);
        }
    }

    public static MethodReference MakeGeneric(TypeReference declaringType, MethodReference self)
    {
        var reference = new MethodReference(self.Name, self.ReturnType)
                            {
                                DeclaringType = declaringType,
                                HasThis = self.HasThis,
                                ExplicitThis = self.ExplicitThis,
                                CallingConvention = self.CallingConvention,
                            };

        foreach (var parameter in self.Parameters)
        {
            reference.Parameters.Add(new ParameterDefinition(parameter.ParameterType));
        }

        return reference;
    }

}