using System.ComponentModel;
using NotifyPropertyWeaver;

public class ClassWithOnceRemovedINotify : INotifyPropertyChangedChild
{
    [NotifyProperty]
    public string Property1 { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;
}