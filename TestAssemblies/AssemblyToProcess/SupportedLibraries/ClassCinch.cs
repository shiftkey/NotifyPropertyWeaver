using Cinch;
using NotifyPropertyWeaver;


public class ClassCinch : ViewModelBase
{
    [NotifyProperty]
    public string Property1 { get; set; }
}