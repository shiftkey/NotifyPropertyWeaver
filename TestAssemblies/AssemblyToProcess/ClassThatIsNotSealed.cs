using System.ComponentModel;
using NotifyPropertyWeaver;

public class ClassThatIsNotSealed : INotifyPropertyChanged
{
    [NotifyProperty]
    public string Property1 { get; set; }
    public event PropertyChangedEventHandler PropertyChanged;
}