using Jounce.Core.ViewModel;
using NotifyPropertyWeaver;

public class ClassJounceBaseViewModel : BaseViewModel
{
    [NotifyProperty]
    public string Property1 { get; set; }
}