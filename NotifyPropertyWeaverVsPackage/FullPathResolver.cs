using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Text;
using EnvDTE;
using Microsoft.VisualStudio.Shell;

[Export, PartCreationPolicy(CreationPolicy.Shared)]
public class FullPathResolver
{

    public DirectoryInfo GetFullPath(string path, Project project)
    {
        var dte = (DTE) ServiceProvider.GlobalProvider.GetService(typeof (DTE));
        var solutionDirectory = Path.GetDirectoryName(dte.Solution.FullName) + @"\";
        var projectDirectory = Path.GetDirectoryName(project.FullName) + @"\";
        path = ReplaceCaseLess(path, "$(SolutionDir)", solutionDirectory);
        path = ReplaceCaseLess(path, "$(ProjectDir)", projectDirectory);
        return new DirectoryInfo(Path.Combine(projectDirectory, path));
    }

    public static string ReplaceCaseLess(string str, string oldValue, string newValue)
    {
        var sb = new StringBuilder();

        var previousIndex = 0;
        var index = str.IndexOf(oldValue, StringComparison.OrdinalIgnoreCase);
        while (index != -1)
        {
            sb.Append(str.Substring(previousIndex, index - previousIndex));
            sb.Append(newValue);
            index += oldValue.Length;

            previousIndex = index;
            index = str.IndexOf(oldValue, index, StringComparison.OrdinalIgnoreCase);
        }
        sb.Append(str.Substring(previousIndex));

        return sb.ToString();
    }
}