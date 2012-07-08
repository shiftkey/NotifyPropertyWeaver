using System.ComponentModel;
using NotifyPropertyWeaver;

[NotifyForAll]
public class Class1 : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
    public Class1()
    {
        var type = typeof(NotifyForAllAttribute);
    }
}