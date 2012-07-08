using System.IO;
using System.Linq;
using System.Xml.Linq;

public class ProjectRemover
{
    XDocument xDocument;

    public ProjectRemover(string projectFile)
    {
        new FileInfo(projectFile).IsReadOnly = false;
        xDocument = XDocument.Load(projectFile);
        RemoveReference();
        RemoveUsingTask();
        RemoveWeavingTask();
        xDocument.Save(projectFile);
    }

    void RemoveWeavingTask()
    {
        xDocument.Descendants(XDocumentExtensions.BuildNamespace + "NotifyPropertyWeaverMsBuildTask.WeavingTask")
            .Remove();
    }

    void RemoveUsingTask()
    {
        xDocument.BuildDescendants("UsingTask")
            .Where(x => (string) x.Attribute("TaskName") == "NotifyPropertyWeaverMsBuildTask.WeavingTask")
            .Remove();
    }

    void RemoveReference()
    {
        xDocument.BuildDescendants("Reference")
            .Where(x => ((string) x.Attribute("Include")).StartsWith("NotifyPropertyWeaver"))
            .Remove();
    }
}