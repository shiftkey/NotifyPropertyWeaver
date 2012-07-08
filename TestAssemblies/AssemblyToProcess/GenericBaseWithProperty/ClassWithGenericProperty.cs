using System.ComponentModel;
using NotifyPropertyWeaver;

namespace GenericBaseWithProperty
{
    public class ClassWithGenericPropertyParent<T> : INotifyPropertyChanged
    {
        [NotifyProperty]
        public T Property1 { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}