using System;
using System.Reflection;
using Fody;
using Xunit;
using MethodAttributes = Mono.Cecil.MethodAttributes;

public class MakeExistingEmptyConstructorsFamilyIntegrationTests
{
    static Assembly assembly;
    static TestResult testResult;

    static MakeExistingEmptyConstructorsFamilyIntegrationTests()
    {
        var weavingTask = new ModuleWeaver
        {
            Visibility = MethodAttributes.Family,
            MakeExistingEmptyConstructorsVisible = true
        };
        testResult = weavingTask.ExecuteTestRun("AssemblyToProcess.dll",
            assemblyName: nameof(MakeExistingEmptyConstructorsFamilyIntegrationTests));
        assembly = testResult.Assembly;
    }

    [Fact]
    public void ClassWithPrivateEmptyConstructor_MustNotBeAbleToConstruct()
    {
        Assert.Throws<MissingMethodException>(() => testResult.GetInstance("ClassWithPrivateConstructor"));
    }

    [Fact]
    public void ClassWithPrivateEmptyConstructor_MustHaveCorrectAccessModifier()
    {
        var constructor = assembly.GetConstructor("ClassWithPrivateConstructor");
        Assert.False(constructor.IsPrivate);
        Assert.True(constructor.IsFamily);
        Assert.False(constructor.IsPublic);
    }

    [Fact]
    public void ClassWithProtectedEmptyConstructor_MustNotBeAbleToConstruct()
    {
        Assert.Throws<MissingMethodException>(() => testResult.GetInstance("ClassWithProtectedConstructor"));
    }

    [Fact]
    public void ClassWithProtectedEmptyConstructor_MustHaveCorrectAccessModifier()
    {
        var constructor = assembly.GetConstructor("ClassWithProtectedConstructor");
        Assert.False(constructor.IsPrivate);
        Assert.True(constructor.IsFamily);
        Assert.False(constructor.IsPublic);
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