using System.ComponentModel;
using NotifyPropertyWeaver;

public class ClassDependsOn : INotifyPropertyChanged
{
    [NotifyProperty]
    public string Property1 { get; set; }
    [DependsOn("Property1")]
    public string Property2 { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;
}