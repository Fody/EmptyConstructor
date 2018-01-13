using System;
using System.Linq;
using System.Reflection;
using Fody;
using Xunit;
#pragma warning disable 618

public class IntegrationTests
{
    static Assembly assembly;
    static TestResult testResult;

    static IntegrationTests()
    {
        var weavingTask = new ModuleWeaver();
        testResult = weavingTask.ExecuteTestRun("AssemblyToProcess.dll",
            assemblyName: nameof(IntegrationTests));
        assembly = testResult.Assembly;
    }

    [Fact]
    public void ClassInheritWithBothConstructors()
    {
       testResult.GetInstance("ClassInheritWithBothConstructors");
    }

    [Fact]
    public void ClassInheritWithEmptyConstructorFromOtherAssembly()
    {
        testResult.GetInstance("ClassInheritWithEmptyConstructorFromOtherAssembly");
    }

    [Fact]
    public void ClassInheritAbstractWithEmptyConstructor()
    {
        testResult.GetInstance("ClassInheritAbstractWithEmptyConstructor");
    }

    [Fact]
    public void ClassInheritWithNullableParam()
    {
        testResult.GetInstance("ClassInheritWithNullableParam");
    }

    [Fact]
    public void ClassInheritWithEmptyConstructor()
    {
        testResult.GetInstance("ClassInheritWithEmptyConstructor");
    }

    [Fact]
    public void ClassInheritWithNonEmptyConstructor()
    {
        testResult.GetInstance("ClassInheritWithNonEmptyConstructor");
    }

    [Fact]
    public void ClassWithBothConstructors()
    {
        testResult.GetInstance("ClassWithBothConstructors");
    }

    [Fact]
    public void ClassWithDefaultSingleParamConstructor()
    {
        var type = assembly.GetType("ClassWithDefaultSingleParamConstructor", true);
        Assert.Equal(2,type.GetConstructors().Length);
        Activator.CreateInstance(type,"aString");
    }

    [Fact]
    public void ClassWithEmptyConstructor()
    {
        testResult.GetInstance("ClassWithEmptyConstructor");
    }

    [Fact]
    public void ClassWithNoEmptyConstructor()
    {
        testResult.GetInstance("ClassWithNoEmptyConstructor");
    }

    [Fact]
    public void ClassWithPrivateEmptyConstructor()
    {
        Assert.Throws<MissingMethodException>(() => testResult.GetInstance("ClassWithPrivateConstructor"));
    }

    [Fact]
    public void ClassWithProtectedEmptyConstructor()
    {
        Assert.Throws<MissingMethodException>(() => testResult.GetInstance("ClassWithProtectedConstructor"));
    }

    [Fact]
    public void ClassAbstractWithPrivateEmptyConstructor()
    {
        var type = assembly.GetType("ClassAbstractWithPrivateConstructor", true);
        var constructorInfo = type
            .GetConstructors(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
            .Single(x => x.GetParameters().Length == 0);

        Assert.False(constructorInfo.IsPublic);
        Assert.False(constructorInfo.IsFamily);
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