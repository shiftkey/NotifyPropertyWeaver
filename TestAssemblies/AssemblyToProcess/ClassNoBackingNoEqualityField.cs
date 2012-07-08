using System.ComponentModel;
using NotifyPropertyWeaver;

public class ClassNoBackingNoEqualityField : INotifyPropertyChanged
{

    [NotifyProperty(PerformEqualityCheck = false)]
    public string StringProperty
    {
        get { return "Foo"; }
        set { }
    }

    public event PropertyChangedEventHandler PropertyChanged;
}