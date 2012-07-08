using System.ComponentModel.Composition;
using Mono.Cecil;
using Mono.Cecil.Cil;

[Export, PartCreationPolicy(CreationPolicy.Shared)]
public class DelegateHolderInjector
{
    MsCoreReferenceFinder msCoreReferenceFinder;

    [ImportingConstructor]
    public DelegateHolderInjector(MsCoreReferenceFinder msCoreReferenceFinder)
    {
        this.msCoreReferenceFinder = msCoreReferenceFinder;
    }

    public void Execute(TypeDefinition targetTypeDefinition, MethodReference onPropertyChangedMethodReference)
    {
        TypeDefinition = new TypeDefinition(null, "<>PropertyNotificationDelegateHolder", TypeAttributes.AutoClass | TypeAttributes.AnsiClass | TypeAttributes.Sealed | TypeAttributes.NestedPrivate | TypeAttributes.BeforeFieldInit, msCoreReferenceFinder.ObjectTypeReference);
        CreateFields(targetTypeDefinition);
        CreateOnPropChanged(onPropertyChangedMethodReference);
        CreateConstructor();
        targetTypeDefinition.NestedTypes.Add(TypeDefinition);
    }

    void CreateFields(TypeDefinition targetTypeDefinition)
    {
        Target = new FieldDefinition("target", FieldAttributes.Public, targetTypeDefinition);
        TypeDefinition.Fields.Add(Target);
        PropertyName = new FieldDefinition("propertyName", FieldAttributes.Public, msCoreReferenceFinder.StringTypeReference);
        TypeDefinition.Fields.Add(PropertyName);
    }

    void CreateOnPropChanged(MethodReference onPropertyChangedMethodReference)
    {
        MethodDefinition = new MethodDefinition("OnPropertyChanged", MethodAttributes.Public | MethodAttributes.HideBySig, msCoreReferenceFinder.VoidTypeReference);
        MethodDefinition.Body.Instructions.Append(
            Instruction.Create(OpCodes.Ldarg_0),
            Instruction.Create(OpCodes.Ldfld, Target),
            Instruction.Create(OpCodes.Ldarg_0),
            Instruction.Create(OpCodes.Ldfld, PropertyName),
            Instruction.Create(OpCodes.Callvirt, onPropertyChangedMethodReference),
            Instruction.Create(OpCodes.Ret)
            );
        TypeDefinition.Methods.Add(MethodDefinition);
    }

    void CreateConstructor()
    {
        ConstructorDefinition = new MethodDefinition(".ctor", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, msCoreReferenceFinder.VoidTypeReference);
        ConstructorDefinition.Body.Instructions.Append(
            Instruction.Create(OpCodes.Ldarg_0),
            Instruction.Create(OpCodes.Call, msCoreReferenceFinder.ObjectConstructor),
            Instruction.Create(OpCodes.Ret)
            );
        TypeDefinition.Methods.Add(ConstructorDefinition);
    }

    public MethodDefinition MethodDefinition { get; set; }
    public FieldDefinition PropertyName { get; set; }
    public FieldDefinition Target { get; set; }
    public TypeDefinition TypeDefinition { get; set; }
    public MethodDefinition ConstructorDefinition { get; set; }
}