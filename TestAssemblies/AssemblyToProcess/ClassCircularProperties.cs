using System.ComponentModel;
using NotifyPropertyWeaver;

[NotifyForAll]
public class ClassCircularProperties : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
    [DependsOn("Self")]
    public string Self { get; set; }

    [DependsOn("PropertyA2")]
    public string PropertyA1 { get; set; }

    [DependsOn("PropertyA1")]
    public string PropertyA2 { get; set; }

    [NotifyProperty(AlsoNotifyFor = new[] { "PropertyB2" })]
    public string PropertyB1 { get; set; }

    [NotifyProperty(AlsoNotifyFor = new[] { "PropertyB1" })]
    public string PropertyB2 { get; set; }
}