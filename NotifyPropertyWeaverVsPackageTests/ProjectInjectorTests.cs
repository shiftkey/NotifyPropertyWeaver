using System.IO;
using Microsoft.Build.Framework;
using NUnit.Framework;

[TestFixture]
public class ProjectInjectorTests
{
    [Test]
    public void WithNoWeaving()
    {
        var sourceProjectFile = new FileInfo(@"TestProjects\ProjectWithNoWeaving.csproj");
        var targetFileInfo = sourceProjectFile.CopyTo(sourceProjectFile.FullName + "ProjectInjectorTests", true);
        try
        {

            var injector = new NotifyPropertyWeaverProjectInjector
            {
                CheckForEquality = true,
				CheckForIsChanged = true,
                DependenciesDirectory = @"Lib\",
                ToolsDirectory = @"Tools\",
                EventInvokerName = "OnPropertyChanged",
                ProjectFile = targetFileInfo.FullName,
                IncludeAttributeAssembly = true,
                ProcessFields = true,
                TargetPath = "Foo.dll",
                Target = "AfterCompile",
                TryToWeaveAllTypes = true,
                MessageImportance = MessageImportance.High,
            };
            injector.Execute();

            var reader = new ProjectReader(targetFileInfo.FullName);

            Assert.IsTrue(reader.CheckForEquality.Value);
			Assert.IsTrue(reader.CheckForIsChanged.Value);
			Assert.IsTrue(reader.ProcessFields.Value);
            Assert.IsTrue(reader.TryToWeaveAllTypes.Value);
            Assert.AreEqual("OnPropertyChanged", reader.EventInvokerName);
            Assert.AreEqual("Foo.dll", reader.TargetPath);
            Assert.AreEqual(@"Lib\", reader.DependenciesDirectory);
            Assert.AreEqual(@"Tools\", reader.ToolsDirectory);
            Assert.AreEqual(MessageImportance.High, reader.MessageImportance);
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
        var targetFileInfo = sourceProjectFile.CopyTo(sourceProjectFile.FullName + "ProjectInjectorTests", true);

        try
        {
            var injector = new NotifyPropertyWeaverProjectInjector
            {
                CheckForEquality = false,
				CheckForIsChanged = false,
                DependenciesDirectory = @"Lib2\",
                ToolsDirectory = @"Tools2\",
                EventInvokerName = "OnPropertyChanged2",
                ProjectFile = targetFileInfo.FullName,
                IncludeAttributeAssembly = true,
                ProcessFields = false,
                Target = "AfterCompile",
                TargetPath = "Foo2.dll",
                TryToWeaveAllTypes = false,
                MessageImportance = MessageImportance.High,
            };
            injector.Execute();

            var reader = new ProjectReader(targetFileInfo.FullName);

            Assert.IsFalse(reader.CheckForEquality.Value);
			Assert.IsFalse(reader.CheckForIsChanged.Value);
            Assert.IsFalse(reader.TryToWeaveAllTypes.Value);
            Assert.IsFalse(reader.ProcessFields.Value);
            Assert.AreEqual("OnPropertyChanged2", reader.EventInvokerName);
            Assert.AreEqual("Foo2.dll", reader.TargetPath);
            Assert.AreEqual(@"Lib2\", reader.DependenciesDirectory);
            Assert.AreEqual(@"Tools2\", reader.ToolsDirectory);
            Assert.AreEqual(MessageImportance.High, reader.MessageImportance);

        }
        finally
        {
            targetFileInfo.Delete();
        }

    }

}