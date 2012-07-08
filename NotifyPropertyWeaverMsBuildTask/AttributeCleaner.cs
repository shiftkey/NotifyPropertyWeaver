using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Mono.Cecil;
using Mono.Collections.Generic;

[Export, PartCreationPolicy(CreationPolicy.Shared)]
public class AttributeCleaner
{
    ModuleReader moduleReader;
    Logger logger;
    List<string> propertyAttributeNames;
    List<string> typeAttributeNames;

    [ImportingConstructor]
    public AttributeCleaner(ModuleReader moduleReader, Logger logger)
    {
        propertyAttributeNames = new List<string> {"DoNotNotifyAttribute", "NotifyPropertyAttribute", "DependsOnAttribute"};
        typeAttributeNames = new List<string> { "NotifyForAllAttribute", "DoNotNotifyAttribute" };
        this.moduleReader = moduleReader;
        this.logger = logger;
    }

    void ProcessType(TypeDefinition type)
    {
        RemoveAttributes(type);
        foreach (var property in type.Properties)
        {
            RemoveAttributes(property.CustomAttributes, property.FullName);
        }
        foreach (var field in type.Fields)
        {
            RemoveAttributes(field.CustomAttributes, field.FullName);
        }
    }

    void RemoveAttributes(TypeDefinition typeDefinition)
    {
        var attributes = typeDefinition.CustomAttributes
            .Where(attribute => typeAttributeNames.Contains(attribute.Constructor.DeclaringType.Name));

        foreach (var customAttribute in attributes.ToList())
        {
            logger.LogMessage(string.Format("\tRemoving attribute '{0}' from '{1}'.", customAttribute.Constructor.DeclaringType.FullName, typeDefinition.FullName));
            typeDefinition.CustomAttributes.Remove(customAttribute);
        }
    }

    void RemoveAttributes(Collection<CustomAttribute> customAttributes, string fullName)
    {
        var attributes = customAttributes
            .Where(attribute => propertyAttributeNames.Contains(attribute.Constructor.DeclaringType.Name));

        foreach (var customAttribute in attributes.ToList())
        {
            logger.LogMessage(string.Format("\tRemoving attribute '{0}' from '{1}'.", customAttribute.Constructor.DeclaringType.FullName, fullName));
            customAttributes.Remove(customAttribute);
        }
    }

    public void Execute()
    {
        foreach (var type in moduleReader.Module.GetAllTypeDefinitions())
        {
            ProcessType(type);
        }
    }
}