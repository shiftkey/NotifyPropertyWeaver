using System.ComponentModel;
using NotifyPropertyWeaver;

[NotifyForAll]
public class ClassWithCustomPropertyChanged : INotifyPropertyChanged
{
    PropertyChangedEventHandler propertyChanged;

    public event PropertyChangedEventHandler PropertyChanged
    {
        add
        {
            propertyChanged += value;
        }
        remove
        {
            propertyChanged -= value;
        }
    }

    public string Property1 { get; set; }

}