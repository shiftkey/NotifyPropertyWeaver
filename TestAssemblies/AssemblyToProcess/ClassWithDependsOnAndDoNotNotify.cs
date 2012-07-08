using System.ComponentModel;
using NotifyPropertyWeaver;

public class ClassWithDependsOnAndDoNotNotify : INotifyPropertyChanged
{
    [DoNotNotify]
    public string UseLessProperty { get; set; }

    [NotifyProperty]
    public string Property1 { get; set; }
    [NotifyProperty]   
    public string Property2 { get { return Property1; } }


    public event PropertyChangedEventHandler PropertyChanged;
}