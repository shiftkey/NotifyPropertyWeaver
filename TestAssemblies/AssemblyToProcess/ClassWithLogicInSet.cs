using System.ComponentModel;
using System.Diagnostics;
using NotifyPropertyWeaver;

public class ClassWithLogicInSet : INotifyPropertyChanged
{

    string property1;

    [NotifyProperty]
    public string Property1
    {
        get { return property1; }
        set
        {
            Debug.WriteLine("Foo");
            property1 = value;
            Debug.WriteLine("Bar");
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
}
