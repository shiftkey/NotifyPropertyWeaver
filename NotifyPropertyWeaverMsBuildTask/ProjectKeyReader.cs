using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using NotifyPropertyWeaverMsBuildTask;

[Export, PartCreationPolicy(CreationPolicy.Shared)]
public class ProjectKeyReader
{
    BuildEnginePropertyExtractor buildEnginePropertyExtractor;
    WeavingTask config;

    [ImportingConstructor]
    public ProjectKeyReader(BuildEnginePropertyExtractor buildEnginePropertyExtractor, WeavingTask config)
    {
        this.buildEnginePropertyExtractor = buildEnginePropertyExtractor;
        this.config = config;
    }

    public StrongNameKeyPair StrongNameKeyPair { get; set; }

    static string GetKeyFile(string projectPath)
    {
        var xDocument = XDocument.Load(projectPath);
        return (
                   from c in xDocument.BuildDescendants("AssemblyOriginatorKeyFile")
                   select c.Value)
            .FirstOrDefault();
    }

    static bool IsSignAssemblyTrue(string projectPath)
    {
        var xDocument = XDocument.Load(projectPath);
        var signAssembly = (
                               from c in xDocument.BuildDescendants("SignAssembly")
                               select c.Value)
            .FirstOrDefault();
        return string.Equals(signAssembly, "true", StringComparison.OrdinalIgnoreCase);
    }

    public void Execute()
    {
        if (config.KeyFilePath == null)
        {
            var projectFilePath = buildEnginePropertyExtractor.GetProjectPath();
            if (!IsSignAssemblyTrue(projectFilePath))
            {
                return;
            }

            var assemblyOriginatorKeyFile = GetKeyFile(projectFilePath);
            if (assemblyOriginatorKeyFile == null)
            {
                return;
            }
            config.KeyFilePath = Path.Combine(Path.GetDirectoryName(projectFilePath), assemblyOriginatorKeyFile);
        }

        StrongNameKeyPair = new StrongNameKeyPair(File.OpenRead(config.KeyFilePath));
    }
}