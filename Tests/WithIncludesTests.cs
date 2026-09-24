using System.Reflection;
using Fody;

public class WithIncludesTests
{
    static Assembly assembly;
    static TestResult testResult;

    static WithIncludesTests()
    {
        var weaver = new ModuleWeaver
        {
            IncludeNamespaces = new()
            {
                "MyNameSpace"
            },
        };
        testResult = weaver.ExecuteTestRun("AssemblyToProcess.dll",
            assemblyName: nameof(WithIncludesTests));
        assembly = testResult.Assembly;
    }

    [Test]
    public async Task ClassInheritWithNonEmptyConstructor()
    {
        var type = assembly.GetType("ClassInheritWithNonEmptyConstructor", true);
        await Assert.That(type.GetConstructors()).HasSingleItem();
    }

    [Test]
    public void ClassInheritWithNonEmptyConstructorInNamespace()
    {
        testResult.GetInstance("MyNameSpace.ClassWithNoEmptyConstructorInNamespace");
    }
}