using System.Collections.Generic;
using System.Reflection;
using Fody;
using Xunit;

public class WithExcludesTests
{
    static Assembly assembly;
    static TestResult testResult;

    static WithExcludesTests()
    {
        var weavingTask = new ModuleWeaver
        {
            ExcludeNamespaces = new List<string> { "MyNameSpace" },
        };
        testResult = weavingTask.ExecuteTestRun("AssemblyToProcess.dll",
            assemblyName: nameof(WithExcludesTests));
        assembly = testResult.Assembly;
    }

    [Fact]
    public void ClassInheritWithNonEmptyConstructor()
    {
        testResult.GetInstance("ClassInheritWithNonEmptyConstructor");
    }

    [Fact]
    public void ClassInheritWithNonEmptyConstructorInNamespace()
    {
        var type = assembly.GetType("MyNameSpace.ClassWithNoEmptyConstructorInNamespace", true);
        Assert.Single(type.GetConstructors());
    }
}