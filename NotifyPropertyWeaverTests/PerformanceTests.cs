using System.Diagnostics;
using NUnit.Framework;


[TestFixture]
public class PerformanceTests
{
    [Test]
    public void Foo()
    {
        var startNew = Stopwatch.StartNew();
        new WeaverHelper(@"AssemblyWithLotsOfClasses\AssemblyWithLotsOfClasses.csproj", true);
        startNew.Stop();
        Debug.WriteLine(startNew.ElapsedMilliseconds + "ms");
    }
}