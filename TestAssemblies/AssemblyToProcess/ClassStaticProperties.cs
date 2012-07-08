using System.ComponentModel;
using NotifyPropertyWeaver;

[NotifyForAll]
public class ClassStaticProperties : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
    public static string Property { get; set; }
}