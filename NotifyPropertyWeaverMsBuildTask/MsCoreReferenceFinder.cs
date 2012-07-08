using System;
using System.ComponentModel.Composition;
using System.Linq;
using Mono.Cecil;

[Export, PartCreationPolicy(CreationPolicy.Shared)]
public class MsCoreReferenceFinder
{
    ModuleReader moduleReader;
    IAssemblyResolver assemblyResolver;
    public MethodReference ComponentModelPropertyChangedEventHandlerInvokeReference;
    public MethodReference ComponentModelPropertyChangedEventConstructorReference;
    public TypeReference BoolTypeReference;
    public TypeReference VoidTypeReference;
    public TypeReference ObjectTypeReference;
    public TypeReference StringTypeReference;
    public MethodReference ActionConstructorReference;
    public MethodReference ObjectConstructor;
    public TypeReference ActionTypeReference;
    public MethodDefinition NullableEqualsMethod;
    public MethodReference GetMethodFromHandle;
    public TypeReference MethodInfoTypeReference;
    public MethodReference PropertyReference;
    public TypeReference PropChangedHandlerReference;

    [ImportingConstructor]
    public MsCoreReferenceFinder(ModuleReader moduleReader, IAssemblyResolver assemblyResolver)
    {
        this.moduleReader = moduleReader;
        this.assemblyResolver = assemblyResolver;
    }


    public void Execute()
    {
        var msCoreLibDefinition = assemblyResolver.Resolve("mscorlib");
        var msCoreTypes = msCoreLibDefinition.MainModule.Types;

        var objectDefinition = msCoreTypes.FirstOrDefault(x => x.Name == "Object");
        if (objectDefinition == null)
        {
            ExecuteWinRT();
            return;
        }
        var module = moduleReader.Module;
        ObjectTypeReference = module.Import(objectDefinition);
        var constructorDefinition = objectDefinition.Methods.First(x => x.IsConstructor);
        ObjectConstructor = module.Import(constructorDefinition);


        var nullableDefinition = msCoreTypes.FirstOrDefault(x => x.Name == "Nullable");
        NullableEqualsMethod = module.Import(nullableDefinition).Resolve().Methods.First(x => x.Name == "Equals");

        var methodBaseDefinition = msCoreTypes.FirstOrDefault(x => x.Name == "MethodBase");
        GetMethodFromHandle = module.Import(methodBaseDefinition.Methods.First(x => x.Name == "GetMethodFromHandle"));

        var methodInfo = msCoreTypes.FirstOrDefault(x => x.Name == "MethodInfo");
        MethodInfoTypeReference = module.Import(methodInfo);

        var boolDefinition = msCoreTypes.First(x => x.Name == "Boolean");
        BoolTypeReference = module.Import(boolDefinition);

        var voidDefinition = msCoreTypes.First(x => x.Name == "Void");
        VoidTypeReference = module.Import(voidDefinition);

        var stringDefinition = msCoreTypes.First(x => x.Name == "String");
        StringTypeReference = module.Import(stringDefinition);


        var systemDefinition = assemblyResolver.Resolve("System");
        var systemTypes = systemDefinition.MainModule.Types;

        var actionDefinition = msCoreTypes.FirstOrDefault(x => x.Name == "Action");
        if (actionDefinition == null)
        {
            actionDefinition = systemTypes.FirstOrDefault(x => x.Name == "Action");
        }
        var systemCoreDefinition = GetSystemCoreDefinition();
        if (actionDefinition == null)
        {
            actionDefinition = systemCoreDefinition.MainModule.Types.FirstOrDefault(x => x.Name == "Action");
        }
        ActionTypeReference = module.Import(actionDefinition);


        var expressionTypeDefiniton = systemCoreDefinition.MainModule.Types.First(x => x.Name == "Expression");
        var propertyMethodDefinition = expressionTypeDefiniton.Methods.First(x => x.Name == "Property" && x.Parameters.Last().ParameterType.Name == "MethodInfo");
        PropertyReference = module.Import(propertyMethodDefinition);

        var actionConstructor = actionDefinition.Methods.First(x => x.IsConstructor);
        ActionConstructorReference = module.Import(actionConstructor);


        var propChangedHandlerDefinition = systemTypes.First(x => x.Name == "PropertyChangedEventHandler");
        PropChangedHandlerReference = module.Import(propChangedHandlerDefinition);
        ComponentModelPropertyChangedEventHandlerInvokeReference = module.Import(propChangedHandlerDefinition.Methods.First(x => x.Name == "Invoke"));
        var propChangedArgsDefinition = systemTypes.First(x => x.Name == "PropertyChangedEventArgs");
        ComponentModelPropertyChangedEventConstructorReference = module.Import(propChangedArgsDefinition.Methods.First(x => x.IsConstructor));

    }

    public void ExecuteWinRT()
    {
        var systemRuntime = assemblyResolver.Resolve("System.Runtime");
        var systemRuntimeTypes = systemRuntime.MainModule.Types;

        var objectDefinition = systemRuntimeTypes.First(x => x.Name == "Object");
        var module = moduleReader.Module;
        ObjectTypeReference = module.Import(objectDefinition);
        var constructorDefinition = objectDefinition.Methods.First(x => x.IsConstructor);
        ObjectConstructor = module.Import(constructorDefinition);


        var nullableDefinition = systemRuntimeTypes.FirstOrDefault(x => x.Name == "Nullable");
        NullableEqualsMethod = module.Import(nullableDefinition).Resolve().Methods.First(x => x.Name == "Equals");


        var actionDefinition = systemRuntimeTypes.First(x => x.Name == "Action");
        ActionTypeReference = module.Import(actionDefinition);
        var actionConstructor = actionDefinition.Methods.First(x => x.IsConstructor);
        ActionConstructorReference = module.Import(actionConstructor);

        var boolDefinition = systemRuntimeTypes.First(x => x.Name == "Boolean");
        BoolTypeReference = module.Import(boolDefinition);

        var voidDefinition = systemRuntimeTypes.First(x => x.Name == "Void");
        VoidTypeReference = module.Import(voidDefinition);

        var stringDefinition = systemRuntimeTypes.First(x => x.Name == "String");
        StringTypeReference = module.Import(stringDefinition);


        var systemReflection = assemblyResolver.Resolve("System.Reflection");
        var methodBaseDefinition = systemReflection.MainModule.Types.First(x => x.Name == "MethodBase");
        GetMethodFromHandle = module.Import(methodBaseDefinition.Methods.First(x => x.Name == "GetMethodFromHandle"));

        var methodInfo = systemReflection.MainModule.Types.FirstOrDefault(x => x.Name == "MethodInfo");
        MethodInfoTypeReference = module.Import(methodInfo);


        var systemObjectModel = assemblyResolver.Resolve("System.ObjectModel");
        var systemObjectModelTypes = systemObjectModel.MainModule.Types;
        var propChangedHandlerDefinition = systemObjectModelTypes.First(x => x.Name == "PropertyChangedEventHandler");
        PropChangedHandlerReference = module.Import(propChangedHandlerDefinition);
        ComponentModelPropertyChangedEventHandlerInvokeReference = module.Import(propChangedHandlerDefinition.Methods.First(x => x.Name == "Invoke"));
        var propChangedArgsDefinition = systemObjectModelTypes.First(x => x.Name == "PropertyChangedEventArgs");
        ComponentModelPropertyChangedEventConstructorReference = module.Import(propChangedArgsDefinition.Methods.First(x => x.IsConstructor));

        var windowsRuntime = assemblyResolver.Resolve("System.Runtime.InteropServices.WindowsRuntime");
        var genericInstanceType = new GenericInstanceType(windowsRuntime.MainModule.Types.First(x => x.Name == "EventRegistrationTokenTable`1"));
        genericInstanceType.GenericArguments.Add(PropChangedHandlerReference);

        var systemLinqExpressions = assemblyResolver.Resolve("System.Linq.Expressions");
        var expressionTypeDefiniton = systemLinqExpressions.MainModule.Types.First(x => x.Name == "Expression");
        var propertyMethodDefinition = expressionTypeDefiniton.Methods.First(x => x.Name == "Property" && x.Parameters.Last().ParameterType.Name == "MethodInfo");
        PropertyReference = module.Import(propertyMethodDefinition);

    }


    AssemblyDefinition GetSystemCoreDefinition()
    {
        try
        {
            return assemblyResolver.Resolve("System.Core");
        }
        catch (Exception exception)
        {
            var message = string.Format(@"Could not resolve System.Core. Please ensure you are using .net 3.5 or higher.{0}Inner message:{1}.",Environment.NewLine, exception.Message);
            throw new WeavingException(message);
        }
    }
}