using System.Collections.ObjectModel;
using System.ComponentModel;

public class ExperimentClass : ReadOnlyObservableCollection<object>
{
    public ExperimentClass()
        : base(new ObservableCollection<object>())
    {
    }
    public void OnPropertyChanged(string propertyName)
    {
        base.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
    }
    public string Property1 { get; set; }
}
