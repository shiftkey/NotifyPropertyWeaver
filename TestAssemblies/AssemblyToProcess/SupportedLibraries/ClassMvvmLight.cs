using GalaSoft.MvvmLight;
using NotifyPropertyWeaver;

public class ClassMvvmLightFromViewModel : ViewModelBase
{
    [NotifyProperty]
    public string Property1 { get; set; }
}
public class ClassMvvmLightFromObservableObject : ObservableObject
{
    [NotifyProperty]
    public string Property1 { get; set; }
}