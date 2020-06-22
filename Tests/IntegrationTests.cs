using System;
using System.Reflection;
using Fody;
using Xunit;

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
    public void ClassInheritGenericWithEmptyConstructorFromOtherAssembly()
    {
        testResult.GetInstance("ClassInheritGenericWithEmptyConstructorFromOtherAssembly");
    }

    [Fact]
    public void ClassInheritWithNonEmptyConstructorFromOtherAssembly()
    {
        Assert.Throws<MissingMethodException>(() => testResult.GetInstance("ClassInheritWithNonEmptyConstructorFromOtherAssembly"));
    }

    [Fact]
    public void ClassInheritGenericWithNonEmptyConstructorFromOtherAssembly()
    {
        Assert.Throws<MissingMethodException>(() => testResult.GetInstance("ClassInheritGenericWithNonEmptyConstructorFromOtherAssembly"));
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
        Assert.Equal(2, type.GetConstructors().Length);
        Activator.CreateInstance(type, "aString");
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
        var constructor = assembly.GetConstructor("ClassAbstractWithPrivateConstructor");
        Assert.False(constructor.IsPublic);
        Assert.False(constructor.IsFamily);
    }

    [Fact]
    public void ClassAbstractWithProtectedEmptyConstructor()
    {
        var constructor = assembly.GetConstructor("ClassAbstractWithProtectedConstructor");
        Assert.True(constructor.IsFamily);
        Assert.False(constructor.IsPublic);
    }

    [Fact]
    public void ClassWithGeneric()
    {
        testResult.GetGenericInstance("ClassWithGeneric`1", typeof(string));
    }

    [Fact]
    public void ClassInheritWithGeneric()
    {
        testResult.GetInstance("ClassInheritWithGeneric");
    }

    [Fact]
    public void ClassWithGenericInheritWithGeneric()
    {
        testResult.GetGenericInstance("ClassWithGenericInheritWithGeneric`1", typeof(object));
    }

    [Fact]
    public void ClassInheritWithGenericInheritWithGeneric()
    {
        testResult.GetInstance("ClassInheritWithGenericInheritWithGeneric");
    }

    [Fact]
    public void ClassWithGenericInReverseDeclarationOrder()
    {
        testResult.GetGenericInstance("ClassWithGenericInReverseDeclarationOrder`1", typeof(object));
    }

    [Fact]
    public void ClassInheritWithGenericInReverseDeclarationOrder()
    {
        testResult.GetInstance("ClassInheritWithGenericInReverseDeclarationOrder");
    }

    [Fact]
    public void ClassWithInitializedFields()
    {
        var instance = testResult.GetInstance("ClassWithInitializedFields");
        Assert.Equal(0, instance.X);
        Assert.Equal(null, instance.Y);
        Assert.Null(instance.Z);
    }

    [Fact]
    public void ClassWithInitializedProperties()
    {
        var instance = testResult.GetInstance("ClassWithInitializedProperties");
        Assert.Equal(0, instance.X);
        Assert.Equal(null, instance.Y);
        Assert.Null(instance.Z);
    }
}