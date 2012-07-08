using System.CodeDom.Compiler;
using System.ComponentModel;
using NotifyPropertyWeaver;


[GeneratedCode("asd","asd")]
public class ClassWithGeneratedCodeAttribute : INotifyPropertyChanged
{
    [NotifyProperty]
    public string Property1 { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;
}
