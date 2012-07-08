using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Mono.Cecil;

[Export, PartCreationPolicy(CreationPolicy.Shared)]
public class TypeNodeBuilder
{
    ModuleReader moduleReader;
    NotifyInterfaceFinder notifyInterfaceFinder;
    TypeResolver typeResolver;
    List<TypeDefinition> allClasses;
    public List<TypeNode> Nodes;
    public List<TypeNode> NotifyNodes;
    ModuleDefinition moduleDefinition;

    [ImportingConstructor]
    public TypeNodeBuilder(ModuleReader moduleReader, NotifyInterfaceFinder notifyInterfaceFinder, TypeResolver typeResolver)
    {
        this.moduleReader = moduleReader;
        this.notifyInterfaceFinder = notifyInterfaceFinder;
        this.typeResolver = typeResolver;
    }

    public void Execute()
    {
        moduleDefinition = moduleReader.Module;
        Nodes = new List<TypeNode>();
        NotifyNodes = new List<TypeNode>();
        allClasses = new List<TypeDefinition>();
        WalkAllTypeDefinitions();
        foreach (var typeDefinition in allClasses.ToList())
        {
            AddClass(typeDefinition);
        }
        var typeNodes = Nodes;

        PopulateINotifyNodes(typeNodes);
    }

    void PopulateINotifyNodes(List<TypeNode> typeNodes)
    {
        foreach (var node in typeNodes)
        {
            if (notifyInterfaceFinder.HierachyImplementsINotify(node.TypeDefinition))
            {
                NotifyNodes.Add(node);
                continue;
            }
            PopulateINotifyNodes(node.Nodes);
        }
    }

    TypeNode AddClass(TypeDefinition typeDefinition)
    {
        allClasses.Remove(typeDefinition);
        var typeNode = new TypeNode
                           {
                               TypeDefinition = typeDefinition
                           };
        if (typeDefinition.BaseType.Scope.Name != moduleDefinition.Name)
        {
            Nodes.Add(typeNode);
        }
        else
        {
            var baseType = typeResolver.Resolve(typeDefinition.BaseType);
            var parentNode = FindClassNode(baseType, Nodes);
            if (parentNode == null)
            {
                parentNode = AddClass(baseType);
            }
            parentNode.Nodes.Add(typeNode);
        }
        return typeNode;

    }

    TypeNode FindClassNode(TypeDefinition type, IEnumerable<TypeNode> typeNode)
    {
        foreach (var node in typeNode)
        {
            if (type == node.TypeDefinition)
            {
                return node;
            }
            var findNode = FindClassNode(type, node.Nodes);
            if (findNode != null)
            {
                return findNode;
            }
        }
        return null;
    }

    public void WalkAllTypeDefinitions()
    {
        //First is always module so we will skip that;
        GetTypes(moduleDefinition.Types.Skip(1));
    }

    void GetTypes(IEnumerable<TypeDefinition> typeDefinitions)
    {
        foreach (var typeDefinition in typeDefinitions)
        {
            GetTypes(typeDefinition.NestedTypes);
            if (typeDefinition.IsClass)
            {
                allClasses.Add(typeDefinition);
            }
        }
    }
}