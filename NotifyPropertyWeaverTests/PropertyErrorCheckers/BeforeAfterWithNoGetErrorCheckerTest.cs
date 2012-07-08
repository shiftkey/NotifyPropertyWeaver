using NUnit.Framework;

[TestFixture]
public class BeforeAfterWithNoGetErrorCheckerTest
{

    [Test]
    public void WithGet()
    {
        var checker = new ErrorChecker(null, null);

        var propertyDefinition = DefinitionFinder.FindProperty<BeforeAfterWithNoGetErrorCheckerTest>("PropertyWithGet");

        var message = checker.CheckForErrors(new PropertyData
                                                {
                                                    PropertyDefinition = propertyDefinition,
                                                    NotificationAddedDirectly = true,
                                                }, true);
        Assert.IsNull(message);
    }

    [Test]
    public void NoGet()
    {
        var checker = new ErrorChecker(null, null);

        var propertyDefinition = DefinitionFinder.FindProperty<BeforeAfterWithNoGetErrorCheckerTest>("PropertyNoGet");

        var message = checker.CheckForErrors(new PropertyData
                                                {
                                                    PropertyDefinition = propertyDefinition,
                                                    NotificationAddedDirectly = true,
                                                }, true);
        Assert.IsNotNull(message);
    }




    string property;

    [NotifyProperty]
    public string PropertyNoGet
    {
        set { property = value; }
    }
    [NotifyProperty]
    public string PropertyWithGet
    {
        set { property = value; }
        get { return property; }
    }

}