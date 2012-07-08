using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using Microsoft.Build.Framework;
using Mono.Cecil;
using NSubstitute;
using NotifyPropertyWeaverMsBuildTask;


public class WeaverHelper
{
    string projectPath;
    string assemblyPath;
    public Assembly Assembly { get; set; }

    public WeaverHelper(string projectPath, bool tryToWeaveAllTypes)
    {
        this.projectPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\TestAssemblies", projectPath));

        GetAssemblyPath();


        var newAssembly = assemblyPath.Replace(".dll", "2.dll");
        var pdbFileName = Path.ChangeExtension(assemblyPath, "pdb");
        var newPdbFileName = Path.ChangeExtension(newAssembly, "pdb");
        File.Copy(assemblyPath, newAssembly, true);
        File.Copy(pdbFileName, newPdbFileName, true);


        var buildEngine = Substitute.For<IBuildEngine>();
        buildEngine.ProjectFileOfTaskNode.Returns(projectPath);
        var buildEngineExtensions = Substitute.For<BuildEnginePropertyExtractor>();
        buildEngineExtensions.GetEnvironmentVariable("_DebugSymbolsIntermediatePath", false).Returns(new[] { newPdbFileName });
        buildEngineExtensions.GetProjectPath().Returns(this.projectPath);

        var weavingTask = new WeavingTask
                              {
                                  TargetPath = newAssembly,
                                  BuildEngine = buildEngine,
                                  TryToWeaveAllTypes = tryToWeaveAllTypes,
                                  References = GetReferences(),
                                  CheckForIsChanged = true,
                                  ProcessFields = true,
                                  BuildEnginePropertyExtractor = buildEngineExtensions,
                                  EventInvokerName = "CustomEventInvoker"
                              };

        var execute = weavingTask.Execute();
        if (!execute)
        {
            throw weavingTask.Exception;
        }

        var combine = Path.Combine(new FileInfo(assemblyPath).DirectoryName, "NotifyPropertyWeaver.dll");
        if (File.Exists(combine))
        {
            Assembly.LoadFile(Path.GetFullPath(combine));
        }
        Assembly = Assembly.LoadFile(newAssembly);
    }

    void GetAssemblyPath()
    {
        assemblyPath = Path.Combine(Path.GetDirectoryName(projectPath), GetOutputPathValue(), GetAssemblyName() + ".dll");
    }

    string GetAssemblyName()
    {
        return XDocument.Load(projectPath)
            .BuildDescendants("AssemblyName")
            .Select(x => x.Value)
            .First();
    }

    string GetOutputPathValue()
    {
        var xDocument = XDocument.Load(projectPath);

        var outputPathValue = (from propertyGroup in xDocument.BuildDescendants("PropertyGroup")
                               let condition = ((string)propertyGroup.Attribute("Condition"))
                               where (condition != null) &&
                                     (condition.Trim() == "'$(Configuration)|$(Platform)' == 'Debug|AnyCPU'")
                               from outputPath in propertyGroup.BuildDescendants("OutputPath")
                               select outputPath.Value).First();
#if (!DEBUG)
            outputPathValue = outputPathValue.Replace("Debug", "Release");
#endif
        return outputPathValue;
    }

    string GetReferences()
    {
        var referenceFinder = new ReferenceFinder(assemblyPath, projectPath);
        var builder = new StringBuilder();

        var assemblyNameReferences = ModuleDefinition.ReadModule(assemblyPath).AssemblyReferences;
        foreach (var assemblyNameReference in assemblyNameReferences)
        {
            builder.Append(referenceFinder.Resolve(assemblyNameReference));
            builder.Append(";");
        }
        builder.Append(referenceFinder.Resolve("System"));
        builder.Append(";");
        builder.Append(referenceFinder.Resolve("System.Core"));
        builder.Append(";");
        return builder.ToString();
    }
}