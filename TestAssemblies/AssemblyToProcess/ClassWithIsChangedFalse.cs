using System.ComponentModel;
using NotifyPropertyWeaver;

public class ClassWithIsChangedFalse : INotifyPropertyChanged
{
	[NotifyProperty(SetIsChanged = false)]
	public string Property1 { get; set; }
	public bool IsChanged { get; set; }

	public event PropertyChangedEventHandler PropertyChanged;
}