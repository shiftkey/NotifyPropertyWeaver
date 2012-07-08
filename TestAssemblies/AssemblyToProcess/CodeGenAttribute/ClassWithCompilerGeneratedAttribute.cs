using System.ComponentModel;
using System.Runtime.CompilerServices;
using NotifyPropertyWeaver;

[CompilerGenerated]
public class ClassWithCompilerGeneratedAttribute : INotifyPropertyChanged
{

    [NotifyProperty]
    public string Property1 { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;
}