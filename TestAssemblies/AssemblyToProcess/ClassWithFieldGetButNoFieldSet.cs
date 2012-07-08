using System.ComponentModel;
using NotifyPropertyWeaver;

public class ClassWithFieldGetButNoFieldSet : INotifyPropertyChanged
{
    string property;

    [NotifyProperty]
    public string Property1
    {
        get { return property; }
        set { SetField(value); }
    }

    void SetField(string value)
    {
        property = value; 
    }

    public event PropertyChangedEventHandler PropertyChanged;
}