using System.Collections.Generic;
using NotifyPropertyWeaverMsBuildTask;
using NotifyPropertyWeaverMsBuildTask.PropertyErrorCheckers;
using NUnit.Framework;

namespace NotifyPropertyWeaverTests.PropertyErrorCheckers
{
    [TestFixture]
    public class AlsoNotifyForPropertyNotExistsErrorCheckerTest
    {

        [Test]
        public void PropertyThatDoesExists()
        {

            var checker = new AlsoNotifyForPropertyNotExistsErrorChecker();

            var propertyDefinition = DefinitionFinder.FindProperty(() => Property);

            var message = checker.CheckForError(new PropertyData
                                                    {
                                                        PropertyDefinition = propertyDefinition,
                                                        PropertyNamesToNotify = new List<string> { "Property" }
                                                    });
            Assert.IsNull(message);
        }

        [Test]
        public void PropertyThatDoesNotExist()
        {

            var checker = new AlsoNotifyForPropertyNotExistsErrorChecker();

            var propertyDefinition = DefinitionFinder.FindProperty(() => Property);

            var message = checker.CheckForError(new PropertyData
                                                    {
                                                        PropertyDefinition = propertyDefinition,
                                                        PropertyNamesToNotify = new List<string> { "PropertyX"}
                                                    });
            Assert.IsNotNull(message);
        }


        public int Property { get; set; }

    }
}