using Jounce.Core.Model;
using NotifyPropertyWeaver;

public class ClassJounceBaseNotify : BaseNotify
{
    [NotifyProperty]
    public string Property1 { get; set; }
}