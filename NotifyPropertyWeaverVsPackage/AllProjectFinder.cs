using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using EnvDTE;
using Microsoft.VisualStudio.Shell;

[Export, PartCreationPolicy(CreationPolicy.Shared)]
public class AllProjectFinder
{
    public IEnumerable<Project> GetAllProjects()
    {
        var projectList = new List<Project>();
        var dte = (DTE) ServiceProvider.GlobalProvider.GetService(typeof (DTE));
        foreach (Project project in dte.Solution.Projects)
        {
            if (ProjectKind.IsSupportedProjectKind(project.Kind))
            {
                projectList.Add(project);
            }
            FindProjectInternal(project.ProjectItems, projectList);
        }
        return projectList;
    }


    static void FindProjectInternal(ProjectItems items, List<Project> projectList)
    {
        if (items == null)
        {
            return;
        }

        foreach (ProjectItem item in items)
        {
            try
            {
                var project = item.SubProject;
                if (project == null)
                {
                    project = item.Object as Project;
                }
                if (project != null)
                {
                    if (ProjectKind.IsSupportedProjectKind(project.Kind))
                    {
                        projectList.Add(project);
                    }
                    FindProjectInternal(project.ProjectItems, projectList);
                }
            }
            catch (NotImplementedException)
            {
            }
            catch (NullReferenceException)
            {
            }
        }
    }

}