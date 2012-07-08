using System.ComponentModel;
using NotifyPropertyWeaver;

public class ClassMissingSetGet : INotifyPropertyChanged
{
    string property;

    public string PropertyNoSet
    {
        get { return property; }
    }


    [NotifyProperty]
    public string PropertyNoGet
    {
        set { property = value; }
    }

    public event PropertyChangedEventHandler PropertyChanged;

}