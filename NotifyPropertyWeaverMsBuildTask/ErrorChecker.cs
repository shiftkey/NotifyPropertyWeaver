using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

[Export, PartCreationPolicy(CreationPolicy.Shared)]
public class ErrorChecker
{
    TypeNodeBuilder typeNodeBuilder;
    bool errorFound;
    Logger logger;

    [ImportingConstructor]
    public ErrorChecker(TypeNodeBuilder typeNodeBuilder, Logger logger)
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
                    var error = CheckForErrors(propertyData, node.EventInvoker.IsBeforeAfter);
                    if (error != null)
                    {
                        logger.LogError(string.Format("{0} {1}", propertyData.PropertyDefinition.GetName(), error));
                        errorFound = true;
                    }
                }
            }
            Process(node.Nodes);
        }
    }

    public string CheckForErrors(PropertyData propertyData, bool isBeforeAfter)
    {

        var propertyDefinition = propertyData.PropertyDefinition;
        if (propertyDefinition.SetMethod.Name == "set_Item" && propertyDefinition.SetMethod.Parameters.Count == 2 && propertyDefinition.SetMethod.Parameters[1].Name == "value")
        {
            return "Property is an indexer.";
        }
        if (propertyDefinition.SetMethod.IsAbstract)
        {
            return "Property is abstract.";
        }
        if (propertyData.CheckForEquality && (propertyData.BackingFieldReference == null) && (propertyDefinition.GetMethod == null))
        {
            return "When using CheckForEquality the property set must contain code that sets the backing field or have a property Get. Either the property contains no field set or it contains multiple sets and the names cannot be mapped to a property.";
        }
        if (isBeforeAfter && (propertyDefinition.GetMethod == null))
        {
            return "When using a before/after invoker the property have a 'get'.";
        }

        return null;
    }


    public void Execute()
    {
        var notifyNodes = typeNodeBuilder.NotifyNodes;
        Process(notifyNodes);
        if (errorFound)
        {
            throw new WeavingException("Some properties had errors. Weaving halted");
        }
    }
}