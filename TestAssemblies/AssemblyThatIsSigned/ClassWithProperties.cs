using System.ComponentModel;
using NotifyPropertyWeaver;


public class ClassWithProperties : INotifyPropertyChanged
{
    [NotifyProperty]
    public string Property1 { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;

}
