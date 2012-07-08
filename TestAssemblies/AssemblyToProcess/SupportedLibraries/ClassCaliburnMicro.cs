using Caliburn.Micro;
using NotifyPropertyWeaver;

public class ClassCaliburnMicro : PropertyChangedBase
{
    [NotifyProperty]
    public string Property1 { get; set; }
}