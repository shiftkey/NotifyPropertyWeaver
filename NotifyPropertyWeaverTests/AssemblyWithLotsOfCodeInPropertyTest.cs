using NUnit.Framework;

[TestFixture]
public class AssemblyWithLotsOfCodeInPropertyTest
{
    [Test]
    public void Simple()
    {
        var weaverHelper = new WeaverHelper(@"AssemblyWithLotsOfCodeInProperty\AssemblyWithLotsOfCodeInProperty.csproj", true);
        var instance = weaverHelper.Assembly.GetInstance("ClassWithProperties");
        EventTester.TestProperty(instance, false);
    }
}