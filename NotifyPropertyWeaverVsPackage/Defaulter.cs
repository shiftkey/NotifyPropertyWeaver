using Microsoft.Build.Framework;

public class Defaulter
{
    public void ToModel(ProjectReader projectReader, ConfigureWindowModel configureWindowModel)
    {
        configureWindowModel.CheckForEquality = projectReader.CheckForEquality.GetValueOrDefault(true);
        configureWindowModel.CheckForIsChanged = projectReader.CheckForIsChanged.GetValueOrDefault(false);
        configureWindowModel.TryToWeaveAllTypes = projectReader.TryToWeaveAllTypes.GetValueOrDefault(true);
        configureWindowModel.ProcessFields = projectReader.ProcessFields.GetValueOrDefault(false);
        configureWindowModel.MessageImportance = projectReader.MessageImportance.GetValueOrDefault(MessageImportance.Low);
        if (projectReader.TargetNode == null)
        {
            configureWindowModel.TargetNode = "AfterCompile";
        }
        else
        {
            configureWindowModel.TargetNode = projectReader.TargetNode;
        }
        configureWindowModel.TargetPath = projectReader.TargetPath;
        configureWindowModel.DeriveTargetPathFromBuildEngine = projectReader.TargetPath == null;
        configureWindowModel.DependenciesDirectory = GetValueOrDefault(projectReader.DependenciesDirectory, @"$(SolutionDir)Lib\");
        configureWindowModel.ToolsDirectory = GetValueOrDefault(projectReader.ToolsDirectory, @"$(SolutionDir)Tools\");
        configureWindowModel.EventInvokerName = GetValueOrDefault(projectReader.EventInvokerName, "OnPropertyChanged");
        configureWindowModel.IncludeAttributeAssembly = !string.IsNullOrWhiteSpace(projectReader.DependenciesDirectory);
    }

    public static string GetValueOrDefault(string str, string defaultValue)
    {
        if (string.IsNullOrWhiteSpace(str))
        {
            return defaultValue;
        }
        return str;
    }

    public void FromModel(NotifyPropertyWeaverProjectInjector projectInjector, ConfigureWindowModel configureWindowModel)
    {
        if (!configureWindowModel.CheckForEquality)
        {
            projectInjector.CheckForEquality = false;
        }
        if (configureWindowModel.ProcessFields)
        {
            projectInjector.ProcessFields = true;
        }
        if (configureWindowModel.CheckForIsChanged)
        {
            projectInjector.CheckForIsChanged = true;
        }
        projectInjector.IncludeAttributeAssembly = configureWindowModel.IncludeAttributeAssembly;

        if (!configureWindowModel.DeriveTargetPathFromBuildEngine)
        {
            projectInjector.TargetPath = configureWindowModel.TargetPath;
        }


        if (!configureWindowModel.TryToWeaveAllTypes)
        {
            projectInjector.TryToWeaveAllTypes = false;
        }
        if (configureWindowModel.MessageImportance != MessageImportance.Low)
        {
            projectInjector.MessageImportance = configureWindowModel.MessageImportance;
        }
        projectInjector.Target = configureWindowModel.TargetNode;
        projectInjector.ToolsDirectory = configureWindowModel.ToolsDirectory;

        projectInjector.DependenciesDirectory = configureWindowModel.DependenciesDirectory;
        if (configureWindowModel.EventInvokerName != "OnPropertyChanged")
        {
            projectInjector.EventInvokerName = configureWindowModel.EventInvokerName;
        }
    }
}