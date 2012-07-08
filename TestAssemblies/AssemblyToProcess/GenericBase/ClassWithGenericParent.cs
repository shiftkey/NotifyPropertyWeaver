using System.ComponentModel;
using NotifyPropertyWeaver;

public class ClassWithGenericParent<T> : INotifyPropertyChanged
{
    [NotifyProperty]
    public string Property1 { get; set; }
    public event PropertyChangedEventHandler PropertyChanged;
}