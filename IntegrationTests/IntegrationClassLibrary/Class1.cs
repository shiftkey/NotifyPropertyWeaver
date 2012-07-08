using System;
using System.ComponentModel;

namespace IntegrationClassLibrary
{
    public class Class1:INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Property1 { get; set; }
    }
    public static class PropertyNotificationInterceptor
    {
        public static void Intercept(object target, Action action, string propertyName)
        {
            action();
            InterceptCalled = true;
        }

        public static bool InterceptCalled { get; set; }
    }

}
