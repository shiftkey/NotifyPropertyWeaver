using System.ComponentModel;
using NotifyPropertyWeaver;

public class ClassWithNested
{

    public class ClassNested : INotifyPropertyChanged
    {
        [NotifyProperty]
        public string Property1 { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public class ClassNestedNested : INotifyPropertyChanged
        {
            [NotifyProperty]
            public string Property1 { get; set; }

            public event PropertyChangedEventHandler PropertyChanged;

        }
    }
}