using Caliburn.PresentationFramework;
using NotifyPropertyWeaver;


public class ClassCaliburnOverriddenInvoker : PropertyChangedBase
{
    [NotifyProperty]
    public string Property1 { get; set; }
    public bool OverrideCalled { get; set; }

    public override void NotifyOfPropertyChange(string propertyName)
    {
        OverrideCalled = true;
        base.NotifyOfPropertyChange(propertyName);
    }

}