using System.Linq;
using NUnit.Framework;

[TestFixture]
public class IndexerErrorCheckerTest
{

    [Test]
    public void IsAbstract()
    {
        var checker = new ErrorChecker(null, null);
        var propertyDefinition = DefinitionFinder.FindType<IndexerClass>().Properties.First();

        var message = checker.CheckForErrors(new PropertyData
                                                              {
                                                                  PropertyDefinition = propertyDefinition,
                                                                  NotificationAddedDirectly = true
                                                              }, false);
        Assert.IsNotNull(message);
    }



    public abstract class IndexerClass
    {
        public string this[string i]
        {
            get
            {
                return null;
            }
            set
            {
            }
        }
    }

}