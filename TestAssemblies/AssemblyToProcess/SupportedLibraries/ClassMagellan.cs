using Magellan.Framework;
using NotifyPropertyWeaver;

public class ClassMagellan : PresentationObject
{
    [NotifyProperty]
    public string Property1 { get; set; }
}