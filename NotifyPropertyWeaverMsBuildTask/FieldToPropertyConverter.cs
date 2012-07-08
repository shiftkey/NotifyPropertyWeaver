using System.Collections.Generic;
using System.ComponentModel.Composition;
using Mono.Cecil;
using Mono.Cecil.Cil;

[Export, PartCreationPolicy(CreationPolicy.Shared)]
public class FieldToPropertyConverter
{

    TypeNodeBuilder typeNodeBuilder;
    MsCoreReferenceFinder msCoreReferenceFinder;
    Logger logger;
    public Dictionary<FieldDefinition, PropertyDefinition> ForwardedFields;

    [ImportingConstructor]
    public FieldToPropertyConverter(TypeNodeBuilder typeNodeBuilder, MsCoreReferenceFinder msCoreReferenceFinder, Logger logger)
    {
        ForwardedFields = new Dictionary<FieldDefinition, PropertyDefinition>();
        this.typeNodeBuilder = typeNodeBuilder;
        this.msCoreReferenceFinder = msCoreReferenceFinder;
        this.logger = logger;
    }

    void Process(List<TypeNode> notifyNodes)
    {
        foreach (var node in notifyNodes)
        {
            foreach (var field in node.TypeDefinition.Fields)
            {
                ProcessField(node, field);
            }
            Process(node.Nodes);
        }
    }

    void ProcessField(TypeNode node, FieldDefinition field)
    {
        var name = field.Name;
        if (!field.IsPublic || field.IsStatic || !char.IsUpper(name, 0))
        {
            return;
        }
        if (node.TypeDefinition.HasGenericParameters)
        {
            var message = string.Format("Skipped converting public field '{0}.{1}' to a property because generic types are not currently supported. You should make this a public property instead.", node.TypeDefinition.Name, field.Name);
            logger.LogWarning(message);
            return;
        }
        field.Name = string.Format("<{0}>k__BackingField", name);
        field.IsPublic = false;
        field.IsPrivate = true;
        var get = GetGet(field, name);
        node.TypeDefinition.Methods.Add(get);

        var set = GetSet(field, name);
        node.TypeDefinition.Methods.Add(set);

        var propertyDefinition = new PropertyDefinition(name, PropertyAttributes.None, field.FieldType)
                                     {
                                         GetMethod = get,
                                         SetMethod = set
                                     };
        foreach (var customAttribute in field.CustomAttributes)
        {
            propertyDefinition.CustomAttributes.Add(customAttribute);
        }
        node.TypeDefinition.Properties.Add(propertyDefinition);
        ForwardedFields.Add(field, propertyDefinition);
    }

    MethodDefinition GetGet(FieldDefinition field, string name)
    {
        var get = new MethodDefinition("get_" + name, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, field.FieldType);
        var instructions = get.Body.Instructions;
        instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
        instructions.Add(Instruction.Create(OpCodes.Ldfld, field));
        instructions.Add(Instruction.Create(OpCodes.Stloc_0));
        var inst = Instruction.Create(OpCodes.Ldloc_0);
        instructions.Add(Instruction.Create(OpCodes.Br_S, inst));
        instructions.Add(inst);
        instructions.Add(Instruction.Create(OpCodes.Ret));
        get.Body.Variables.Add(new VariableDefinition(field.FieldType));
        get.Body.InitLocals = true;
        get.SemanticsAttributes = MethodSemanticsAttributes.Getter;
        return get;
    }

    MethodDefinition GetSet(FieldDefinition field, string name)
    {
        var set = new MethodDefinition("set_" + name, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, msCoreReferenceFinder.VoidTypeReference);
        var instructions = set.Body.Instructions;
        instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
        instructions.Add(Instruction.Create(OpCodes.Ldarg_1));
        instructions.Add(Instruction.Create(OpCodes.Stfld, field));
        instructions.Add(Instruction.Create(OpCodes.Ret));
        set.Parameters.Add(new ParameterDefinition(field.FieldType));
        set.SemanticsAttributes = MethodSemanticsAttributes.Setter;
        return set;
    }

    public void Execute()
    {
        Process(typeNodeBuilder.NotifyNodes);
    }
}