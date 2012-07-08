using System.Linq;
using Mono.Cecil;
using NSubstitute;
using NUnit.Framework;


[TestFixture]
public class MethodFinderTest
{

    TypeDefinition typeDefinition;
    MethodFinder methodFinder;

    public MethodFinderTest()
    {

        var codeBase = typeof(MethodFinderTest).Assembly.CodeBase.Replace("file:///", string.Empty);
        var module = ModuleDefinition.ReadModule(codeBase);
        var moduleReader = new ModuleReader(null, null, Substitute.For<Logger>()) { Module = module };
        methodFinder = new MethodFinder(new MethodGenerifier(moduleReader), null, null, moduleReader, Substitute.For<Logger>(), null, new EventInvokerNameResolver(null));

        typeDefinition = module.Types.First(x => x.Name.EndsWith("MethodFinderTest"));
    }


    [Test]
    public void WithStringParamTest()
    {
        var definitionToProcess = typeDefinition.NestedTypes.First(x => x.Name == "WithStringParam");
        var methodReference = methodFinder.RecursiveFindMethod(definitionToProcess);
        Assert.IsNotNull(methodReference);
        Assert.AreEqual("OnPropertyChanged", methodReference.MethodReference.Name);
    }

    public class WithStringParam
    {
        public void OnPropertyChanged(string propertyName)
        {
        }
    }


    [Test]
    public void WithStringAndBeforeAfterParamTest()
    {
        var definitionToProcess = typeDefinition.NestedTypes.First(x => x.Name == "WithStringAndBeforeAfter");
        var methodReference = methodFinder.RecursiveFindMethod(definitionToProcess);
        Assert.IsNotNull(methodReference);
        Assert.AreEqual("OnPropertyChanged", methodReference.MethodReference.Name);
        Assert.IsTrue(methodReference.IsBeforeAfter);
    }

    public class WithStringAndBeforeAfter
    {
        public void OnPropertyChanged(string propertyName, object before, object after)
        {
        }
    }


    [Test]
    public void NoMethodTest()
    {

        var definitionToProcess = typeDefinition.NestedTypes.First(x => x.Name == "NoMethod");
        Assert.IsNull(methodFinder.RecursiveFindMethod(definitionToProcess));
    }

    public class NoMethod
    {
    }
    [Test]
    public void NoParamsTest()
    {
        var definitionToProcess = typeDefinition.NestedTypes.First(x => x.Name == "NoParams");
        Assert.IsNull(methodFinder.RecursiveFindMethod(definitionToProcess));
    }

    public class NoParams
    {
        public void OnPropertyChanged()
        {
        }
    }

    [Test]
    public void WrongParamsTest()
    {

        var definitionToProcess = typeDefinition.NestedTypes.First(x => x.Name == "WrongParams");
        Assert.IsNull(methodFinder.RecursiveFindMethod(definitionToProcess));
    }

    public class WrongParams
    {
        public void OnPropertyChanged(int propertyName)
        {
        }
    }

}
