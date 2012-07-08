using Microsoft.Practices.Prism.ViewModel;
using NotifyPropertyWeaver;

public class ClassPrism : NotificationObject
{
    [NotifyProperty]
    public string Property1 { get; set; }
}