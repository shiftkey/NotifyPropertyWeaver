using System.ComponentModel;
using NotifyPropertyWeaver;

public class ClassAlsoNotifyFor : INotifyPropertyChanged
{
    [NotifyProperty(AlsoNotifyFor = new[] { "Property2" })]
    public string Property1 { get; set; }
    public string Property2 { get; set; }
    public event PropertyChangedEventHandler PropertyChanged;
}