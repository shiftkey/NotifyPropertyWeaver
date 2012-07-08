using NotifyPropertyWeaver;

public interface InterfaceWithAttributes
{
    [NotifyProperty(PerformEqualityCheck = false)]
    [DependsOn("a")]
    [DoNotNotify]
    string Property1 { get; set; }
}