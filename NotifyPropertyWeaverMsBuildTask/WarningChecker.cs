using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

[Export, PartCreationPolicy(CreationPolicy.Shared)]
public class WarningChecker
{
    TypeNodeBuilder typeNodeBuilder;
    Logger logger;


    [ImportingConstructor]
    public WarningChecker(TypeNodeBuilder typeNodeBuilder, Logger logger)
    {
        this.typeNodeBuilder = typeNodeBuilder;
        this.logger = logger;
    }

    void Process(List<TypeNode> notifyNodes)
    {
        foreach (var node in notifyNodes)
        {
            foreach (var propertyData in node.PropertyDatas.ToList())
            {
                if (propertyData.NotificationAddedDirectly)
                {
                    continue;
                }
                var warning = CheckForWarning(propertyData, node.EventInvoker.IsBeforeAfter);
                if (warning != null)
                {
                    logger.LogMessage(string.Format("\t{0} {1} Property will be ignored.", propertyData.PropertyDefinition.GetName(), warning));
                    node.PropertyDatas.Remove(propertyData);
                }
            }
            Process(node.Nodes);
        }
    }

    public string CheckForWarning(PropertyData propertyData, bool isBeforeAfter)
    {
        var propertyDefinition = propertyData.PropertyDefinition;
        var setMethod = propertyDefinition.SetMethod;
        if (setMethod.Name == "set_Item" && setMethod.Parameters.Count == 2 && setMethod.Parameters[1].Name == "value")
        {
            return "Property is an indexer.";
        }
        if (setMethod.IsAbstract)
        {
            return "Property is abstract.";
        }
        if (propertyData.CheckForEquality && (propertyData.BackingFieldReference == null) && (propertyDefinition.GetMethod == null))
        {
            return "Property has no field set logic or it contains multiple sets and the names cannot be mapped to a property.";
        }
        if (isBeforeAfter && (propertyDefinition.GetMethod == null))
        {
            return "When using a before/after invoker the property have a 'get'.";
        }
        return null;
    }


    public void Execute()
    {
        Process(typeNodeBuilder.NotifyNodes);
    }
}