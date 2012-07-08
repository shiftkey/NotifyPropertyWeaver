using System.ComponentModel.Composition;
using System.Linq;
using Mono.Cecil;
using NotifyPropertyWeaverMsBuildTask;


[Export, PartCreationPolicy(CreationPolicy.Shared)]
public class FileChangedChecker
{
    Logger logger;
    ModuleReader moduleReader;
    string namespaceKey;

    [ImportingConstructor]
    public FileChangedChecker(Logger logger, WeavingTask config, ModuleReader moduleReader)
    {
        namespaceKey = config.GetType().Namespace.Replace(".", string.Empty);
        this.logger = logger;
        this.moduleReader = moduleReader;
    }

    public bool ShouldStart()
    {
        if (moduleReader.Module.Types.Any(x => x.Name == namespaceKey))
        {
            logger.LogMessage("\tDid not process because file has already been processed");
            return false;
        }
        moduleReader.Module.Types.Add(new TypeDefinition(null, namespaceKey, TypeAttributes.NotPublic | TypeAttributes.Abstract | TypeAttributes.Interface));
        return true;
    }
}