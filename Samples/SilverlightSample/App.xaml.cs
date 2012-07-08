using System.Windows;

namespace SilverlightSample
{
    public partial class App
    {

        public App()
        {
            Startup += Application_Startup;
            InitializeComponent();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            RootVisual = new MainPage();
        }

    }
}
