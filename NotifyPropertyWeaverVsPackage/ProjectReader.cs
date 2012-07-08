using System;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Build.Framework;

public class ProjectReader
{
    string projectFile;
    public string DependenciesDirectory;
    public string ToolsDirectory;
    public string TargetPath;
    public string EventInvokerName;
    public MessageImportance? MessageImportance;
    public string TargetNode;
    public bool? CheckForEquality;
    public bool? ProcessFields;
    public bool? CheckForIsChanged;
    public bool? TryToWeaveAllTypes;

    public ProjectReader(string projectFile)
    {
        this.projectFile = projectFile;
        SetWeavingProps();
        DependenciesDirectory = GetDependenciesDirectory(projectFile);
        ToolsDirectory = GetToolsDirectory(projectFile);
    }

    public static string GetDependenciesDirectory(string projectFile)
    {
        var xDocument = ReadXDocument(projectFile);
        var elements =
            from el in xDocument.BuildDescendants("Reference")
            where ((string) el.Attribute("Include")).StartsWith("NotifyPropertyWeaver")
            select el;
        var firstOrDefault = elements.FirstOrDefault();
        if (firstOrDefault != null)
        {
            var value = firstOrDefault.Value;
            var indexOf = value.IndexOf("NotifyPropertyWeaver.dll", StringComparison.OrdinalIgnoreCase);
            if (indexOf > 0)
            {
                return value.Substring(0, indexOf);
            }
        }

        return null;
    }

    public static string GetToolsDirectory(string projectFile)
    {
        var xDocument = ReadXDocument(projectFile);
        var elements =
            from el in xDocument.BuildDescendants("UsingTask")
            where (string) el.Attribute("TaskName") == "NotifyPropertyWeaverMsBuildTask.WeavingTask"
            select el.Attribute("AssemblyFile");
        var firstOrDefault = elements.FirstOrDefault();
        if (firstOrDefault != null)
        {
            var value = firstOrDefault.Value;
            return value.Substring(0, value.IndexOf("NotifyPropertyWeaverMsBuildTask.dll", StringComparison.OrdinalIgnoreCase));
        }
        return null;
    }

    static XDocument ReadXDocument(string projectFile)
    {
        try
        {
            return XDocument.Load(projectFile);
        }
        catch (Exception exception)
        {
            throw new Exception(string.Format("Could not load project file '{0}'.", projectFile), exception);
        }
    }
    void SetWeavingProps()
    {
        var xDocument = XDocument.Load(projectFile);
        var weavingTask = xDocument.BuildDescendants("NotifyPropertyWeaverMsBuildTask.WeavingTask").FirstOrDefault();
        if (weavingTask == null)
        {
            return;
        }
        TryToWeaveAllTypes = ToBool(weavingTask.Attribute("TryToWeaveAllTypes"));
        CheckForEquality = ToBool(weavingTask.Attribute("CheckForEquality"));
        CheckForIsChanged = ToBool(weavingTask.Attribute("CheckForIsChanged"));
        ProcessFields = ToBool(weavingTask.Attribute("ProcessFields"));
        EventInvokerName = (string) weavingTask.Attribute("EventInvokerName");
        TargetPath = (string) weavingTask.Attribute("TargetPath");
        MessageImportance = ConvertToEnum((string) weavingTask.Attribute("MessageImportance"));
        var xAttribute = weavingTask.Parent.Attribute("Name");
        if (xAttribute == null)
        {
            throw new Exception("Target node contains no 'Name' attribute.");
        }
        ;
        TargetNode = xAttribute.Value;
    }

    public static bool? ToBool(XAttribute attribute)
    {
        if (attribute == null)
        {
            return null;
        }
        return bool.Parse(attribute.Value);
    }

    static MessageImportance? ConvertToEnum(string messageImportance)
    {
        if (!string.IsNullOrWhiteSpace(messageImportance))
        {
            MessageImportance messageImportanceEnum;
            if (Enum.TryParse(messageImportance, out messageImportanceEnum))
            {
                return messageImportanceEnum;
            }
        }
        return null;
    }
}