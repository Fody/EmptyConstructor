using System;
using System.Linq;
using System.Reflection;
using Fody;
using Xunit;
using MethodAttributes = Mono.Cecil.MethodAttributes;
#pragma warning disable 618

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
        var type = assembly.GetType("ClassWithPrivateConstructor", true);
        var constructorInfo = type
            .GetConstructors(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
            .Single(x => x.GetParameters().Length == 0);

        Assert.False(constructorInfo.IsPrivate);
        Assert.True(constructorInfo.IsFamily);
        Assert.False(constructorInfo.IsPublic);
    }

    [Fact]
    public void ClassWithProtectedEmptyConstructor_MustNotBeAbleToConstruct()
    {
        Assert.Throws<MissingMethodException>(() => testResult.GetInstance("ClassWithProtectedConstructor"));
    }

    [Fact]
    public void ClassWithProtectedEmptyConstructor_MustHaveCorrectAccessModifier()
    {
        var type = assembly.GetType("ClassWithProtectedConstructor", true);
        var constructorInfo = type
            .GetConstructors(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
            .Single(x => x.GetParameters().Length == 0);

        Assert.False(constructorInfo.IsPrivate);
        Assert.True(constructorInfo.IsFamily);
        Assert.False(constructorInfo.IsPublic);
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
}