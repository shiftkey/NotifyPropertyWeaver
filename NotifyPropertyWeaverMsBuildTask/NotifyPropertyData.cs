using System.Collections.Generic;
using Mono.Cecil;

public class NotifyPropertyData
{
    public NotifyPropertyData()
    {
        AlsoNotifyFor = new List<PropertyDefinition>();
    }

    public List<PropertyDefinition> AlsoNotifyFor;
    public bool? CheckForEquality;
    public bool? SetIsChanged;
}