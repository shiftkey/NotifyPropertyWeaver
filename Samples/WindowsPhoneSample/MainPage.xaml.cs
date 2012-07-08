using System.ComponentModel;

namespace WindowsPhoneSample
{
    public partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();
            var person = new Person();
            person.PropertyChanged += PropertyChanged;
            DataContext = person;
        }

        private void PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var value = sender.GetType().GetProperty(e.PropertyName).GetValue(sender, null);
            eventsTextBox.Text = string.Format("PropertyChanged fired. \r\n\tPropertyName='{0}'\r\n\tPropertyValue='{1}'\r\n", e.PropertyName, value) + eventsTextBox.Text;
        }
    }
}