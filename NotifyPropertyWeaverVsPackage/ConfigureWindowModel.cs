using System.ComponentModel;
using System.Text;
using Microsoft.Build.Framework;

public class ConfigureWindowModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    string dependenciesDirectory;

    public string DependenciesDirectory
    {
        get { return dependenciesDirectory; }
        set
        {
            dependenciesDirectory = value;
            OnPropertyChanged("DependenciesDirectory");
        }
    }

    string toolsDirectory;

    public string ToolsDirectory
    {
        get { return toolsDirectory; }
        set
        {
            toolsDirectory = value;
            OnPropertyChanged("ToolsDirectory");
        }
    }

    string targetPath;

    public string TargetPath
    {
        get { return targetPath; }
        set
        {
            targetPath = value;
            OnPropertyChanged("TargetPath");
        }
    }

    string eventInvokerName;

    public string EventInvokerName
    {
        get { return eventInvokerName; }
        set
        {
            eventInvokerName = value;
            OnPropertyChanged("EventInvokerName");
        }
    }

    bool processFields;

    public bool ProcessFields
    {
        get { return processFields; }
        set
        {
            processFields = value;
            OnPropertyChanged("ProcessFields");
        }
    }

    bool deriveTargetPathFromBuildEngine;

    public bool DeriveTargetPathFromBuildEngine
    {
        get { return deriveTargetPathFromBuildEngine; }
        set
        {
            deriveTargetPathFromBuildEngine = value;
            OnPropertyChanged("DeriveTargetPathFromBuildEngine");
        }
    }

    bool checkForEquality;

    public bool CheckForEquality
    {
        get { return checkForEquality; }
        set
        {
            checkForEquality = value;
            OnPropertyChanged("CheckForEquality");
        }
    }

    bool checkForIsChanged;

    public bool CheckForIsChanged
    {
        get { return checkForIsChanged; }
        set
        {
            checkForIsChanged = value;
            OnPropertyChanged("CheckForIsChanged");
        }
    }

    bool tryToWeaveAllTypes;

    public bool TryToWeaveAllTypes
    {
        get { return tryToWeaveAllTypes; }
        set
        {
            tryToWeaveAllTypes = value;
            OnPropertyChanged("TryToWeaveAllTypes");
        }
    }

    MessageImportance messageImportance;

    public MessageImportance MessageImportance
    {
        get { return messageImportance; }
        set
        {
            messageImportance = value;
            OnPropertyChanged("MessageImportance");
        }
    }

    string targetNode;

    public string TargetNode
    {
        get { return targetNode; }
        set
        {
            targetNode = value;
            OnPropertyChanged("TargetNode");
        }
    }

    bool includeAttributeAssembly;

    public bool IncludeAttributeAssembly
    {
        get { return includeAttributeAssembly; }
        set
        {
            includeAttributeAssembly = value;
            OnPropertyChanged("IncludeAttributeAssembly");
        }
    }


    string version;

    public string Version
    {
        get { return version; }
        set
        {
            version = value;
            OnPropertyChanged("Version");
        }
    }

    public ConfigureWindowModel()
    {
        Version = CurrentVersion.Version.ToString();
    }

    void OnPropertyChanged(string propertyName)
    {
        var handler = PropertyChanged;
        if (handler != null)
        {
            handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public string GetErrors()
    {
        var stringBuilder = new StringBuilder();
        if (IncludeAttributeAssembly)
        {
            if (string.IsNullOrWhiteSpace(DependenciesDirectory))
            {
                stringBuilder.AppendLine("DependenciesDirectory is required if you have selected IncludeAttributeAssembly.");
            }
        }
        if (!DeriveTargetPathFromBuildEngine)
        {
            if (string.IsNullOrWhiteSpace(TargetPath))
            {
                stringBuilder.AppendLine("TargetPath is required if you have selected DeriveTargetPathFromBuildEngine.");
            }
        }
        if (string.IsNullOrWhiteSpace(EventInvokerName))
        {
            stringBuilder.AppendLine("EventInvokerName is required.");
        }
        if (string.IsNullOrWhiteSpace(ToolsDirectory))
        {
            stringBuilder.AppendLine("ToolsDirectory is required.");
        }
        if (string.IsNullOrWhiteSpace(TargetNode))
        {
            stringBuilder.AppendLine("TargetNode is required.");
        }
        if (stringBuilder.Length == 0)
        {
            return null;
        }
        return stringBuilder.ToString();
    }
}