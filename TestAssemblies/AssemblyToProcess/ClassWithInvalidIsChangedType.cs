using System.ComponentModel;
using NotifyPropertyWeaver;

public class ClassWithInvalidIsChangedType : INotifyPropertyChanged
{
    [NotifyProperty]
    public string Property1 { get; set; }
    public string IsChanged { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;
}