using System.ComponentModel;
using NotifyPropertyWeaver;

public class ClassWithBoolPropUsingStringProp: INotifyPropertyChanged
{
    public bool StringCompareProperty
    {
        get
        {
            return StringProperty == "magicString";
        }
    }

    [NotifyProperty]
    public bool BoolProperty { get; set; }

    string stringProperty;
    [NotifyProperty]
    public string StringProperty
    {
        get { return stringProperty; }
        set
        {
            stringProperty = value;
            if (StringCompareProperty)
            {
                BoolProperty = true;
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
}