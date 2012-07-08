using NUnit.Framework;


[TestFixture]
public class ReferencingExecutableTests
{
    [Test]
    public void Foo()
    {
        var weaverHelper = new WeaverHelper(@"AssemblyReferencingExecutable\AssemblyReferencingExecutable.csproj", true);

        var instance = weaverHelper.Assembly.GetInstance("ClassChild");
        EventTester.TestProperty(instance, false);
    }
}