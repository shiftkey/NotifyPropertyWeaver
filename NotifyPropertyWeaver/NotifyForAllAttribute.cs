using System;
using System.Reflection;

namespace NotifyPropertyWeaver
{

    /// <summary>
    /// Enables property notification for all properties on the <see cref="Type"/> or <see cref="Assembly"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class NotifyForAllAttribute : Attribute
    {
    }
}