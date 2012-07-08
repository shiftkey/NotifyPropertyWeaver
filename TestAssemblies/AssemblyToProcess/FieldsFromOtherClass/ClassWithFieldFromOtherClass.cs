using System.ComponentModel;
using NotifyPropertyWeaver;

public class ClassWithFieldFromOtherClass : INotifyPropertyChanged
{
    OtherClass otherClass;

    public ClassWithFieldFromOtherClass()
    {
        otherClass = new OtherClass();
    }

    [NotifyProperty]
    public string Property1
    {
        get { return otherClass.property1; }
        set { otherClass.property1 = value; }
    }

    public event PropertyChangedEventHandler PropertyChanged;
}