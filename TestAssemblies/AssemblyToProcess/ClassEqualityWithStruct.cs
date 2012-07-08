using System.ComponentModel;
using NotifyPropertyWeaver;

public class ClassEqualityWithStruct : INotifyPropertyChanged
{
    [NotifyProperty(PerformEqualityCheck = true)]
    public SimpleStruct Property1 { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;
    
    public struct SimpleStruct
    {
    }

}