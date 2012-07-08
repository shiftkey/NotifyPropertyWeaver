using System.ComponentModel;
using NotifyPropertyWeaver;

public class ClassWithBeforeAfterAndSimpleImplementation : INotifyPropertyChanged
{

    [NotifyProperty]
    public string Property1 { get; set; }
    [DependsOn("Property1")]
    public string Property2 { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;

    public void OnPropertyChanged(string propertyName)
    {

    }
    public void OnPropertyChanged(string propertyName, object before, object after)
    {
        var handler = PropertyChanged;
        if (handler != null)
        {
            handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}