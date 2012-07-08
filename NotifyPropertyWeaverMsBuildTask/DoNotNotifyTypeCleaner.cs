using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

[Export, PartCreationPolicy(CreationPolicy.Shared)]
public class DoNotNotifyTypeCleaner
{
    TypeNodeBuilder typeNodeBuilder;

    [ImportingConstructor]
    public DoNotNotifyTypeCleaner(TypeNodeBuilder typeNodeBuilder)
    {
        this.typeNodeBuilder = typeNodeBuilder;
    }

    void Process(List<TypeNode> notifyNodes)
    {
        foreach (var node in notifyNodes.ToList())
        {
            var containsDoNotNotifyAttribute = node.TypeDefinition.CustomAttributes.ContainsAttribute("DoNotNotifyAttribute");
            if (containsDoNotNotifyAttribute)
            {
                notifyNodes.Remove(node);
                continue;
            }
            Process(node.Nodes);
        }
    }

    public void Execute()
    {
        Process(typeNodeBuilder.NotifyNodes);
    }
}