using System.ComponentModel;
using NotifyPropertyWeaver;

public class ClassWithBranchingReturnAndNoField : INotifyPropertyChanged
{
    public bool HasValue;
    int x;

    [NotifyProperty(PerformEqualityCheck = false)]
    public string Property1
    {
        get { return null; }
        set
        {
            if (HasValue)
            {
                x++;
            }
            else
            {
                x ++;
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
}