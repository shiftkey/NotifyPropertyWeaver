using NUnit.Framework;

[TestFixture]
public class CheckForEqualityWithNoBackingFieldSetErrorCheckerTest
{

    [Test]
    public void WithBackingField()
    {
        var checker = new ErrorChecker(null, null);
        var propertyDefinition = DefinitionFinder.FindProperty(() => WithBackingFieldProperty);

        var message = checker.CheckForErrors(new PropertyData
                                                {
                                                    PropertyDefinition = propertyDefinition,
                                                    NotificationAddedDirectly = true,
                                                    CheckForEquality = true,
                                                    BackingFieldReference = propertyDefinition.DeclaringType.Fields[0],
                                                }, false);
        Assert.IsNull(message);
    }

    [Test]
    public void WithoutBackingField()
    {
        var checker = new ErrorChecker(null, null);

        var propertyDefinition = DefinitionFinder.FindProperty<CheckForEqualityWithNoBackingFieldSetErrorCheckerTest>("WithoutBackingFieldProperty");

        var message = checker.CheckForErrors(new PropertyData
                                                {
                                                    PropertyDefinition = propertyDefinition,
                                                    NotificationAddedDirectly = true,
                                                    CheckForEquality = true,
                                                    BackingFieldReference = null,

                                                }, false);
        Assert.IsNotNull(message);
    }




    public int WithBackingFieldProperty { get; set; }

    public int WithoutBackingFieldProperty
    {
        set
        {
        }
    }

}