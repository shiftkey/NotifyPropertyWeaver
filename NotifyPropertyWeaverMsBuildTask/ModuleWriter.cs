using System.ComponentModel.Composition;
using System.IO;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Mdb;
using Mono.Cecil.Pdb;
using NotifyPropertyWeaverMsBuildTask;

[Export, PartCreationPolicy(CreationPolicy.Shared)]
public class ModuleWriter
{
    ModuleReader moduleReader;
    ProjectKeyReader projectKeyReader;
    Logger logger;
    WeavingTask config;

    [ImportingConstructor]
    public ModuleWriter(ModuleReader moduleReader, ProjectKeyReader projectKeyReader, Logger logger, WeavingTask config)
    {
        this.moduleReader = moduleReader;
        this.projectKeyReader = projectKeyReader;
        this.logger = logger;
        this.config = config;
    }

    static ISymbolWriterProvider GetSymbolWriterProvider(string targetPath)
    {
        var pdbPath = Path.ChangeExtension(targetPath, "pdb");
        if (File.Exists(pdbPath))
        {
            return new PdbWriterProvider();
        }
        var mdbPath = Path.ChangeExtension(targetPath, "mdb");

        if (File.Exists(mdbPath))
        {
            return new MdbWriterProvider();
        }
        return null;
    }

    public void Execute()
    {
        Execute(config.TargetPath);
    }

    public void Execute(string targetPath)
    {
        if (projectKeyReader.StrongNameKeyPair == null)
        {
            logger.LogMessage(string.Format("\tSaving assembly to '{0}'.", targetPath));
        }
        else
        {
            logger.LogMessage(string.Format("\tSigning and saving assembly to '{0}'.", targetPath));
        }
        var parameters = new WriterParameters
                             {
                                 StrongNameKeyPair = projectKeyReader.StrongNameKeyPair,
                                 WriteSymbols = true,
                                 SymbolWriterProvider = GetSymbolWriterProvider(config.TargetPath)
                             };
        moduleReader.Module.Write(targetPath, parameters);
    }
}