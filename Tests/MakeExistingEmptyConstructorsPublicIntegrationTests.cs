using System.Reflection;
using Fody;
using MethodAttributes = Mono.Cecil.MethodAttributes;

public class MakeExistingEmptyConstructorsPublicIntegrationTests
{
    static Assembly assembly;
    static TestResult testResult;

    static MakeExistingEmptyConstructorsPublicIntegrationTests()
    {
        var weaver = new ModuleWeaver
        {
            Visibility = MethodAttributes.Public,
            MakeExistingEmptyConstructorsVisible = true
        };
        testResult = weaver.ExecuteTestRun(
            "AssemblyToProcess.dll",
            assemblyName: nameof(MakeExistingEmptyConstructorsPublicIntegrationTests));
        assembly = testResult.Assembly;
    }

    [Test]
    public void ClassWithPrivateEmptyConstructor_MustBeAbleToConstruct()
    {
        testResult.GetInstance("ClassWithPrivateConstructor");
    }

    [Test]
    public async Task ClassWithPrivateEmptyConstructor_MustHaveCorrectAccessModifier()
    {
        var constructor = assembly.GetConstructor("ClassWithPrivateConstructor");
        await Assert.That(constructor.IsPrivate).IsFalse();
        await Assert.That(constructor.IsFamily).IsFalse();
        await Assert.That(constructor.IsPublic).IsTrue();
    }

    [Test]
    public void ClassWithProtectedEmptyConstructor_MustBeAbleToConstruct()
    {
        testResult.GetInstance("ClassWithProtectedConstructor");
    }

    [Test]
    public async Task ClassWithProtectedEmptyConstructor_MustHaveCorrectAccessModifier()
    {
        var constructor = assembly.GetConstructor("ClassWithProtectedConstructor");
        await Assert.That(constructor.IsPrivate).IsFalse();
        await Assert.That(constructor.IsFamily).IsFalse();
        await Assert.That(constructor.IsPublic).IsTrue();
    }

    [Test]
    public async Task ClassAbstractWithPrivateEmptyConstructor()
    {
        var constructor = assembly.GetConstructor("ClassAbstractWithPrivateConstructor");
        await Assert.That(constructor.IsFamily).IsFalse();
        await Assert.That(constructor.IsPublic).IsFalse();
    }

    [Test]
    public async Task ClassAbstractWithProtectedEmptyConstructor()
    {
        var constructor = assembly.GetConstructor("ClassAbstractWithProtectedConstructor");
        await Assert.That(constructor.IsFamily).IsTrue();
        await Assert.That(constructor.IsPublic).IsFalse();
    }
}