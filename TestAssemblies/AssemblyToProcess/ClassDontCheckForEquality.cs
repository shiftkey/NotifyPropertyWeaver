using System.ComponentModel;
using NotifyPropertyWeaver;

public class ClassDontCheckForEquality : INotifyPropertyChanged
{
    [NotifyProperty(PerformEqualityCheck = false)]
    public string Property1 { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;
}