using NotifyPropertyWeaver;

public class ClassWithPropertyImp : ClassWithAbstractProperty
{
    [NotifyProperty]
    public override string Property1 { get; set; }
}