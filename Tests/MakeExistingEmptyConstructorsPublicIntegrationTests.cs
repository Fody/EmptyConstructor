using System.Linq;
using System.Reflection;
using Fody;
using Xunit;
using Xunit.Abstractions;
using MethodAttributes = Mono.Cecil.MethodAttributes;

public class MakeExistingEmptyConstructorsPublicIntegrationTests :
    XunitLoggingBase
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
        var type = assembly.GetType("ClassWithPrivateConstructor", true);
        var constructorInfo = type
            .GetConstructors(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
            .Single(x => x.GetParameters().Length == 0);

        Assert.False(constructorInfo.IsPrivate);
        Assert.False(constructorInfo.IsFamily);
        Assert.True(constructorInfo.IsPublic);
    }

    [Fact]
    public void ClassWithProtectedEmptyConstructor_MustBeAbleToConstruct()
    {
        testResult.GetInstance("ClassWithProtectedConstructor");
    }

    [Fact]
    public void ClassWithProtectedEmptyConstructor_MustHaveCorrectAccessModifier()
    {
        var type = assembly.GetType("ClassWithProtectedConstructor", true);
        var constructorInfo = type
            .GetConstructors(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
            .Single(x => x.GetParameters().Length == 0);

        Assert.False(constructorInfo.IsPrivate);
        Assert.False(constructorInfo.IsFamily);
        Assert.True(constructorInfo.IsPublic);
    }

    [Fact]
    public void ClassAbstractWithPrivateEmptyConstructor()
    {
        var type = assembly.GetType("ClassAbstractWithPrivateConstructor", true);
        var constructorInfo = type
            .GetConstructors(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
            .Single(x => x.GetParameters().Length == 0);

        Assert.False(constructorInfo.IsFamily);
        Assert.False(constructorInfo.IsPublic);
    }

    [Fact]
    public void ClassAbstractWithProtectedEmptyConstructor()
    {
        var type = assembly.GetType("ClassAbstractWithProtectedConstructor", true);
        var constructorInfo = type
            .GetConstructors(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
            .Single(x => x.GetParameters().Length == 0);

        Assert.True(constructorInfo.IsFamily);
        Assert.False(constructorInfo.IsPublic);
    }

    public MakeExistingEmptyConstructorsPublicIntegrationTests(ITestOutputHelper output) :
        base(output)
    {
    }
}