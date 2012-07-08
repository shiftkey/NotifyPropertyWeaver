using System.ComponentModel;
using NotifyPropertyWeaver;

[NotifyForAll]
public class ClassWithDependencyAfterSet : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    string property2;
    public string Property2
    {
        get { return property2; }
        set { property2 = value; }
    }

    string property1;
    public string Property1
    {
        get { return property1; }
        set
        {
            property1 = value;
            Property2 = value;
        }
    }
}