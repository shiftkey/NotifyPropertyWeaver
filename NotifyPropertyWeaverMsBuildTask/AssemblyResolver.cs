using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Reflection;
using System.Linq;
using Mono.Cecil;
using NotifyPropertyWeaverMsBuildTask;

[Export(typeof(AssemblyResolver))]
[Export(typeof(IAssemblyResolver))]
[PartCreationPolicy(CreationPolicy.Shared)]
public class AssemblyResolver : IAssemblyResolver
{
    WeavingTask config;
    Logger logger;
    BuildEnginePropertyExtractor buildEnginePropertyExtractor;
    Dictionary<string, string> references;
    Dictionary<string, AssemblyDefinition> assemblyDefinitionCache;

    [ImportingConstructor]
    public AssemblyResolver(WeavingTask config, Logger logger, BuildEnginePropertyExtractor buildEnginePropertyExtractor)
    {
        this.config = config;
        this.logger = logger;
        this.buildEnginePropertyExtractor = buildEnginePropertyExtractor;
        assemblyDefinitionCache = new Dictionary<string, AssemblyDefinition>(StringComparer.OrdinalIgnoreCase);
    }

    void SetRefDictionary(IEnumerable<string> filePaths)
    {
        foreach (var filePath in filePaths)
        {
            references[Path.GetFileNameWithoutExtension(filePath)] = filePath;
        }
    }

    AssemblyDefinition GetAssembly(string file, ReaderParameters parameters)
    {
        AssemblyDefinition assemblyDefinition;
        if (assemblyDefinitionCache.TryGetValue(file, out assemblyDefinition))
        {
            return assemblyDefinition;
        }
        if (parameters.AssemblyResolver == null)
        {
            parameters.AssemblyResolver = this;
        }
        try
        {
            assemblyDefinitionCache[file] = assemblyDefinition = ModuleDefinition.ReadModule(file, parameters).Assembly;
            return assemblyDefinition;
        }
        catch (Exception exception)
        {
            throw new Exception(string.Format("Could not read '{0}'.", file), exception);
        }
    }

    public AssemblyDefinition Resolve(AssemblyNameReference assemblyNameReference)
    {
        return Resolve(assemblyNameReference, new ReaderParameters());
    }

    public AssemblyDefinition Resolve(AssemblyNameReference assemblyNameReference, ReaderParameters parameters)
    {
        if (parameters == null)
        {
            parameters = new ReaderParameters();
        }

        string fileFromDerivedReferences;
        if (references.TryGetValue(assemblyNameReference.Name, out fileFromDerivedReferences))
        {
            return GetAssembly(fileFromDerivedReferences, parameters);
        }

        return TryToReadFromDirs(assemblyNameReference, parameters);
    }

    AssemblyDefinition TryToReadFromDirs(AssemblyNameReference assemblyNameReference, ReaderParameters parameters)
    {
        var filesWithMatchingName = SearchDirForMatchingName(assemblyNameReference).ToList();
        foreach (var filePath in filesWithMatchingName)
        {
            var assemblyName = AssemblyName.GetAssemblyName(filePath);
            if (assemblyNameReference.Version == null || assemblyName.Version == assemblyNameReference.Version)
            {
                return GetAssembly(filePath, parameters);
            }
        }
        foreach (var filePath in filesWithMatchingName.OrderByDescending(s => AssemblyName.GetAssemblyName(s).Version))
        {
            return GetAssembly(filePath, parameters);
        }

        var joinedReferences = String.Join(Environment.NewLine, references.Values.OrderBy(x => x));
        throw new Exception(string.Format("Can not find '{0}'.{1}Tried:{1}{2}", assemblyNameReference.FullName, Environment.NewLine, joinedReferences));
    }

    IEnumerable<string> SearchDirForMatchingName(AssemblyNameReference assemblyNameReference)
    {
        var fileName = assemblyNameReference.Name + ".dll";
        return references.Values
            .Select(x => Path.Combine(Path.GetDirectoryName(x), fileName))
            .Where(File.Exists);
    }

    public AssemblyDefinition Resolve(string fullName)
    {
        return Resolve(AssemblyNameReference.Parse(fullName));
    }

    public AssemblyDefinition Resolve(string fullName, ReaderParameters parameters)
    {
        if (fullName == null)
        {
            throw new ArgumentNullException("fullName");
        }

        return Resolve(AssemblyNameReference.Parse(fullName), parameters);
    }

    public void Execute()
    {

        references = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        if (string.IsNullOrWhiteSpace(config.References))
        {
            try
            {
                SetRefDictionary(buildEnginePropertyExtractor.GetEnvironmentVariable("ReferencePath", true));
                logger.LogMessage("\tYou did not define the WeavingTask.References. So references were extracted from the BuildEngine. Reference count=" + references.Count);
            }
            catch (Exception exception)
            {
                throw new WeavingException(string.Format(@"Tried to extract references from the BuildEngine. 
Please raise a bug here http://code.google.com/p/notifypropertyweaver/issues/list with the below exception text.
The temporary work-around is to change the weaving task as follows 
<WeavingTask ... References=""@(ReferencePath)"" />
Exception details: {0}", exception));
            }
        }
        else
        {
            SetRefDictionary(config.References.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries));
            logger.LogMessage("\tUsing WeavingTask.References. Reference count=" + references.Count);
        }
    }
}