using System;
using System.Reflection;
using Fody;
using MethodAttributes = Mono.Cecil.MethodAttributes;

public class MakeExistingEmptyConstructorsFamilyIntegrationTests
{
    static Assembly assembly;
    static TestResult testResult;

    static MakeExistingEmptyConstructorsFamilyIntegrationTests()
    {
        var weaver = new ModuleWeaver
        {
            Visibility = MethodAttributes.Family,
            MakeExistingEmptyConstructorsVisible = true
        };
        testResult = weaver.ExecuteTestRun(
            "AssemblyToProcess.dll",
            assemblyName: nameof(MakeExistingEmptyConstructorsFamilyIntegrationTests));
        assembly = testResult.Assembly;
    }

    [Test]
    public async Task ClassWithPrivateEmptyConstructor_MustNotBeAbleToConstruct()
    {
        await Assert.That(() => testResult.GetInstance("ClassWithPrivateConstructor")).Throws<MissingMethodException>();
    }

    [Test]
    public async Task ClassWithPrivateEmptyConstructor_MustHaveCorrectAccessModifier()
    {
        var constructor = assembly.GetConstructor("ClassWithPrivateConstructor");
        await Assert.That(constructor.IsPrivate).IsFalse();
        await Assert.That(constructor.IsFamily).IsTrue();
        await Assert.That(constructor.IsPublic).IsFalse();
    }

    [Test]
    public async Task ClassWithProtectedEmptyConstructor_MustNotBeAbleToConstruct()
    {
        await Assert.That(() => testResult.GetInstance("ClassWithProtectedConstructor")).Throws<MissingMethodException>();
    }

    [Test]
    public async Task ClassWithProtectedEmptyConstructor_MustHaveCorrectAccessModifier()
    {
        var constructor = assembly.GetConstructor("ClassWithProtectedConstructor");
        await Assert.That(constructor.IsPrivate).IsFalse();
        await Assert.That(constructor.IsFamily).IsTrue();
        await Assert.That(constructor.IsPublic).IsFalse();
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