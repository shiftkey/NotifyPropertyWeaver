using System.ComponentModel.Composition;
using System.Linq;

[Export, PartCreationPolicy(CreationPolicy.Shared)]
public class ReferenceCleaner
{
    ModuleReader moduleReader;
    Logger logger;

    [ImportingConstructor]
    public ReferenceCleaner(ModuleReader moduleReader, Logger logger)
    {
        this.moduleReader = moduleReader;
        this.logger = logger;
    }

    public void Execute()
    {
        var referenceToRemove = moduleReader.Module.AssemblyReferences.FirstOrDefault(x => x.Name == "NotifyPropertyWeaver");
        if (referenceToRemove == null)
        {
            logger.LogMessage("\tNo reference to NotifyPropertyWeaver found. References not modified.");
            return;
        }

        moduleReader.Module.AssemblyReferences.Remove(referenceToRemove);
        logger.LogMessage("\tRemoving reference to NotifyPropertyWeaver.");
    }
}