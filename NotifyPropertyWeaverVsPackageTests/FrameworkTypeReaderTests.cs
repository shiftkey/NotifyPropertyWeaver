using NUnit.Framework;

[TestFixture]
public class FrameworkTypeReaderTests
{

    [Test]
    public void GetFrameworkType()
    {
        Assert.AreEqual("DotNetStandard", FrameworkTypeReader.GetFrameworkType(@"TestProjects\ProjectDotNet.csproj"));
        Assert.AreEqual("Silverlight", FrameworkTypeReader.GetFrameworkType(@"TestProjects\ProjectSilverlight.csproj"));
        Assert.AreEqual("Phone", FrameworkTypeReader.GetFrameworkType(@"TestProjects\ProjectPhone.csproj"));
    }
}