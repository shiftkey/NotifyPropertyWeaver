using System.ComponentModel;
using NotifyPropertyWeaver;

[NotifyForAll]
public class ClassWithExplicitPropertyChanged : INotifyPropertyChanged
{
    PropertyChangedEventHandler propertyChanged;
    event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
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