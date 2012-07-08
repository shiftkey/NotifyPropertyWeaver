using System.ComponentModel.Composition;
using System.IO;
using EnvDTE;

[Export, PartCreationPolicy(CreationPolicy.Shared)]
public class TaskFileProcessor
{
    TaskFileReplacer taskFileReplacer;
    FullPathResolver fullPathResolver;

    [ImportingConstructor]
    public TaskFileProcessor(TaskFileReplacer taskFileReplacer, FullPathResolver fullPathResolver)
    {
        this.taskFileReplacer = taskFileReplacer;
        this.fullPathResolver = fullPathResolver;
    }

    public void ProcessTaskFile(Project project)
    {
        var toolsDirectory = ProjectReader.GetToolsDirectory(project.FullName);
        if (toolsDirectory == null)
        {
            return;
        }

        var directoryInfo = fullPathResolver.GetFullPath(toolsDirectory, project);
        var targetFile = new FileInfo(Path.Combine(directoryInfo.FullName, "NotifyPropertyWeaverMsBuildTask.dll"));

        if (!targetFile.Exists)
        {
            return;
        }
        if (VersionChecker.IsVersionNewer(targetFile))
        {
            taskFileReplacer.AddFile(directoryInfo);
        }
    }

}