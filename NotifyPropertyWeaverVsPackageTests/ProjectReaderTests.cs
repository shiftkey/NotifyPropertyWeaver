using System.IO;
using Microsoft.Build.Framework;
using NUnit.Framework;

[TestFixture]
public class ProjectReaderTests
{

    [Test]
    public void WithNoWeaving()
    {
        var sourceProjectFile = new FileInfo(@"TestProjects\ProjectWithNoWeaving.csproj");
        var targetFileInfo = sourceProjectFile.CopyTo(sourceProjectFile.FullName + "ProjectReaderTest", true);
        try
        {

            var reader = new ProjectReader(targetFileInfo.FullName);

            Assert.IsNull(reader.DependenciesDirectory);
            Assert.IsNull(reader.ToolsDirectory);
            Assert.IsNull(reader.CheckForEquality);
			Assert.IsNull(reader.CheckForIsChanged);
			Assert.IsNull(reader.ProcessFields);
            Assert.IsNull(reader.MessageImportance);
            Assert.IsNull(reader.TryToWeaveAllTypes);
            Assert.IsNull(reader.EventInvokerName);
            Assert.IsNull(reader.EventInvokerName);
            Assert.IsNull(reader.TargetPath);
            Assert.IsNull(reader.TargetNode);
            Assert.IsNull(reader.DependenciesDirectory);
        }
        finally
        {
            targetFileInfo.Delete();
        }

    }

    [Test]
    public void WithExistingWeaving()
    {
        var sourceProjectFile = new FileInfo(@"TestProjects\ProjectWithWeaving.csproj");
        var targetFileInfo = sourceProjectFile.CopyTo(sourceProjectFile.FullName + "ProjectReaderTest", true);
        try
        {
            var reader = new ProjectReader(targetFileInfo.FullName);

            Assert.IsTrue(reader.CheckForEquality.Value);
			Assert.IsTrue(reader.CheckForIsChanged.Value);
			Assert.IsTrue(reader.ProcessFields.Value);
            Assert.IsTrue(reader.TryToWeaveAllTypes.Value);
            Assert.AreEqual("OnPropertyChanged", reader.EventInvokerName);
            Assert.AreEqual("AfterCompile", reader.TargetNode);
            Assert.AreEqual("@(IntermediateAssembly)", reader.TargetPath);
            Assert.AreEqual("$(SolutionDir)Lib\\", reader.DependenciesDirectory);
            Assert.AreEqual("$(SolutionDir)Tools\\", reader.ToolsDirectory);
            Assert.AreEqual(MessageImportance.High, reader.MessageImportance);
        }
        finally
        {
            targetFileInfo.Delete();
        }
    }


    [Test]
    public void WithMinimalWeaving()
    {

        var sourceProjectFile = new FileInfo(@"TestProjects\ProjectWithMinimalWeaving.csproj");
        var targetFileInfo = sourceProjectFile.CopyTo(sourceProjectFile.FullName + "ProjectReaderTest", true);
        try
        {

            var reader = new ProjectReader(targetFileInfo.FullName);

            Assert.IsNull(reader.CheckForEquality);
			Assert.IsNull(reader.CheckForIsChanged);
			Assert.IsNull(reader.ProcessFields);
            Assert.IsNull(reader.TryToWeaveAllTypes);
            Assert.IsNull(reader.EventInvokerName);
            Assert.IsNull(reader.TargetPath);
            Assert.IsNull(reader.TargetPath);
            Assert.AreEqual("AfterCompile", reader.TargetNode);
            Assert.AreEqual(@"$(SolutionDir)Lib\", reader.DependenciesDirectory);
            Assert.AreEqual(@"$(SolutionDir)Tools\", reader.ToolsDirectory);
            Assert.IsNull(reader.MessageImportance);
        }
        finally
        {
            targetFileInfo.Delete();
        }

    }
}