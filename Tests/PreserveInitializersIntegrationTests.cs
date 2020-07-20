using Fody;
using Xunit;

public class PreserveInitializersIntegrationTests
{
    static TestResult testResult;

    static PreserveInitializersIntegrationTests()
    {
        var weavingTask = new ModuleWeaver
        {
            PreserveInitializers = true
        };
        testResult = weavingTask.ExecuteTestRun("AssemblyToProcess.dll",
            assemblyName: nameof(PreserveInitializersIntegrationTests));
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

    [Fact]
    public void ReproBug143()
    {
        var instance = testResult.GetInstance("Bug143Child");
        Assert.NotNull(instance);
    }
}