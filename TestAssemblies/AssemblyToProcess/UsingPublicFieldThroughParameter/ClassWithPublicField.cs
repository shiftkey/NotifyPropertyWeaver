using System.ComponentModel;
using NotifyPropertyWeaver;

public class ClassWithPublicField : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    [NotifyProperty]
    public bool Property1;
}