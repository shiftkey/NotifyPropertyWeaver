using System.IO;
using NUnit.Framework;

[TestFixture]
public class ProjectRemoverTests
{
    [Test]
    public void WithNoWeavingNotChanged()
    {
        var sourceProjectFile = new FileInfo(@"TestProjects\ProjectWithNoWeaving.csproj");
        var targetFileInfo = sourceProjectFile.CopyTo(sourceProjectFile.FullName + "ProjectRemoverTests", true);

        try
        {

            new ProjectRemover(targetFileInfo.FullName);

            var reader = new ProjectReader(targetFileInfo.FullName);

            Assert.IsNull(reader.CheckForEquality);
			Assert.IsNull(reader.CheckForIsChanged);
            Assert.IsNull(reader.TryToWeaveAllTypes);
            Assert.IsNull(reader.EventInvokerName);
            Assert.IsNull(reader.TargetPath);
            Assert.IsNull(reader.DependenciesDirectory);
            Assert.IsNull(reader.ToolsDirectory);
        }
        finally
        {
            targetFileInfo.Delete();
        }
    }


    [Test]
    public void WeavingRemoved()
    {
        var sourceProjectFile = new FileInfo(@"TestProjects\ProjectWithWeaving.csproj");
        var targetFileInfo = sourceProjectFile.CopyTo(sourceProjectFile.FullName + "ProjectRemoverTests", true);

        try
        {

            new ProjectRemover(targetFileInfo.FullName);

            var reader = new ProjectReader(targetFileInfo.FullName);

            Assert.IsNull(reader.CheckForEquality);
			Assert.IsNull(reader.CheckForIsChanged);
            Assert.IsNull(reader.TryToWeaveAllTypes);
            Assert.IsNull(reader.EventInvokerName);
            Assert.IsNull(reader.TargetPath);
            Assert.IsNull(reader.DependenciesDirectory);
            Assert.IsNull(reader.ToolsDirectory);
        }
        finally
        {
            targetFileInfo.Delete();
        }
    }
}