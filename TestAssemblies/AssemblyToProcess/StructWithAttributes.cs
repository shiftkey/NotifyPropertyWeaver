using NotifyPropertyWeaver;

public struct StructWithAttributes
{
    [NotifyProperty(PerformEqualityCheck = false)]
    [DependsOn("a")]
    [DoNotNotify]
    public string Property1 { get; set; }
}