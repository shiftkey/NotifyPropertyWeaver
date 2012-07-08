using System.ComponentModel;
using NotifyPropertyWeaver;

namespace GenericBaseWithPropertyBeforeAfter
{
    public class ClassWithGenericPropertyParent<T> : INotifyPropertyChanged
    {
        [NotifyProperty]
        public T Property1 { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName, object before, object after)
        {
            var propertyChanged = PropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}