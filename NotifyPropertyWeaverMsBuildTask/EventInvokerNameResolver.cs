using System.Collections.Generic;
using System.ComponentModel.Composition;
using NotifyPropertyWeaverMsBuildTask;

[Export, PartCreationPolicy(CreationPolicy.Shared)]
public class EventInvokerNameResolver
{
    WeavingTask moduleWeaver;
    public List<string> EventInvokerNames { get; set; }

 
    [ImportingConstructor]
    public EventInvokerNameResolver(WeavingTask moduleWeaver)
    {
        this.moduleWeaver = moduleWeaver;
        EventInvokerNames = new List<string> { "OnPropertyChanged", "NotifyOfPropertyChange", "RaisePropertyChanged", "NotifyPropertyChanged", "NotifyChanged" };
    }

    public void Execute()
    {

        if (!string.IsNullOrWhiteSpace(moduleWeaver.EventInvokerName))
        {
            EventInvokerNames.Remove(moduleWeaver.EventInvokerName);
            EventInvokerNames.Insert(0,moduleWeaver.EventInvokerName);
        }
    }
}