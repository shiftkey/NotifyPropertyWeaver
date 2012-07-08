using System.ComponentModel;
using NotifyPropertyWeaver;

public class ExperimentClass : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    [NotifyProperty(PerformEqualityCheck = true)]
    public string[] Property1 { get; set; }
}

