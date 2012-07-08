using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading;

[Export, PartCreationPolicy(CreationPolicy.Shared)]
public class TaskFileReplacer
{
    ErrorDisplayer errorDisplayer;
    NotifyPropertyWeaverFileExporter fileExporter;
    public string taskFilePath;

    [ImportingConstructor]
    public TaskFileReplacer(ErrorDisplayer errorDisplayer, NotifyPropertyWeaverFileExporter fileExporter)
    {
        this.errorDisplayer = errorDisplayer;
        this.fileExporter = fileExporter;
        Directory.CreateDirectory(Environment.ExpandEnvironmentVariables(@"%appdata%\NotifyPropertyWeaver"));
        taskFilePath = Environment.ExpandEnvironmentVariables(@"%appdata%\NotifyPropertyWeaver\TaskAssembliesToUpdate.txt");
        if (!File.Exists(taskFilePath))
        {
            using (File.Create(taskFilePath))
            {
            }
        }
    }

    public void ClearFile()
    {
        File.Delete(taskFilePath);
        using (File.Create(taskFilePath))
        {
        }
    }

    public void CheckForFilesToUpdate()
    {
        ThreadPool.QueueUserWorkItem(x =>
                                         {
                                             bool createdNew;
                                             using (new Mutex(true, typeof (TaskFileReplacer).FullName, out createdNew))
                                             {
                                                 if (!createdNew)
                                                 {
                                                     //already being used;
                                                     return;
                                                 }
                                                 var newStrings = new List<string>();
                                                 foreach (var targetDirectory in File.ReadAllLines(taskFilePath))
                                                 {
                                                     var trimmed = targetDirectory.Trim();
                                                     if (trimmed.Length == 0)
                                                     {
                                                         continue;
                                                     }
                                                     var directoryInfo = new DirectoryInfo(trimmed);
                                                     if (!directoryInfo.Exists)
                                                     {
                                                         continue;
                                                     }
                                                     if (fileExporter.ExportTask(directoryInfo))
                                                     {
                                                         var path = Path.Combine(trimmed, "NotifyPropertyWeaverMsBuildTask.dll");
                                                         errorDisplayer.ShowInfo(string.Format("NotifyPropertyWeaver: Updated '{0}' to version {1}.", path, CurrentVersion.Version));
                                                     }
                                                     else
                                                     {
                                                         newStrings.Add(trimmed);
                                                     }
                                                 }
                                                 File.WriteAllLines(taskFilePath, newStrings);
                                             }
                                         });
    }

    public void AddFile(DirectoryInfo directoryInfo)
    {
        using (var mutex = new Mutex(true, typeof (TaskFileReplacer).FullName))
        {
            if (!mutex.WaitOne(100))
            {
                return;
            }
            var allText = File.ReadAllLines(taskFilePath);
            var fileContainsDirectory = allText.Any(line => string.Equals(line, directoryInfo.FullName, StringComparison.OrdinalIgnoreCase));
            if (!fileContainsDirectory)
            {
                errorDisplayer.ShowInfo(string.Format("NotifyPropertyWeaver: Restart of Visual Studio required to update '{0}'.", Path.Combine(directoryInfo.FullName, "NotifyPropertyWeaverMsBuildTask.dll")));
                File.AppendAllText(taskFilePath, directoryInfo.FullName + "\r\n");
            }
        }
    }

}