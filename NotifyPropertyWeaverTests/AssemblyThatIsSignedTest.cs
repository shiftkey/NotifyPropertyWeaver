using System.Linq;
using System.Reflection;
using NUnit.Framework;

[TestFixture]
public class AssemblyThatIsSignedTest
{
    [Test]
    public void EnsureReferenceToNotifyPropertyWeaverIsRemoved()
    {
        var weaverHelper = new WeaverHelper(@"AssemblyThatIsSigned\AssemblyThatIsSigned.csproj", false);
        var instance = weaverHelper.Assembly.GetInstance("ClassWithProperties");
        EventTester.TestProperty(instance, false);
        Assert.AreEqual(GetAssemblyPublickKeyToken(weaverHelper.Assembly), "3a0be277382290ee");
    }

    static string GetAssemblyPublickKeyToken(Assembly assembly)
    {
        var token = assembly.GetName().GetPublicKeyToken();
        return token.Select(x => x.ToString("x2")).Aggregate((x, y) => x + y);
    }
}