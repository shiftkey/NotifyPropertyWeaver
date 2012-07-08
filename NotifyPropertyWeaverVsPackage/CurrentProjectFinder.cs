using System.ComponentModel.Composition;
using System.Linq;
using System.Runtime.InteropServices;
using EnvDTE;
using Microsoft.VisualStudio.Shell;

[Export, PartCreationPolicy(CreationPolicy.Shared)]
public class CurrentProjectFinder
{


    public Project GetCurrentProject()
    {
        var dte = (DTE) ServiceProvider.GlobalProvider.GetService(typeof (DTE));
        if (dte.Solution == null)
        {
            return null;
        }
        if (string.IsNullOrEmpty(dte.Solution.FullName))
        {
            return null;
        }
        try
        {
            var objects = (object[]) dte.ActiveSolutionProjects;
            return (Project) objects.FirstOrDefault();
        }
        catch (COMException)
        {
            return null;
        }
    }
}