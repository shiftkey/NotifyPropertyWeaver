using NotifyPropertyWeaver;

public class ClassWithNotifyInBase : ClassParentWithProperty
{
    [NotifyProperty(AlsoNotifyFor = new[] { "Property2" })]
    public string Property1 { get; set; }
}