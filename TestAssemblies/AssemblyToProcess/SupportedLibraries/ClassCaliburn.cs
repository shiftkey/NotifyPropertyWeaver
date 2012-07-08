using Caliburn.PresentationFramework;
using NotifyPropertyWeaver;

public class 
    ClassCaliburn : PropertyChangedBase
{
    [NotifyProperty]
    public string Property1 { get; set; }
}