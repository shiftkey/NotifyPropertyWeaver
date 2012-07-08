using System.Linq;
using NUnit.Framework;

[TestFixture]
public class AbstractErrorCheckerTest
{

    [Test]
    public void IsAbstract()
    {
        var checker = new ErrorChecker(null, null);
        var propertyDefinition = DefinitionFinder.FindType<BaseClass>().Properties.First(x => x.Name == "AbstractProperty");

        var message = checker.CheckForErrors(new PropertyData
                                                              {
                                                                  PropertyDefinition = propertyDefinition,
                                                                  NotificationAddedDirectly = true
                                                              }, false);
        Assert.IsNotNull(message);
    }

    [Test]
    public void NonAbstract()
    {


        var checker = new ErrorChecker(null, null);
        var propertyDefinition = DefinitionFinder.FindType<BaseClass>().Properties.First(x => x.Name == "NonAbstractProperty");

        var message = checker.CheckForErrors(new PropertyData
                                                              {
                                                                  PropertyDefinition = propertyDefinition,
                                                                  NotificationAddedDirectly = false
                                                              }, false);
        Assert.IsNull(message);
    }

    public abstract class BaseClass
    {
        public abstract int AbstractProperty { get; set; }
        public int NonAbstractProperty { get; set; }

    }

}