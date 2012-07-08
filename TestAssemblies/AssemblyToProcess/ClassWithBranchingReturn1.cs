using System;
using System.ComponentModel;
using NotifyPropertyWeaver;

public class ClassWithBranchingReturn1 : INotifyPropertyChanged
{
    string property1;
    bool isInSomeMode;

    [NotifyProperty]
    public string Property1
    {
        get { return property1; }
        set
        {
            property1 = value;
            if (isInSomeMode)
            {
                Console.WriteLine("code here so 'if' does not get optimized away in release mode");
                return;
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
}