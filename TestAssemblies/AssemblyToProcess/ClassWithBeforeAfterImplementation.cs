using System;
using System.ComponentModel;
using NotifyPropertyWeaver;

public class ClassWithBeforeAfterImplementation : INotifyPropertyChanged
{

    [NotifyProperty]
    public string Property1 { get; set; }
    [DependsOn("Property1")]
    public string Property2 { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;

    public void OnPropertyChanged(string propertyName, object before, object after)
    {
        ValidateIsString(after);
        ValidateIsString(before);

        var handler = PropertyChanged;
        if (handler != null)
        {
            handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    void ValidateIsString(object value)
    {
        if (value != null)
        {
            var name = value.GetType().Name;
            if (name != "String")
            {
                throw new Exception(string.Format("Value shoud be string but is '{0}'.", name));
            }
        }
    }
}