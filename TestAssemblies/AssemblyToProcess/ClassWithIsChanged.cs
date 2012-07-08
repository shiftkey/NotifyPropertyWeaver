using System.ComponentModel;
using NotifyPropertyWeaver;

public class ClassWithIsChanged : INotifyPropertyChanged
{
    [NotifyProperty]
    public string Property1 { get; set; }
    public bool IsChanged { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;
}