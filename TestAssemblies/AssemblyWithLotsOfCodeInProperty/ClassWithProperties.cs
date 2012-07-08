using System.ComponentModel;
using System.Diagnostics;


public class ClassWithProperties : INotifyPropertyChanged
{
    string property1;
    public string Property1
    {
        get { return property1; }
        set
        {
            Debug.WriteLine("sdfsdf");
            Debug.WriteLine("sdfsdf");
            property1 = value;
            Debug.WriteLine("sdfsdf");
            Debug.WriteLine("sdfsdf");
            Debug.WriteLine("sdfsdf");
            Debug.WriteLine("sdfsdf");
            Debug.WriteLine("sdfsdf");
            Debug.WriteLine("sdfsdf");
            Debug.WriteLine("sdfsdf");
            Debug.WriteLine("sdfsdf");
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
}