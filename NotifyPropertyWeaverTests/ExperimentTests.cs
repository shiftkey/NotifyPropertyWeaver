using NUnit.Framework;

[TestFixture]
public class ExperimentTests
{
    [Test]
    [Ignore]
    public void Foo()
    {
        var weaverHelper = new WeaverHelper(@"AssemblyExperiments\AssemblyExperiments.csproj", true);

        var instance = weaverHelper.Assembly.GetInstance("ExperimentClass");
        Verifier.Verify(weaverHelper.Assembly.Location);
    }
}