using Catel.Data;
using NotifyPropertyWeaver;

public class ClassCatel : ObservableObject
{
    [NotifyProperty]
    public string Property1 { get; set; }
}