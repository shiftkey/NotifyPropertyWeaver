using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Mono.Cecil;

[Export]
public class NotifyPropertyDataAttributeReader
{
    Logger logger;

    [ImportingConstructor]
    public NotifyPropertyDataAttributeReader(Logger logger)
    {
        this.logger = logger;
    }

    public NotifyPropertyData Read(PropertyDefinition property, List<PropertyDefinition> allProperties)
    {
        var notifyAttribute = property.CustomAttributes.GetAttribute("NotifyPropertyAttribute");
        if (notifyAttribute == null)
        {
            return null;
        }
        var propertyNamesToNotify = GetPropertyNamesToNotify(notifyAttribute, property, allProperties);

        return new NotifyPropertyData
                   {
                       CheckForEquality = GetCheckForEquality(notifyAttribute),
                       AlsoNotifyFor = propertyNamesToNotify.ToList(),
                       SetIsChanged = GetSetIsChanged(notifyAttribute),
                   };
    }

    IEnumerable<PropertyDefinition> GetPropertyNamesToNotify(CustomAttribute notifyAttribute, PropertyDefinition property, List<PropertyDefinition> allProperties)
    {
        var alsoNotifyProperty = notifyAttribute.Properties.FirstOrDefault(x => x.Name == "AlsoNotifyFor");
        var alsoValue = (CustomAttributeArgument[]) alsoNotifyProperty.Argument.Value;
        if (alsoValue != null)
        {
            foreach (string argument in alsoValue.Select(x => x.Value))
            {
                var propertyDefinition = allProperties.FirstOrDefault(x => x.Name == argument);
                if (propertyDefinition == null)
                {
                    logger.LogError(string.Format("Could not find property '{0}' for AlsoNotifyFor attribute assinged to '{1}'.", argument, property.Name));
                    continue;
                }
                yield return propertyDefinition;
            }
        }
    }

    bool? GetSetIsChanged(CustomAttribute notifyAttribute)
    {
        var setIsChanged = notifyAttribute.Properties.FirstOrDefault(x => x.Name == "SetIsChanged");
        var setIsChangedValue = setIsChanged.Argument.Value;
        if (setIsChangedValue != null)
        {
            return (bool) setIsChangedValue;
        }
        return null;
    }


    bool? GetCheckForEquality(CustomAttribute notifyAttribute)
    {
        var performEqualityCheck = notifyAttribute.Properties.FirstOrDefault(x => x.Name == "PerformEqualityCheck");
        var equalityCheckValue = performEqualityCheck.Argument.Value;
        if (equalityCheckValue != null)
        {
            return (bool) equalityCheckValue;
        }
        return null;
    }

}