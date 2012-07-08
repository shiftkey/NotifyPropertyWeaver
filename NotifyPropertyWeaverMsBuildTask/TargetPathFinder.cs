using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using NotifyPropertyWeaverMsBuildTask;

[Export, PartCreationPolicy(CreationPolicy.Shared)]
public class TargetPathFinder
{
    WeavingTask config;
    BuildEnginePropertyExtractor buildEnginePropertyExtractor;
    Logger logger;

    //For Testing
    public TargetPathFinder()
    {
    }

    [ImportingConstructor]
    public TargetPathFinder(WeavingTask config, BuildEnginePropertyExtractor buildEnginePropertyExtractor, Logger logger)
    {
        this.config = config;
        this.buildEnginePropertyExtractor = buildEnginePropertyExtractor;
        this.logger = logger;
    }

    public string GetBuildEngineKey()
    {
        var projectFilePath = buildEnginePropertyExtractor.GetProjectPath();
        var xDocument = XDocument.Load(projectFilePath);
        var weavingTaskName = config.GetType().Assembly.GetName().Name + ".WeavingTask";
        var weavingTaskNode = xDocument.BuildDescendants(weavingTaskName).First();
        var xAttribute = weavingTaskNode.Parent.Attribute("Name");
        if (xAttribute == null)
        {
            throw new Exception("Target node contains no 'Name' attribute.");
        }
        ;
        var targetNodeName = xAttribute.Value.ToUpperInvariant();
        switch (targetNodeName)
        {
            case ("AFTERCOMPILE"):
                {
                    logger.LogMessage("\tTarget node is 'AfterCompile' so using 'IntermediateAssembly'.");
                    return "IntermediateAssembly";
                }
            case ("AFTERBUILD"):
                {
                    logger.LogMessage("\tTarget node is 'AfterBuild' so using 'TargetPath'.");
                    return "TargetPath";
                }
        }
        throw new WeavingException(
            string.Format(
                @"Failed to derive TargetPath from target node. WeavingTask is located in '{0}'. 
Target path can only be derived when WeavingTask is located in 'AfterCompile' or 'AfterBuild'.
Please define 'TargetPath' as follows: 
<WeavingTask ... TargetPath=""PathToYourAssembly"" />", targetNodeName));
    }


    public void Execute()
    {
        if (string.IsNullOrWhiteSpace(config.TargetPath))
        {
            logger.LogMessage("\tYou did not define the WeavingTask.TargetPath. So it was extracted from the BuildEngine.");
            var buildEngineKey = GetBuildEngineKey();
            try
            {
                config.TargetPath = buildEnginePropertyExtractor.GetEnvironmentVariable(buildEngineKey, true).First();
            }
            catch (Exception exception)
            {
                throw new WeavingException(string.Format(
                    @"Failed to extract target assembly path from the BuildEngine. 
Please raise a bug with the below exception text.
The temporary work-around is to change the weaving task as follows 
<WeavingTask ... TargetPath=""PathToYourAssembly"" />
Exception details: {0}", exception));
            }
        }
        if (!File.Exists(config.TargetPath))
        {
            throw new WeavingException(string.Format("TargetPath \"{0}\" does not exists. If you have not done a build you can ignore this error.", config.TargetPath));
        }
    }
}