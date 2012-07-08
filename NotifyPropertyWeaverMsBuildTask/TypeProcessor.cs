using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Rocks;

[Export, PartCreationPolicy(CreationPolicy.Shared)]
public class TypeProcessor
{
    TypeNodeBuilder typeNodeBuilder;
    Logger logger;
    MsCoreReferenceFinder msCoreReferenceFinder;
    TypeEqualityFinder typeEqualityFinder;

    [ImportingConstructor]
    public TypeProcessor(TypeNodeBuilder typeNodeBuilder, Logger logger, MsCoreReferenceFinder msCoreReferenceFinder, TypeEqualityFinder typeEqualityFinder)
    {
        this.typeNodeBuilder = typeNodeBuilder;
        this.logger = logger;
        this.msCoreReferenceFinder = msCoreReferenceFinder;
        this.typeEqualityFinder = typeEqualityFinder;
    }

    public void Execute()
    {
        Process(typeNodeBuilder.NotifyNodes);
    }

    void Process(List<TypeNode> notifyNodes)
    {
        foreach (var node in notifyNodes)
        {
            if (node.EventInvoker == null)
            {
                continue;
            }
            logger.LogMessage("\t" + node.TypeDefinition.FullName);

            foreach (var propertyData in node.PropertyDatas)
            {
                if (AlreadyContainsNotification(propertyData.PropertyDefinition, node.EventInvoker.MethodReference.Name))
                {
                    if (propertyData.NotificationAddedDirectly)
                    {
                        throw new WeavingException(string.Format("{0} Already has notification functionality. Please remove the attribute on this property.", propertyData.PropertyDefinition.GetName()));
                    }
                    logger.LogMessage(string.Format("\t{0} Already has notification functionality. Property will be ignored.", propertyData.PropertyDefinition.GetName()));
                    continue;
                }
                var methodBody = propertyData.PropertyDefinition.SetMethod.Body;
                methodBody.SimplifyMacros();

                methodBody.MakeLastStatementReturn();

                var propertyWeaver = new PropertyWeaver(msCoreReferenceFinder, logger, propertyData, node);
                propertyWeaver.Execute();

                var equalityCheckWeaver = new EqualityCheckWeaver( logger, propertyData, typeEqualityFinder);
                equalityCheckWeaver.Execute();

                methodBody.InitLocals = true;

                methodBody.OptimizeMacros();
            }
            Process(node.Nodes);
        }
    }

    public static bool AlreadyContainsNotification(PropertyDefinition propertyDefinition, string methodName)
    {
        var instructions = propertyDefinition.SetMethod.Body.Instructions;
        return instructions.Any(x =>
                                x.OpCode.IsCall() &&
                                x.Operand is MethodReference &&
                                ((MethodReference) x.Operand).Name == methodName);
    }

}