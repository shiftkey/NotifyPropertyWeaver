using System.ComponentModel;

namespace WpfSample
{
    public class Person : INotifyPropertyChanged
    {
        public string GivenNames;
        public string FamilyName;

        public string FullName
        {
            get
            {
                return string.Format("{0} {1}", GivenNames, FamilyName);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
