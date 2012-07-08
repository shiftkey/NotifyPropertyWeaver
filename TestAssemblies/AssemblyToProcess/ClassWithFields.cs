using System.ComponentModel;
using NotifyPropertyWeaver;

public class ClassWithFields : INotifyPropertyChanged
{
    [NotifyProperty] 
    public string Property1;
    [DependsOn("Property1")] 
    public string Property2;

    public event PropertyChangedEventHandler PropertyChanged;
}