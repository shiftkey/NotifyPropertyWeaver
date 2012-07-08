using System.ComponentModel;
using NotifyPropertyWeaver;

[NotifyForAll]
public class ClassWithIndexer : INotifyPropertyChanged
{
    public string Property1 { get; set; }
    string[] arr = new string[100];

    public string this[int i]
    {
        get
        {
            return arr[i];
        }
        set
        {
            arr[i] = value;
        }
    }
    public event PropertyChangedEventHandler PropertyChanged;
}