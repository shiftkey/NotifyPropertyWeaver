using System;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class NotifyPropertyAttribute : Attribute
{
    public bool PerformEqualityCheck { get; set; }
    public string[] AlsoNotifyFor { get; set; }
    public bool SetIsChanged { get; set; }
}