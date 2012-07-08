using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Build.Framework;

public class NotifyPropertyWeaverProjectInjector
{
    public string TargetPath;
    public string EventInvokerName;
    public string DependenciesDirectory;
    public bool? CheckForEquality;
    public MessageImportance? MessageImportance;
    public bool IncludeAttributeAssembly;
    public bool? ProcessFields;
    public bool? TryToWeaveAllTypes;
    public bool? CheckForIsChanged;
    XDocument xDocument;
    public string ToolsDirectory;
    public string Target;
    public string ProjectFile;

    public void Execute()
    {
        new FileInfo(ProjectFile).IsReadOnly = false;
        xDocument = XDocument.Load(ProjectFile);
        TryInjectReference();
        InjectUsingTask();
        InjectWeavingTask();
        xDocument.Save(ProjectFile);
    }


    void InjectWeavingTask()
    {
        var buildDescendants = xDocument.BuildDescendants("NotifyPropertyWeaverMsBuildTask.WeavingTask").ToList();
        if (buildDescendants != null)
        {
            foreach (var element in buildDescendants)
            {
                element.Remove();
            }
        }

        var target = GetOrCreateTarget(Target);

        var xElement = BuildWeavingTaskElement();
        target.Add(xElement);
    }

    XElement GetOrCreateTarget(string target)
    {
        var targetElement = xDocument.BuildDescendants("Target")
            .FirstOrDefault(x => string.Equals((string) x.Attribute("Name"), target, StringComparison.OrdinalIgnoreCase));
        if (targetElement == null)
        {
            targetElement = new XElement(XDocumentExtensions.BuildNamespace + "Target", new XAttribute("Name", target));
            xDocument.Root.Add(targetElement);
        }
        return targetElement;
    }

    XElement BuildWeavingTaskElement()
    {
        var xAttributes = new List<XAttribute>();
        if (TryToWeaveAllTypes != null)
        {
            xAttributes.Add(new XAttribute("TryToWeaveAllTypes", TryToWeaveAllTypes));
        }
        if (!string.IsNullOrWhiteSpace(EventInvokerName))
        {
            xAttributes.Add(new XAttribute("EventInvokerName", EventInvokerName));
        }
        if (CheckForEquality != null)
        {
            xAttributes.Add(new XAttribute("CheckForEquality", CheckForEquality));
        }
        if (CheckForIsChanged != null)
        {
            xAttributes.Add(new XAttribute("CheckForIsChanged", CheckForIsChanged));
        }
        if (MessageImportance != null)
        {
            xAttributes.Add(new XAttribute("MessageImportance", MessageImportance));
        }
        if (ProcessFields != null)
        {
            xAttributes.Add(new XAttribute("ProcessFields", ProcessFields));
        }
        if (!string.IsNullOrWhiteSpace(TargetPath))
        {
            xAttributes.Add(new XAttribute("TargetPath", TargetPath));
        }
        return new XElement(XDocumentExtensions.BuildNamespace + "NotifyPropertyWeaverMsBuildTask.WeavingTask", xAttributes.ToArray());
    }


    void InjectUsingTask()
    {
        var count = xDocument.BuildDescendants("UsingTask")
            .Where(x => (string) x.Attribute("TaskName") == "NotifyPropertyWeaverMsBuildTask.WeavingTask").ToList();
        foreach (var xElement in count)
        {
            xElement.Remove();
        }

        xDocument.Root.Add(
            new XElement(XDocumentExtensions.BuildNamespace + "UsingTask",
                         new XAttribute("TaskName", "NotifyPropertyWeaverMsBuildTask.WeavingTask"),
                         new XAttribute("AssemblyFile", Path.Combine(ToolsDirectory, @"NotifyPropertyWeaverMsBuildTask.dll"))));
    }

    void TryInjectReference()
    {
        if (!IncludeAttributeAssembly)
        {
            RemoveReference();
            return;
        }

        SelectReferences().Remove();

        GetReferences().First().Parent.Add(
            new XElement(XDocumentExtensions.BuildNamespace + "Reference",
                         new XAttribute("Include", "NotifyPropertyWeaver"),
                         new XElement(XDocumentExtensions.BuildNamespace + "HintPath", Path.Combine(DependenciesDirectory, @"NotifyPropertyWeaver.dll"))));
    }

    IEnumerable<XElement> SelectReferences()
    {
        return from el in GetReferences()
               where ((string) el.Attribute("Include")).StartsWith("NotifyPropertyWeaver")
               select el;
    }

    IEnumerable<XElement> GetReferences()
    {
        return xDocument.BuildDescendants("Reference");
    }

    void RemoveReference()
    {
        SelectReferences().Remove();
    }
}