using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace NotifyPropertyWeaverMsBuildTask
{

    public class WeavingTask : Task
    {
        public string TargetPath { set; get; }
        public string EventInvokerName { set; get; }
        public bool CheckForEquality { set; get; }
        public bool CheckForIsChanged { set; get; }
        public bool TryToWeaveAllTypes { set; get; }
        public string MessageImportance { set; get; }
        public string References { get; set; }
        public string KeyFilePath { get; set; }
        public Exception Exception { get; set; }
        public bool ProcessFields { get; set; }
        public BuildEnginePropertyExtractor BuildEnginePropertyExtractor;
        Logger logger;
        static AssemblyCatalog assemblyCatalog;

        static WeavingTask()
        {
            assemblyCatalog = new AssemblyCatalog(typeof(WeavingTask).Assembly);
        }

        public WeavingTask()
        {
            CheckForEquality = true;
            TryToWeaveAllTypes = true;
            MessageImportance = "Low";
        }

        public override bool Execute()
        {
            BuildEngine.LogMessageEvent(new BuildMessageEventArgs(string.Format("NotifyPropertyWeaver (version {0}) Executing (Change MessageImportance to get more or less info)", GetType().Assembly.GetName().Version), "", "NotifyPropertyWeaver", Microsoft.Build.Framework.MessageImportance.High));

            var stopwatch = Stopwatch.StartNew();

            try
            {
                logger = new Logger
                             {
                                 SenderName = "NotifyPropertyWeaver",
                                 BuildEngine = BuildEngine,
                             };
                logger.Initialise(MessageImportance);
                Inner();
            }
            catch (Exception exception)
            {
                HandleException(exception);
            }
            finally 
            {
                stopwatch.Stop();
                logger.Flush();
                BuildEngine.LogMessageEvent(new BuildMessageEventArgs(string.Format("\tFinished ({0}ms)", stopwatch.ElapsedMilliseconds), "", "NotifyPropertyWeaver", Microsoft.Build.Framework.MessageImportance.High));
            }
            return !logger.ErrorHasBeenRaised;
        }

        void HandleException(Exception exception)
        {
            Exception = exception;
            if (exception is WeavingException)
            {
                logger.LogError(exception.Message);
                return;
            }
            
            logger.LogError(string.Format("Unhandled exception occurred {0}", exception));
        }


        void Inner()
        {
            using (var container = new CompositionContainer(assemblyCatalog))
            {
                container.ComposeExportedValue(this);
                container.ComposeExportedValue(BuildEngine);
                container.ComposeExportedValue(logger);
                if (BuildEnginePropertyExtractor != null)
                {
                    container.ComposeExportedValue(BuildEnginePropertyExtractor);
                }
                //TODO: container.GetExportedValue<BuildEngineExtensions>().Execute();
                container.GetExportedValue<TargetPathFinder>().Execute();

                logger.LogMessage(string.Format(@"	TargetPath: {0}
	TryToWeaveAllTypes: {1}
	CheckForEquality: {2}
	EventInvokerName: {3}
	CheckForIsChanged: {4}
	ProcessFields: {5}", TargetPath, TryToWeaveAllTypes, CheckForEquality, EventInvokerName, CheckForIsChanged, ProcessFields));


                container.GetExportedValue<EventInvokerNameResolver>().Execute();
                container.GetExportedValue<AssemblyResolver>().Execute();
                container.GetExportedValue<ModuleReader>().Execute();
                var fileChangedChecker = container.GetExportedValue<FileChangedChecker>();
                if (!fileChangedChecker.ShouldStart())
                {
                    return;
                }
                container.GetExportedValue<MsCoreReferenceFinder>().Execute();
                container.GetExportedValue<InterceptorFinder>().Execute();
                container.GetExportedValue<TypeNodeBuilder>().Execute();
                container.GetExportedValue<DoNotNotifyTypeCleaner>().Execute();
                container.GetExportedValue<CodeGenTypeCleaner>().Execute();
                container.GetExportedValue<MethodFinder>().Execute();
                if (CheckForIsChanged)
                {
                    container.GetExportedValue<IsChangedMethodFinder>().Execute();
                }
                container.GetExportedValue<AllPropertiesFinder>().Execute();
                if (ProcessFields)
                {
                    container.GetExportedValue<FieldToPropertyConverter>().Execute();
                    container.GetExportedValue<FieldToPropertyForwarder>().Execute();
                }
                container.GetExportedValue<MappingFinder>().Execute();
                container.GetExportedValue<IlGeneratedByDependencyProcessor>().Execute();
                container.GetExportedValue<DependsOnDataAttributeReader>().Execute();
                if (!TryToWeaveAllTypes)
                {
                    container.GetExportedValue<ShouldNotifyForAllWalker>().Execute();
                }
                container.GetExportedValue<PropertyDataWalker>().Execute();
                container.GetExportedValue<ErrorChecker>().Execute();
                container.GetExportedValue<WarningChecker>().Execute();
                container.GetExportedValue<OnChangedWalker>().Execute();
                container.GetExportedValue<StackOverflowChecker>().Execute();
                container.GetExportedValue<TypeProcessor>().Execute();
                container.GetExportedValue<AttributeCleaner>().Execute();
                container.GetExportedValue<ReferenceCleaner>().Execute();
                container.GetExportedValue<ProjectKeyReader>().Execute();
                container.GetExportedValue<ModuleWriter>().Execute();
            }
        }

    }
}

