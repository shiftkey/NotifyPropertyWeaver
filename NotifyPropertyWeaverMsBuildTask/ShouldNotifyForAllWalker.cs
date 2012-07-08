using System.Collections.Generic;
using System.ComponentModel.Composition;

[Export, PartCreationPolicy(CreationPolicy.Shared)]
public class ShouldNotifyForAllWalker
{
    TypeNodeBuilder typeNodeBuilder;

    [ImportingConstructor]
    public ShouldNotifyForAllWalker(TypeNodeBuilder typeNodeBuilder)
    {
        this.typeNodeBuilder = typeNodeBuilder;
    }

    void Process(List<TypeNode> notifyNodes)
    {
        foreach (var notifyNode in notifyNodes)
        {
            var shouldNotifyForAllInType = false;
            if (notifyNode.TypeDefinition.CustomAttributes.ContainsAttribute("NotifyForAllAttribute"))
            {
                shouldNotifyForAllInType = true;
            }
            notifyNode.ShouldNotifyForAllInType = shouldNotifyForAllInType;
            Process(notifyNode.Nodes);
        }
    }

    public void Execute()
    {
        Process(typeNodeBuilder.NotifyNodes);
    }
}