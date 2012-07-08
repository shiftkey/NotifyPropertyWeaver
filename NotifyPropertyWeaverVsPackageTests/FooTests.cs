using System;
using System.Diagnostics;
using NUnit.Framework;

[TestFixture]
public class FooTests
{

    [Test]
    public void Bar()
    {
        Debug.WriteLine(Environment.ExpandEnvironmentVariables(@"%appdata%\NotifyPropertyWeaver"));
    }
}