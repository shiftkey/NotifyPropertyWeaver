using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;

//http://blogs.realdolmen.com/experts/2010/09/07/some-clarity-on-auto-loading-visual-studio-2010-extensions/
[ProvideAutoLoad("F1536EF8-92EC-443C-9ED7-FDADF150DA82")] //SolutionExists
[ProvideAutoLoad("ADFC4E64-0397-11D1-9F4E-00A0C911004F ")] //NoSolution
[ProvideAutoLoad("4d7a79c7-e2e3-4140-93cc-f0e68a6cae56")]
[PackageRegistration(UseManagedResourcesOnly = true)]
[InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
[ProvideMenuResource("Menus.ctmenu", 1)]
[Guid("d7cb41bc-0cfc-4747-9399-4b5ed132c898")]
public sealed class NotifyPropertyWeaverVsPackagePackage : Package
{

    protected override void Initialize()
    {
        base.Initialize();

        var exceptionDialog = new ExceptionDialog("http://code.google.com/p/notifypropertyweaver/issues/list", "NotifyPropertyWeaver");
        try
        {
            using (var catalog = new AssemblyCatalog(GetType().Assembly))
            using (var container = new CompositionContainer(catalog))
            {
                var menuCommandService = (IMenuCommandService) GetService(typeof (IMenuCommandService));
                var errorListProvider = new ErrorListProvider(ServiceProvider.GlobalProvider);

                container.ComposeExportedValue(exceptionDialog);
                container.ComposeExportedValue(menuCommandService);
                container.ComposeExportedValue(errorListProvider);

                container.GetExportedValue<MenuConfigure>().RegisterMenus();
                container.GetExportedValue<SolutionEvents>().RegisterSolutionEvents();
                container.GetExportedValue<TaskFileReplacer>().CheckForFilesToUpdate();
            }
        }
        catch (Exception exception)
        {
            exceptionDialog.HandleException(exception);
        }
    }
}