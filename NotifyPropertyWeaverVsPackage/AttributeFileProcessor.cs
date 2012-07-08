using System.ComponentModel.Composition;
using System.IO;
using EnvDTE;

[Export, PartCreationPolicy(CreationPolicy.Shared)]
public class AttributeFileProcessor
{
    NotifyPropertyWeaverFileExporter fileExporter;
    FullPathResolver fullPathResolver;

    [ImportingConstructor]
    public AttributeFileProcessor(NotifyPropertyWeaverFileExporter fileExporter, FullPathResolver fullPathResolver)
    {
        this.fileExporter = fileExporter;
        this.fullPathResolver = fullPathResolver;
    }


    public void ProcessAttributeFile(Project project)
    {
        var dependenciesDirectory = ProjectReader.GetDependenciesDirectory(project.FullName);
        if (dependenciesDirectory == null)
        {
            return;
        }

        var directoryInfo = fullPathResolver.GetFullPath(dependenciesDirectory, project);
        var targetFile = new FileInfo(Path.Combine(directoryInfo.FullName, "NotifyPropertyWeaver.dll"));

        if (!targetFile.Exists)
        {
            return;
        }
        if (VersionChecker.IsVersionNewer(targetFile))
        {
            var frameworkType = FrameworkTypeReader.GetFrameworkType(project.FullName);
            fileExporter.ExportAttribute(directoryInfo, frameworkType);
        }
    }

}