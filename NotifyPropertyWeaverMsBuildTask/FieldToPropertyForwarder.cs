using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Mono.Cecil;
using Mono.Cecil.Rocks;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;

[Export, PartCreationPolicy(CreationPolicy.Shared)]
public class FieldToPropertyForwarder
{
    ModuleReader moduleReader;
    MsCoreReferenceFinder msCoreReferenceFinder;
    Dictionary<FieldDefinition, PropertyDefinition> forwardedFields;

    [ImportingConstructor]
    public FieldToPropertyForwarder(ModuleReader moduleReader, FieldToPropertyConverter fieldToPropertyConverter, MsCoreReferenceFinder msCoreReferenceFinder)
    {
        this.moduleReader = moduleReader;
        this.msCoreReferenceFinder = msCoreReferenceFinder;
        forwardedFields = fieldToPropertyConverter.ForwardedFields;
    }

    public void Execute()
    {
        foreach (var type in moduleReader.Module.GetAllTypeDefinitions())
        {
            if (type.IsInterface)
            {
                continue;
            }
            if (type.IsEnum)
            {
                continue;
            }
            foreach (var method in type.Methods)
            {
                if (method.IsGetter || method.IsSetter)
                {
                    continue;
                }
                Replace(method);
            }
            foreach (var property in type.Properties)
            {
                if (!forwardedFields.ContainsValue(property))
                {
                    Replace(property.GetMethod);
                    Replace(property.SetMethod);
                }
            }
        }
    }

    void Replace(MethodDefinition methodDefinition)
    {
        if (methodDefinition == null)
        {
            return;
        }
        if (methodDefinition.IsAbstract)
        {
            return;
        }
        //for delegates
        if (methodDefinition.Body == null)
        {
            return;
        } 
        methodDefinition.Body.SimplifyMacros();
        var actions = new List<Action<Collection<Instruction>>>();
        var instructions = methodDefinition.Body.Instructions;
        foreach (var instruction in instructions)
        {
            var fieldDefinition = instruction.Operand as FieldDefinition;
            if (fieldDefinition != null)
            {
                if (instruction.OpCode == OpCodes.Ldfld)
                {
                    PropertyDefinition propertyDefinition;
                    if (forwardedFields.TryGetValue(fieldDefinition, out propertyDefinition))
                    {
                        instruction.OpCode = OpCodes.Callvirt;
                        instruction.Operand = propertyDefinition.GetMethod;
                    }
                }
                if (instruction.OpCode == OpCodes.Ldflda)
                {
                    PropertyDefinition propertyDefinition;
                    if (forwardedFields.TryGetValue(fieldDefinition, out propertyDefinition))
                    {
                        methodDefinition.Body.InitLocals = true;
                        var variableDefinition = new VariableDefinition(propertyDefinition.PropertyType);
                        methodDefinition.Body.Variables.Add(variableDefinition);

                        instruction.OpCode = OpCodes.Callvirt;
                        instruction.Operand = propertyDefinition.GetMethod;
                        var localCopy = instruction;
                        actions.Add(collection =>
                                        {
                                            var indexOf = collection.IndexOf(localCopy) + 1; 
                                            collection.Insert(indexOf, Instruction.Create(OpCodes.Stloc, variableDefinition));
                                            collection.Insert(indexOf + 1, Instruction.Create(OpCodes.Ldloca, variableDefinition));
                                        });
                    }
                }

                if (instruction.OpCode == OpCodes.Ldtoken)
                {
                    actions.Add(ProcessLdToken(instruction, fieldDefinition));
                }

                if (instruction.OpCode == OpCodes.Stfld)
                {
                    PropertyDefinition propertyDefinition;
                    if (forwardedFields.TryGetValue(fieldDefinition, out propertyDefinition))
                    {
                        instruction.OpCode = OpCodes.Callvirt;
                        instruction.Operand = propertyDefinition.SetMethod;
                    }
                }
            }
        }
        foreach (var action in actions)
        {
            action(instructions);
        } 
        methodDefinition.Body.OptimizeMacros();
    }

    Action<Collection<Instruction>> ProcessLdToken(Instruction instruction, FieldDefinition fieldDefinition)
    {
        PropertyDefinition propertyDefinition;
        if (!forwardedFields.TryGetValue(fieldDefinition, out propertyDefinition))
        {
            return collection => { };
        }

        instruction.Operand = propertyDefinition.GetMethod;
        var next = instruction.Next;
        if (next == null)
        {
            return collection => { };
        }
        var nextNext = next.Next;
        if (nextNext == null)
        {
            return collection => { };
        }
        if (next.OpCode != OpCodes.Call || nextNext.OpCode != OpCodes.Call)
        {
            return collection => { };
        }
        var nextMethod = next.Operand as MethodReference;
        if (nextMethod == null)
        {
            return collection => { };
        }
        var nextNextMethod = nextNext.Operand as MethodReference;
        if (nextNextMethod == null)
        {
            return collection => { };
        }
        if (nextMethod.FullName != "System.Reflection.FieldInfo System.Reflection.FieldInfo::GetFieldFromHandle(System.RuntimeFieldHandle)")
        {
            return collection => { };
        }
        if (nextNextMethod.FullName != "System.Linq.Expressions.MemberExpression System.Linq.Expressions.Expression::Field(System.Linq.Expressions.Expression,System.Reflection.FieldInfo)")
        {
            return collection => { };
        }
        next.Operand = msCoreReferenceFinder.GetMethodFromHandle;
        Action<Collection<Instruction>> processLdToken = collection =>
                                                             {
                                                                 var indexOf = collection.IndexOf(nextNext);
                                                                 collection.Insert(indexOf, Instruction.Create(OpCodes.Castclass, msCoreReferenceFinder.MethodInfoTypeReference));
                                                             };
        //nextNext.Next
        nextNext.Operand = msCoreReferenceFinder.PropertyReference;
        return processLdToken;
    }
}