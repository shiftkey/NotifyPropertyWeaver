using System;
using NUnit.Framework;

[TestFixture]
public class ExceptionExtensionsTests
{

    [Test]
    [Ignore]
    public void ExceptionHierarchyToString()
    {
        Exception exception1 = null;
        try
        {
            ThrowException1();
        }
        catch (Exception exception)
        {
            exception1 = exception;
        }
        var exceptionAsString = exception1.ExceptionHierarchyToString();
        Assert.AreEqual(
@"Top-level Exception
Type:        System.Exception
Message:     Exceltion1
Source:      NotifyPropertyWeaverVsPackageTests
Stack Trace: at NotifyPropertyWeaverVsPackageTests.ExceptionExtensionsTests.ThrowException1() in D:\Code\notifypropertyweaver\NotifyPropertyWeaverVsPackageTests\ExceptionExtensionsTests.cs:line 50
   at NotifyPropertyWeaverVsPackageTests.ExceptionExtensionsTests.ExceptionHierarchyToString() in D:\Code\notifypropertyweaver\NotifyPropertyWeaverVsPackageTests\ExceptionExtensionsTests.cs:line 18

Inner Exception 1
Type:        System.InvalidOperationException
Message:     Exception2
Source:      NotifyPropertyWeaverVsPackageTests
Stack Trace: at NotifyPropertyWeaverVsPackageTests.ExceptionExtensionsTests.ThrowException2() in D:\Code\notifypropertyweaver\NotifyPropertyWeaverVsPackageTests\ExceptionExtensionsTests.cs:line 53
   at NotifyPropertyWeaverVsPackageTests.ExceptionExtensionsTests.ThrowException1() in D:\Code\notifypropertyweaver\NotifyPropertyWeaverVsPackageTests\ExceptionExtensionsTests.cs:line 44", exceptionAsString);
    }
    void ThrowException1()
    {
        try
        {
            ThrowException2();
        }
        catch (Exception exception)
        {
            throw new Exception("Exceltion1", exception);
        }
    }
    void ThrowException2()
    {
        throw new InvalidOperationException("Exception2");
    }
}