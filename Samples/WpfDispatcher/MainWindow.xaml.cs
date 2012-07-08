using System.ComponentModel;
using System.Threading;

namespace WpfDispatcher
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            var person = new Person();
            person.PropertyChanged +=PropertyChanged;
            DataContext = person;

            new Thread(() => SetPropertyOnOtherThread(person)).Start();
        }

        private void SetPropertyOnOtherThread(Person person)
        {
            Thread.Sleep(5000);
            person.Name = "BGThread";
        }
        private void PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var value = sender.GetType().GetProperty(e.PropertyName).GetValue(sender, null);
            eventsTextBox.Text = string.Format("PropertyChanged fired. \r\n\tPropertyName='{0}'\r\n\tPropertyValue='{1}'\r\n", e.PropertyName, value) + eventsTextBox.Text;
        }
    }
}
