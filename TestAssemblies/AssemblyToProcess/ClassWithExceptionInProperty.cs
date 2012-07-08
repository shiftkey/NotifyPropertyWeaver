using System;
using System.ComponentModel;
using NotifyPropertyWeaver;

public class ClassWithExceptionInProperty : INotifyPropertyChanged
{
    [NotifyProperty]
    public string Property
    {
        get { throw new NotImplementedException(); }
        set { throw new NotImplementedException(); }
    }

    public event PropertyChangedEventHandler PropertyChanged;
}