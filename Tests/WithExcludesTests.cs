using System.Reflection;
using Fody;

public class WithExcludesTests
{
    static Assembly assembly;
    static TestResult testResult;

    static WithExcludesTests()
    {
        var weaver = new ModuleWeaver
        {
            ExcludeNamespaces = new()
            {
                "MyNameSpace"
            },
        };
        testResult = weaver.ExecuteTestRun(
            "AssemblyToProcess.dll",
            assemblyName: nameof(WithExcludesTests));
        assembly = testResult.Assembly;
    }

    [Test]
    public void ClassInheritWithNonEmptyConstructor()
    {
        testResult.GetInstance("ClassInheritWithNonEmptyConstructor");
    }

    [Test]
    public async Task ClassInheritWithNonEmptyConstructorInNamespace()
    {
        var type = assembly.GetType("MyNameSpace.ClassWithNoEmptyConstructorInNamespace", true);
        await Assert.That(type.GetConstructors()).HasSingleItem();
    }
}