using System.ComponentModel;
using NotifyPropertyWeaver;

public class ClassWithCustomEventInvoker : INotifyPropertyChanged
{
    
    [NotifyProperty]
    public string Property1 { get; set; }
    public void CustomEventInvoker(string propertyName)
    {
        if (PropertyChanged!= null)
        {
            PropertyChanged(this,new PropertyChangedEventArgs(propertyName));
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
}