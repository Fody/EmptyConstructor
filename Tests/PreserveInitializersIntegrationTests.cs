using System.Reflection;
using Fody;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

public class PreserveInitializersIntegrationTests :
    VerifyBase
{
    static Assembly assembly;
    static TestResult testResult;

    static PreserveInitializersIntegrationTests()
    {
        var weavingTask = new ModuleWeaver
        {
            PreserveInitializers = true
        };
        testResult = weavingTask.ExecuteTestRun("AssemblyToProcess.dll",
            assemblyName: nameof(MakeExistingEmptyConstructorsFamilyIntegrationTests));
        assembly = testResult.Assembly;
    }

    [Fact]
    public void ClassWithInitializedFields()
    {
        var instance = testResult.GetInstance("ClassWithInitializedFields");
        Assert.Equal(9, instance.X);
        Assert.Equal("aString", instance.Y);
        Assert.NotNull(instance.Z);
    }

    [Fact]
    public void ClassWithInitializedProperties()
    {
        var instance = testResult.GetInstance("ClassWithInitializedProperties");
        Assert.Equal(9, instance.X);
        Assert.Equal("aString", instance.Y);
        Assert.NotNull(instance.Z);
    }

    public PreserveInitializersIntegrationTests(ITestOutputHelper output) :
        base(output)
    {
    }
}