using System.ComponentModel;
using NotifyPropertyWeaver;

public sealed class ClassThatIsSealed : INotifyPropertyChanged
{
    [NotifyProperty]
    public string Property1 { get; set; }
    public event PropertyChangedEventHandler PropertyChanged;
}