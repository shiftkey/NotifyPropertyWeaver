using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using EnvDTE;

[Export, PartCreationPolicy(CreationPolicy.Shared)]
public class ConfigureMenuCallback
{
    NotifyPropertyWeaverFileExporter fileExporter;
    TaskFileReplacer taskFileReplacer;
    CurrentProjectFinder currentProjectFinder;
    FullPathResolver fullPathResolver;
    ExceptionDialog exceptionDialog;

    [ImportingConstructor]
    public ConfigureMenuCallback(CurrentProjectFinder currentProjectFinder, NotifyPropertyWeaverFileExporter fileExporter, TaskFileReplacer taskFileReplacer, FullPathResolver fullPathResolver, ExceptionDialog exceptionDialog)
    {
        this.currentProjectFinder = currentProjectFinder;
        this.fullPathResolver = fullPathResolver;
        this.exceptionDialog = exceptionDialog;
        this.taskFileReplacer = taskFileReplacer;
        this.fileExporter = fileExporter;
    }


    public void ConfigureCallback()
    {
        try
        {
            var project = currentProjectFinder.GetCurrentProject();

            if (UnsaveProjectChecker.HasUnsavedPendingChanges(project))
            {
                return;
            }
            var projectReader = new ProjectReader(project.FullName);

            var model = new ConfigureWindowModel();
            var defaulter = new Defaulter();
            defaulter.ToModel(projectReader, model);

            var configureWindow = new ConfigureWindow(model);
            new WindowInteropHelper(configureWindow)
                {
                    Owner = GetActiveWindow()
                };
            if (configureWindow.ShowDialog().GetValueOrDefault())
            {
                Configure(model, project);
            }
        }
        catch (COMException exception)
        {
            exceptionDialog.HandleException(exception);
        }
        catch (Exception exception)
        {
            exceptionDialog.HandleException(exception);
        }
    }

    [DllImport("user32.dll")]
    static extern IntPtr GetActiveWindow();


    void Configure(ConfigureWindowModel model, Project project)
    {

        var directoryInfo = fullPathResolver.GetFullPath(model.ToolsDirectory, project);
        var targetFile = new FileInfo(Path.Combine(directoryInfo.FullName, "NotifyPropertyWeaverMsBuildTask.dll"));
        if (!targetFile.Exists || VersionChecker.IsVersionNewer(targetFile))
        {
            if (!fileExporter.ExportTask(directoryInfo))
            {
                taskFileReplacer.AddFile(directoryInfo);
            }
        }

        if (model.IncludeAttributeAssembly)
        {
            var frameworkType = FrameworkTypeReader.GetFrameworkType(project.FullName);
            fileExporter.ExportAttribute(fullPathResolver.GetFullPath(model.DependenciesDirectory, project), frameworkType);
        }

        var defaulter = new Defaulter();
        var projectInjector = new NotifyPropertyWeaverProjectInjector
                                  {
                                      ProjectFile = project.FullName
                                  };
        defaulter.FromModel(projectInjector, model);
        projectInjector.Execute();
    }


}