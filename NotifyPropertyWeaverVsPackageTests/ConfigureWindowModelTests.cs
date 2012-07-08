using System;
using System.Diagnostics;
using NUnit.Framework;

[TestFixture]
public class ConfigureWindowModelTests
{


    [Test]
    public void ValidateDependenciesDirectory()
    {
        var model = new ConfigureWindowModel
                        {
                            IncludeAttributeAssembly = true,
                            DependenciesDirectory = null,
                            DeriveTargetPathFromBuildEngine = true,
                            EventInvokerName = "foo",
                            ToolsDirectory = "foo",
                            TargetNode = "AfterCompile",
                        };
        Assert.IsNotNullOrEmpty(model.GetErrors());
        model.DependenciesDirectory = string.Empty;
        Assert.IsNotNullOrEmpty(model.GetErrors());
        model.DependenciesDirectory = "a";
        Assert.IsNull(model.GetErrors());
    }

    [Test]
    public void ValidateEventInvokerName()
    {
        Debug.WriteLine(Guid.NewGuid().ToString());
        var model = new ConfigureWindowModel
        {
            EventInvokerName = null,
            DeriveTargetPathFromBuildEngine = true,
            ToolsDirectory = "foo",
            TargetNode = "AfterCompile",
        };
        Assert.IsNotNullOrEmpty(model.GetErrors());
        model.EventInvokerName = string.Empty;
        Assert.IsNotNullOrEmpty(model.GetErrors());
        model.EventInvokerName = "a";
        Assert.IsNull(model.GetErrors());
    }

    [Test]
    public void ValidateTargetPath()
    {
        var model = new ConfigureWindowModel
                        {
                            TargetPath = null,
                            DeriveTargetPathFromBuildEngine = false,
                            EventInvokerName = "foo",
                            ToolsDirectory = "foo,",
                            TargetNode = "AfterCompile",
                        };
        Assert.IsNotNullOrEmpty(model.GetErrors());
        model.TargetPath = string.Empty;
        Assert.IsNotNullOrEmpty(model.GetErrors());
        model.TargetPath = "a";
        Assert.IsNull(model.GetErrors());

    }
    [Test]
    public void ValidateTargetNode()
    {
        var model = new ConfigureWindowModel
                        {
                            DeriveTargetPathFromBuildEngine = true,
                            EventInvokerName = "foo",
                            ToolsDirectory = "foo,",
                        };
        Assert.IsNotNullOrEmpty(model.GetErrors());
        model.TargetNode = string.Empty;
        Assert.IsNotNullOrEmpty(model.GetErrors());
        model.TargetNode = "AfterCompile";
        Assert.IsNull(model.GetErrors());

    }
}