using System.ComponentModel;
using NotifyPropertyWeaver;

public class ClassWithOnChanged : INotifyPropertyChanged
{
    public bool OnProperty1ChangedCalled;

    [NotifyProperty]
    public string Property1 { get; set; }
    public void OnProperty1Changed ()
    {
        OnProperty1ChangedCalled = true;
    }

    public event PropertyChangedEventHandler PropertyChanged;
}