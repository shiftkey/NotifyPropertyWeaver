using System.ComponentModel;
using NotifyPropertyWeaver;

public class ClassEqualityWithDouble : INotifyPropertyChanged
{
    [NotifyProperty(PerformEqualityCheck = true)]
    public double Property1 { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;
}
