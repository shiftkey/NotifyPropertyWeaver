using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;

public class PropertyWeaver
{
    MsCoreReferenceFinder msCoreReferenceFinder;
    Logger logger;
    PropertyData propertyData;
    TypeNode typeNode;
    MethodBody setMethodBody;
    Collection<Instruction> instructions;

    public PropertyWeaver(MsCoreReferenceFinder msCoreReferenceFinder, Logger logger, PropertyData propertyData, TypeNode typeNode)
    {
        this.msCoreReferenceFinder = msCoreReferenceFinder;
        this.logger = logger;
        this.propertyData = propertyData;
        this.typeNode = typeNode;
    }

    public void Execute()
    {
        logger.LogMessage("\t\t" + propertyData.PropertyDefinition.Name);
        var property = propertyData.PropertyDefinition;
        setMethodBody = property.SetMethod.Body;
        instructions = property.SetMethod.Body.Instructions;

        logger.LogMessage("\t\t\tCheckForEquality=" + propertyData.CheckForEquality);

        var indexes = GetIndexes();
        indexes.Reverse();
        foreach (var index in indexes)
        {
            InjectAtIndex(index);
        }
    }

    List<int> GetIndexes()
    {
        if (propertyData.BackingFieldReference == null)
        {
            return FindReturnInstructions().ToList();
        }
        var setFieldInstructions = FindSetFieldInstructions().ToList();
        if (setFieldInstructions.Count == 0)
        {
            return FindReturnInstructions().ToList();
        }
        return setFieldInstructions;
    }

    void InjectAtIndex(int index)
    {
        index = AddIsChangedSetterCall(index);
        var propertyDefinitions = propertyData.AlsoNotifyFor.Distinct();

        foreach (var propertyDefinition in propertyDefinitions)
        {
            index = AddEventInvokeCall(index, propertyDefinition);
        }
        AddEventInvokeCall(index, propertyData.PropertyDefinition);
    }

    IEnumerable<int> FindSetFieldInstructions()
    {
        for (var index = 0; index < instructions.Count; index++)
        {
            var instruction = instructions[index];
            if (instruction.OpCode != OpCodes.Stfld)
            {
                continue;
            }
            var fieldReference = instruction.Operand as FieldReference;
            if (fieldReference == null)
            {
                continue;
            }

            if (fieldReference.Name == propertyData.BackingFieldReference.Name)
            {
                yield return index + 1;
            }
        }
    }

    IEnumerable<int> FindReturnInstructions()
    {
        for (var index = 0; index < instructions.Count; index++)
        {
            var instruction = instructions[index];
            if (instruction.OpCode == OpCodes.Ret)
            {
                yield return index;
            }
        }
    }

    int AddIsChangedSetterCall(int index)
    {
        if (typeNode.IsChangedInvoker != null &&
            propertyData.PropertyDefinition.Name != "IsChanged" &&
            propertyData.SetIsChanged)
        {
            logger.LogMessage("\t\t\tSet IsChanged");
            return instructions.Insert(index,
                                       Instruction.Create(OpCodes.Ldarg_0),
                                       Instruction.Create(OpCodes.Ldc_I4, 1),
                                       CreateIsChangedInvoker());
        }
        return index;
    }

    int AddEventInvokeCall(int index, PropertyDefinition property)
    {
        logger.LogMessage(string.Format("\t\t\t{0}", property.Name));
        index = AddOnChangedMethodCall(index, property);
        if (typeNode.EventInvoker.IsBeforeAfter)
        {
            return AddBeforeAfterInvokerCall(index, property);
        }
        return AddSimpleInvokerCall(index, property);
    }

    int AddOnChangedMethodCall(int index, PropertyDefinition property)
    {
        var onChangedMethod = typeNode.OnChangedMethods.FirstOrDefault(x => x.Name == string.Format("On{0}Changed", property.Name));
        if (onChangedMethod != null)
        {
            return instructions.Insert(index,
                                       Instruction.Create(OpCodes.Ldarg_0),
                                       CreateCall(onChangedMethod)
                );
        }
        return index;
    }

    int AddSimpleInvokerCall(int index, PropertyDefinition property)
    {
        return instructions.Insert(index,
                                   Instruction.Create(OpCodes.Ldarg_0),
                                   Instruction.Create(OpCodes.Ldstr, property.Name),
                                   CallEventInvoker());
    }

    int AddBeforeAfterInvokerCall(int index, PropertyDefinition property)
    {
        var beforeVariable = new VariableDefinition(msCoreReferenceFinder.ObjectTypeReference);
        setMethodBody.Variables.Add(beforeVariable);
        var afterVariable = new VariableDefinition(msCoreReferenceFinder.ObjectTypeReference);
        setMethodBody.Variables.Add(afterVariable);
        var isVirtual = property.GetMethod.IsVirtual;
        var getMethod = property.GetMethod.GetGeneric();

        index = instructions.Insert(index,
                                    Instruction.Create(OpCodes.Ldarg_0),
                                    CreateCall(getMethod),
                                    //TODO: look into why this box is required
                                    Instruction.Create(OpCodes.Box, property.GetMethod.ReturnType),
                                    Instruction.Create(OpCodes.Stloc, afterVariable),
                                    Instruction.Create(OpCodes.Ldarg_0),
                                    Instruction.Create(OpCodes.Ldstr, property.Name),
                                    Instruction.Create(OpCodes.Ldloc, beforeVariable),
                                    Instruction.Create(OpCodes.Ldloc, afterVariable),
                                    CallEventInvoker()
            );

        instructions.Prepend(
            Instruction.Create(OpCodes.Ldarg_0),
            CreateCall(getMethod),
            //TODO: look into why this box is required
            Instruction.Create(OpCodes.Box, property.GetMethod.ReturnType),
            Instruction.Create(OpCodes.Stloc, beforeVariable));
        return index + 4;
    }

    public Instruction CallEventInvoker()
    {
            return Instruction.Create(OpCodes.Callvirt, typeNode.EventInvoker.MethodReference);
    }

    public Instruction CreateIsChangedInvoker()
    {
        return Instruction.Create(OpCodes.Callvirt, typeNode.IsChangedInvoker);
    }

    public Instruction CreateCall(MethodReference methodReference)
    {
            return Instruction.Create(OpCodes.Callvirt, methodReference);
    }

}