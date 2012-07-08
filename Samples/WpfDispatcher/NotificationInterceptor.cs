using System;
using System.Windows;

namespace WpfDispatcher
{

public static class PropertyNotificationInterceptor
{
    public static void Intercept(object target, Action onPropertyChangedAction, string propertyName)
    {
        Application.Current.Dispatcher.Invoke(onPropertyChangedAction);
    }
}
}