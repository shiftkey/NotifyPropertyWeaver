using System.Diagnostics;
using System.Linq;
using NUnit.Framework;

[TestFixture]
public class RecursiveIlFinderTests
{


    [Test]
    public void Run()
    {
        var typeDefinition = DefinitionFinder.FindType<InnerClass>();
        var recursiveIlFinder = new RecursiveIlFinder(typeDefinition);

        var methodDefinition = typeDefinition.Methods.First(x => x.Name == "Method1");
        recursiveIlFinder.Execute(methodDefinition);
#if(DEBUG)
        Assert.AreEqual(25, recursiveIlFinder.Instructions.Count);
#else
        Assert.AreEqual(15, recursiveIlFinder.Instructions.Count);
#endif
    }

    public abstract class InnerClass
    {


        public abstract string AbstractMethod();


        public void Method1()
        {
            Property = "wssdfsdf";
            Method2();
        }

        private void Method2()
        {
            AbstractMethod();
            Method3();
            Method1();
        }

        private void Method3()
        {
            Debug.WriteLine("a");
        }

        public string Property { get; set; }
    }
}