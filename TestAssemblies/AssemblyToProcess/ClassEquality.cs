using System.ComponentModel;
using NotifyPropertyWeaver;

public class ClassEquality : INotifyPropertyChanged
{
    [NotifyProperty(PerformEqualityCheck = true)]
    public string StringProperty { get; set; }

    [NotifyProperty(PerformEqualityCheck = true)]
    public int IntProperty { get; set; }

    [NotifyProperty(PerformEqualityCheck = true)]
    public int? NullableIntProperty { get; set; }

    [NotifyProperty(PerformEqualityCheck = true)]
    public bool BoolProperty { get; set; }

    [NotifyProperty(PerformEqualityCheck = true)]
    public bool? NullableBoolProperty { get; set; }

    [NotifyProperty(PerformEqualityCheck = true)]
    public object ObjectProperty { get; set; }

    [NotifyProperty(PerformEqualityCheck = true)]
    public string[] ArrayProperty { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;
}