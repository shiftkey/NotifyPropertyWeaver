using System.ComponentModel.Composition;
using System.IO;

[Export, PartCreationPolicy(CreationPolicy.Shared)]
public class NotifyPropertyWeaverFileExporter
{
    ResourceExporter resourceExporter;

    public NotifyPropertyWeaverFileExporter()
    {
        resourceExporter = new ResourceExporter();
    }

    [ImportingConstructor]
    public NotifyPropertyWeaverFileExporter(ResourceExporter resourceExporter)
    {
        this.resourceExporter = resourceExporter;
    }

    public virtual bool ExportTask(string directory)
    {
        return resourceExporter.Export("NotifyPropertyWeaverMsBuildTask.dll", new FileInfo(Path.Combine(directory, "NotifyPropertyWeaverMsBuildTask.dll")));
    }

    public virtual bool ExportTask(DirectoryInfo directory)
    {
        return resourceExporter.Export("NotifyPropertyWeaverMsBuildTask.dll", new FileInfo(Path.Combine(directory.FullName, "NotifyPropertyWeaverMsBuildTask.dll")));
    }

    public virtual void ExportAttribute(DirectoryInfo directory, string frameworkType)
    {
        resourceExporter.Export(frameworkType + ".NotifyPropertyWeaver.dll", new FileInfo(Path.Combine(directory.FullName, "NotifyPropertyWeaver.dll")));
        resourceExporter.Export("NotifyPropertyWeaver.xml", new FileInfo(Path.Combine(directory.FullName, "NotifyPropertyWeaver.xml")));
    }


}
