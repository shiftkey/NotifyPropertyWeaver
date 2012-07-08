using System.ComponentModel;
using NotifyPropertyWeaver;

public class ClassNoBackingWithEqualityField : INotifyPropertyChanged
{

    [NotifyProperty]
    public string StringProperty
    {
        get { return "Foo"; }
        set { }
    }

    public event PropertyChangedEventHandler PropertyChanged;
}
