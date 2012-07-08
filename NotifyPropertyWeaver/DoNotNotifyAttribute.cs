using System;

namespace NotifyPropertyWeaver
{
    /// <summary>
    /// Exclude a <see cref="Type"/> or property from notification.
    /// </summary>
    /// <remarks>
    /// Used when <see cref="NotifyForAllAttribute"/> is used.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class DoNotNotifyAttribute : Attribute
    {
    }
}