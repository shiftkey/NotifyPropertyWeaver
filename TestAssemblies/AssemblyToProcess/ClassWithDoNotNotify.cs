using System.ComponentModel;
using NotifyPropertyWeaver;

[DoNotNotify]
public class ClassWithDoNotNotify : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
}