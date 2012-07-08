#if (DEBUG)
using System;
using System.IO;
using System.Threading;
using NSubstitute;
using NUnit.Framework;

[TestFixture]
public class TaskFileReplacerTests
{

    [Test]
    public void AddFileNotExists()
    {
        var errorDisplayer = Substitute.For<ErrorDisplayer>();
        var taskFileReplacer = new TaskFileReplacer(errorDisplayer, null);
        taskFileReplacer.ClearFile();
        taskFileReplacer.AddFile(new DirectoryInfo(@"C:\Foo"));
        Assert.AreEqual("C:\\Foo\r\n", File.ReadAllText(taskFileReplacer.taskFilePath));
    }

    [Test]
    public void AddFileNotExists2()
    {
        var errorDisplayer = Substitute.For<ErrorDisplayer>();
        var taskFileReplacer = new TaskFileReplacer(errorDisplayer, null);
        taskFileReplacer.ClearFile();
        taskFileReplacer.AddFile(new DirectoryInfo(@"C:\Foo"));
        taskFileReplacer.AddFile(new DirectoryInfo(@"C:\Foo2"));
        Assert.AreEqual("C:\\Foo\r\nC:\\Foo2\r\n", File.ReadAllText(taskFileReplacer.taskFilePath));
    }

    [Test]
    public void CheckForFilesToUpdateNotExists()
    {
        var errorDisplayer = Substitute.For<ErrorDisplayer>();
        var taskFileReplacer = new TaskFileReplacer(errorDisplayer, null);
        taskFileReplacer.ClearFile();
        taskFileReplacer.AddFile(new DirectoryInfo(@"C:\Foo"));
        taskFileReplacer.AddFile(new DirectoryInfo(@"C:\Foo2"));
        Thread.Sleep(300);
        taskFileReplacer.CheckForFilesToUpdate();
        Thread.Sleep(300);
        Assert.AreEqual("", File.ReadAllText(taskFileReplacer.taskFilePath));
    }

    [Test]
    public void CheckForFilesToUpdateExportFails()
    {
        var errorDisplayer = Substitute.For<ErrorDisplayer>();
        var resourceExporter = Substitute.For<NotifyPropertyWeaverFileExporter>(Substitute.For<ResourceExporter>());
        resourceExporter.ExportTask(Arg.Any<string>())
            .Returns(false);
        var taskFileReplacer = new TaskFileReplacer(errorDisplayer, resourceExporter);
        taskFileReplacer.ClearFile();

        var dir1 = new DirectoryInfo(Environment.CurrentDirectory + @"..\..\..\Dir1");
        taskFileReplacer.AddFile(dir1);
        var dir2 = new DirectoryInfo(Environment.CurrentDirectory + @"..\..\..\Dir2");
        taskFileReplacer.AddFile(dir2);

        taskFileReplacer.CheckForFilesToUpdate();
        Assert.AreEqual(dir1.FullName + "\r\n" + dir2.FullName + "\r\n", File.ReadAllText(taskFileReplacer.taskFilePath));
    }
}
#endif
