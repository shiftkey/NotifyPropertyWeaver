using System.ComponentModel;

namespace WpfDispatcher
{
public class Person : INotifyPropertyChanged
{
    public string Name;

    public event PropertyChangedEventHandler PropertyChanged;
}
}
