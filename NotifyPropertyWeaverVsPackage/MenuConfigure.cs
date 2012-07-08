using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using EnvDTE;
using Microsoft.VisualStudio.Shell;

[Export, PartCreationPolicy(CreationPolicy.Shared)]
public class MenuConfigure
{
    OleMenuCommand configureCommand;
    OleMenuCommand disableCommand;
    CurrentProjectFinder currentProjectFinder;
    ExceptionDialog exceptionDialog;
    ConfigureMenuCallback configureMenuCallback;
    DisableMenuConfigure disableMenuConfigure;
    IMenuCommandService menuCommandService;

    [ImportingConstructor]
    public MenuConfigure(CurrentProjectFinder currentProjectFinder, ExceptionDialog exceptionDialog, ConfigureMenuCallback configureMenuCallback, DisableMenuConfigure disableMenuConfigure, IMenuCommandService menuCommandService)
    {
        this.exceptionDialog = exceptionDialog;
        this.configureMenuCallback = configureMenuCallback;
        this.disableMenuConfigure = disableMenuConfigure;
        this.menuCommandService = menuCommandService;
        this.currentProjectFinder = currentProjectFinder;
    }

    public void RegisterMenus()
    {
        var cmdSet = new Guid("af0fbcae-2924-42bf-adb7-31202b59250a");
        var configureCommandId = new CommandID(cmdSet, 1);
        configureCommand = new OleMenuCommand(delegate { configureMenuCallback.ConfigureCallback(); }, configureCommandId);
        configureCommand.BeforeQueryStatus += delegate { CommandStatusCheck(); };
        menuCommandService.AddCommand(configureCommand);

        var disableCommandId = new CommandID(cmdSet, 2);
        disableCommand = new OleMenuCommand(delegate { disableMenuConfigure.DisableCallback(); }, disableCommandId)
                             {
                                 Enabled = false
                             };
        disableCommand.BeforeQueryStatus += delegate { CommandStatusCheck(); };
        menuCommandService.AddCommand(disableCommand);
    }

    void CommandStatusCheck()
    {
        try
        {
            disableCommand.Enabled = true;
            configureCommand.Enabled = true;
            var project = currentProjectFinder.GetCurrentProject();
            if (project == null)
            {
                disableCommand.Enabled = false;
                configureCommand.Enabled = false;
                return;
            }
            var xmlForProject = LoadXmlForProject(project);
            if (xmlForProject == null)
            {

                disableCommand.Enabled = false;
                configureCommand.Enabled = false;
                return;
            }
            disableCommand.Enabled = ContainsWeavingTask(xmlForProject);
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

    static XDocument LoadXmlForProject(Project project)
    {
        string fullName;
        try
        {
            fullName = project.FullName;
        }
        catch (NotImplementedException)
        {
            //HACK: can happen during an upgrade from VS 2008
            return null;
        }
        //cant add to deployment projects
        if (fullName.EndsWith(".vdproj"))
        {
            return null;
        }
        //HACK: for when VS incorrectly calls configure when no project is avaliable
        if (string.IsNullOrWhiteSpace(fullName))
        {
            return null;
        }
        //HACK: for web projects
        if (!File.Exists(fullName))
        {
            return null;
        }
        try
        {
            //validate is xml
            return XDocument.Load(fullName);
        }
        catch (Exception)
        {
            //this means it is not xml and we cant do anything with it
            return null;
        }
    }

    static bool ContainsWeavingTask(XDocument xDocument)
    {
        try
        {
            return xDocument.BuildDescendants("NotifyPropertyWeaverMsBuildTask.WeavingTask").Any();
        }
        catch (Exception exception)
        {
            throw new Exception("Could not check project for weaving task.", exception);
        }
    }
}