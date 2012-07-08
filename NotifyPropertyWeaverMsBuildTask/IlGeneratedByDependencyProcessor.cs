using System.Collections.Generic;
using System.ComponentModel.Composition;

[Export, PartCreationPolicy(CreationPolicy.Shared)]
public class IlGeneratedByDependencyProcessor
{
    TypeNodeBuilder typeNodeBuilder;

    [ImportingConstructor]
    public IlGeneratedByDependencyProcessor(TypeNodeBuilder typeNodeBuilder)
    {
        this.typeNodeBuilder = typeNodeBuilder;
    }

    public void Execute()
    {
        Process(typeNodeBuilder.NotifyNodes);
    }


    void Process(List<TypeNode> notifyNodes)
    {
        foreach (var node in notifyNodes)
        {
            var ilGeneratedByDependencyReader = new IlGeneratedByDependencyReader(node);
            ilGeneratedByDependencyReader.Process(); 
            Process(node.Nodes);
        }
    }


}