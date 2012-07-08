using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using NUnit.Framework;

[TestFixture]
public class NotifyPropertyDataAttributeReaderTests
{

    [Test]
    public void IsChanged()
    {
        var reader = new NotifyPropertyDataAttributeReader(null);

        var typeDefinition = DefinitionFinder.FindType<TestClass>();
        var propertyDefinitions = new List<PropertyDefinition>();
        var trueData = reader.Read(typeDefinition.Properties.First(x => x.Name == "SetIsChangedTrue"), propertyDefinitions);
        Assert.IsTrue(trueData.SetIsChanged.Value);
        var falseData = reader.Read(typeDefinition.Properties.First(x => x.Name == "SetIsChangedFalse"), propertyDefinitions);
        Assert.IsFalse(falseData.SetIsChanged.Value);
        var defauleData = reader.Read(typeDefinition.Properties.First(x => x.Name == "SetIsChangedDefault"), propertyDefinitions);
        Assert.IsNull(defauleData.SetIsChanged);
    }


    [Test]
    public void PerformEqualityCheck()
    {
        var reader = new NotifyPropertyDataAttributeReader(null);

        var typeDefinition = DefinitionFinder.FindType<TestClass>();
        var propertyDefinitions = new List<PropertyDefinition>();
        var trueData = reader.Read(typeDefinition.Properties.First(x => x.Name == "PerformEqualityCheckTrue"), propertyDefinitions);
        Assert.IsTrue(trueData.CheckForEquality.Value);
        var falseData = reader.Read(typeDefinition.Properties.First(x => x.Name == "PerformEqualityCheckFalse"), propertyDefinitions);
        Assert.IsFalse(falseData.CheckForEquality.Value);
        var defauleData = reader.Read(typeDefinition.Properties.First(x => x.Name == "PerformEqualityCheckDefault"), propertyDefinitions);
        Assert.IsNull(defauleData.CheckForEquality);
    }

    public class TestClass
    {

        [NotifyProperty(SetIsChanged = true)]
        public string SetIsChangedTrue { get; set; }

        [NotifyProperty(SetIsChanged = false)]
        public string SetIsChangedFalse { get; set; }

        [NotifyProperty]
        public string SetIsChangedDefault { get; set; }

        [NotifyProperty(PerformEqualityCheck = true)]
        public string PerformEqualityCheckTrue { get; set; }

        [NotifyProperty(PerformEqualityCheck = false)]
        public string PerformEqualityCheckFalse { get; set; }

        [NotifyProperty]
        public string PerformEqualityCheckDefault { get; set; }

    }

}