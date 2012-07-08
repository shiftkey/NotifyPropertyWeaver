using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

[Export, PartCreationPolicy(CreationPolicy.Shared)]
public class CodeGenTypeCleaner
{
    TypeNodeBuilder typeNodeBuilder;

    [ImportingConstructor]
    public CodeGenTypeCleaner(TypeNodeBuilder typeNodeBuilder)
    {
        this.typeNodeBuilder = typeNodeBuilder;
    }

    void Process(List<TypeNode> notifyNodes)
    {
        foreach (var node in notifyNodes.ToList())
        {
            var customAttributes = node.TypeDefinition.CustomAttributes;
            if (customAttributes.ContainsAttribute("CompilerGeneratedAttribute"))
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