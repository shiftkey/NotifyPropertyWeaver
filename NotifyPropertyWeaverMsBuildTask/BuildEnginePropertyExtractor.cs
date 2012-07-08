using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;

[Export, PartCreationPolicy(CreationPolicy.Shared)]
public class BuildEnginePropertyExtractor
{
    IBuildEngine buildEngine;

    public BuildEnginePropertyExtractor()
    {
    }

    [ImportingConstructor]
    public BuildEnginePropertyExtractor(IBuildEngine buildEngine)
    {
        this.buildEngine = buildEngine;
    }

    const BindingFlags bindingFlags = BindingFlags.NonPublic |
                                      BindingFlags.FlattenHierarchy |
                                      BindingFlags.Instance |
                                      BindingFlags.Public;

    public virtual ProjectInstance GetProjectInstance()
    {
        var buildEngineType = buildEngine.GetType();
        var callbackField = buildEngineType.GetField("targetBuilderCallback", bindingFlags);
        if (callbackField == null)
        {
            throw new Exception("Could not extract targetBuilderCallback from " + buildEngineType.FullName);
        }
        var callback = callbackField.GetValue(buildEngine);
        var targetCallbackType = callback.GetType();
        var instanceField = targetCallbackType.GetField("projectInstance", bindingFlags);
        if (instanceField == null)
        {
            throw new Exception("Could not extract projectInstance from " + targetCallbackType.FullName);
        }
        return (ProjectInstance) instanceField.GetValue(callback);
    }

    public virtual string GetProjectPath()
    {
        var projectFilePath = buildEngine.ProjectFileOfTaskNode;
        if (File.Exists(projectFilePath))
        {
            return projectFilePath;
        }
        return GetProjectInstance().FullPath;
    }

    public virtual IEnumerable<string> GetEnvironmentVariable(string key, bool throwIfNotFound)
    {
        var projectInstance = GetProjectInstance();

        var items = projectInstance.Items
            .Where(x => string.Equals(x.ItemType, key, StringComparison.OrdinalIgnoreCase)).ToList();
        if (items.Count > 0)
        {
            return items.Select(x => x.EvaluatedInclude);
        }


        var properties = projectInstance.Properties
            .Where(x => string.Equals(x.Name, key, StringComparison.OrdinalIgnoreCase)).ToList();
        if (properties.Count > 0)
        {
            return properties.Select(x => x.EvaluatedValue);
        }

        if (throwIfNotFound)
        {
            throw new Exception(string.Format("Could not extract from '{0}' environmental variables.", key));
        }

        return Enumerable.Empty<string>();
    }
}