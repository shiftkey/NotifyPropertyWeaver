using Microsoft.Build.Framework;
using NUnit.Framework;

[TestFixture]
public class ConfigureWindowTests
{

    [Test]
    [Ignore]
    public void LaunchEmpty()
    {
        var runner = new CrossThreadRunner();
        runner.RunInSta(() =>
                            {
                                var configureWindow = new ConfigureWindow(new ConfigureWindowModel());
                                configureWindow.ShowDialog();
                            });

    }

    [Test]
    [Ignore]
    public void LaunchExisting()
    {
        var runner = new CrossThreadRunner();
        runner.RunInSta(() =>
                            {
                                var configureWindow = new ConfigureWindow
                                    (new ConfigureWindowModel
                                         {
                                             EventInvokerName = "Foo",
                                             DependenciesDirectory = "bar",
                                             MessageImportance = MessageImportance.High,
                                         }
                                    );
                                configureWindow.ShowDialog();
                            });


    }
}