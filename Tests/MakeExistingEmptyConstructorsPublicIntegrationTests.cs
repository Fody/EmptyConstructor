using System.Reflection;
using Fody;
using Xunit;
using MethodAttributes = Mono.Cecil.MethodAttributes;

public class MakeExistingEmptyConstructorsPublicIntegrationTests
{
    static Assembly assembly;
    static TestResult testResult;

    static MakeExistingEmptyConstructorsPublicIntegrationTests()
    {
        var weavingTask = new ModuleWeaver
        {
            Visibility = MethodAttributes.Public,
            MakeExistingEmptyConstructorsVisible = true
        };
        testResult = weavingTask.ExecuteTestRun("AssemblyToProcess.dll",
            assemblyName: nameof(MakeExistingEmptyConstructorsPublicIntegrationTests));
        assembly = testResult.Assembly;
    }

    [Fact]
    public void ClassWithPrivateEmptyConstructor_MustBeAbleToConstruct()
    {
        testResult.GetInstance("ClassWithPrivateConstructor");
    }

    [Fact]
    public void ClassWithPrivateEmptyConstructor_MustHaveCorrectAccessModifier()
    {
        var constructor = assembly.GetConstructor("ClassWithPrivateConstructor");
        Assert.False(constructor.IsPrivate);
        Assert.False(constructor.IsFamily);
        Assert.True(constructor.IsPublic);
    }

    [Fact]
    public void ClassWithProtectedEmptyConstructor_MustBeAbleToConstruct()
    {
        testResult.GetInstance("ClassWithProtectedConstructor");
    }

    [Fact]
    public void ClassWithProtectedEmptyConstructor_MustHaveCorrectAccessModifier()
    {
        var constructor = assembly.GetConstructor("ClassWithProtectedConstructor");
        Assert.False(constructor.IsPrivate);
        Assert.False(constructor.IsFamily);
        Assert.True(constructor.IsPublic);
    }

    [Fact]
    public void ClassAbstractWithPrivateEmptyConstructor()
    {
        var constructor = assembly.GetConstructor("ClassAbstractWithPrivateConstructor");
        Assert.False(constructor.IsFamily);
        Assert.False(constructor.IsPublic);
    }

    [Fact]
    public void ClassAbstractWithProtectedEmptyConstructor()
    {
        var constructor = assembly.GetConstructor("ClassAbstractWithProtectedConstructor");
        Assert.True(constructor.IsFamily);
        Assert.False(constructor.IsPublic);
    }
}