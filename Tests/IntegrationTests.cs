using System;
using System.Reflection;
using Fody;

public class IntegrationTests
{
    static Assembly assembly;
    static TestResult testResult;

    static IntegrationTests()
    {
        var weaver = new ModuleWeaver();
        testResult = weaver.ExecuteTestRun(
            "AssemblyToProcess.dll",
            assemblyName: nameof(IntegrationTests));
        assembly = testResult.Assembly;
    }

    [Test]
    public void ClassInheritWithBothConstructors()
    {
        testResult.GetInstance("ClassInheritWithBothConstructors");
    }

    [Test]
    public void ClassInheritWithEmptyConstructorFromOtherAssembly()
    {
        testResult.GetInstance("ClassInheritWithEmptyConstructorFromOtherAssembly");
    }

    [Test]
    public void ClassInheritGenericWithEmptyConstructorFromOtherAssembly()
    {
        testResult.GetInstance("ClassInheritGenericWithEmptyConstructorFromOtherAssembly");
    }

    [Test]
    public async Task ClassInheritWithNonEmptyConstructorFromOtherAssembly()
    {
        await Assert.That(() => testResult.GetInstance("ClassInheritWithNonEmptyConstructorFromOtherAssembly")).Throws<MissingMethodException>();
    }

    [Test]
    public async Task ClassInheritGenericWithNonEmptyConstructorFromOtherAssembly()
    {
        await Assert.That(() => testResult.GetInstance("ClassInheritGenericWithNonEmptyConstructorFromOtherAssembly")).Throws<MissingMethodException>();
    }

    [Test]
    public void ClassInheritAbstractWithEmptyConstructor()
    {
        testResult.GetInstance("ClassInheritAbstractWithEmptyConstructor");
    }

    [Test]
    public void ClassInheritWithNullableParam()
    {
        testResult.GetInstance("ClassInheritWithNullableParam");
    }

    [Test]
    public void ClassInheritWithEmptyConstructor()
    {
        testResult.GetInstance("ClassInheritWithEmptyConstructor");
    }

    [Test]
    public void ClassInheritWithNonEmptyConstructor()
    {
        testResult.GetInstance("ClassInheritWithNonEmptyConstructor");
    }

    [Test]
    public void ClassWithBothConstructors()
    {
        testResult.GetInstance("ClassWithBothConstructors");
    }

    [Test]
    public async Task ClassWithDefaultSingleParamConstructor()
    {
        var type = assembly.GetType("ClassWithDefaultSingleParamConstructor", true);
        await Assert.That(type.GetConstructors().Length).IsEqualTo(2);
        Activator.CreateInstance(type, "aString");
    }

    [Test]
    public void ClassWithEmptyConstructor()
    {
        testResult.GetInstance("ClassWithEmptyConstructor");
    }

    [Test]
    public void ClassWithNoEmptyConstructor()
    {
        testResult.GetInstance("ClassWithNoEmptyConstructor");
    }

    [Test]
    public async Task ClassWithPrivateEmptyConstructor()
    {
        await Assert.That(() => testResult.GetInstance("ClassWithPrivateConstructor")).Throws<MissingMethodException>();
    }

    [Test]
    public async Task ClassWithProtectedEmptyConstructor()
    {
        await Assert.That(() => testResult.GetInstance("ClassWithProtectedConstructor")).Throws<MissingMethodException>();
    }

    [Test]
    public async Task ClassAbstractWithPrivateEmptyConstructor()
    {
        var constructor = assembly.GetConstructor("ClassAbstractWithPrivateConstructor");
        await Assert.That(constructor.IsPublic).IsFalse();
        await Assert.That(constructor.IsFamily).IsFalse();
    }

    [Test]
    public async Task ClassAbstractWithProtectedEmptyConstructor()
    {
        var constructor = assembly.GetConstructor("ClassAbstractWithProtectedConstructor");
        await Assert.That(constructor.IsFamily).IsTrue();
        await Assert.That(constructor.IsPublic).IsFalse();
    }

    [Test]
    public void ClassWithGeneric()
    {
        testResult.GetGenericInstance("ClassWithGeneric`1", typeof(string));
    }

    [Test]
    public void ClassInheritWithGeneric()
    {
        testResult.GetInstance("ClassInheritWithGeneric");
    }

    [Test]
    public void ClassWithGenericInheritWithGeneric()
    {
        testResult.GetGenericInstance("ClassWithGenericInheritWithGeneric`1", typeof(object));
    }

    [Test]
    public void ClassInheritWithGenericInheritWithGeneric()
    {
        testResult.GetInstance("ClassInheritWithGenericInheritWithGeneric");
    }

    [Test]
    public void ClassWithGenericInReverseDeclarationOrder()
    {
        testResult.GetGenericInstance("ClassWithGenericInReverseDeclarationOrder`1", typeof(object));
    }

    [Test]
    public void ClassInheritWithGenericInReverseDeclarationOrder()
    {
        testResult.GetInstance("ClassInheritWithGenericInReverseDeclarationOrder");
    }

    [Test]
    public async Task ClassWithInitializedFields()
    {
        var instance = testResult.GetInstance("ClassWithInitializedFields");
        await Assert.That((object?)instance.X).IsEqualTo(0);
        await Assert.That((object?)instance.Y).IsNull();
        await Assert.That((object?)instance.Z).IsNull();
    }

    [Test]
    public async Task ClassWithInitializedProperties()
    {
        var instance = testResult.GetInstance("ClassWithInitializedProperties");
        await Assert.That((object?)instance.X).IsEqualTo(0);
        await Assert.That((object?)instance.Y).IsNull();
        await Assert.That((object?)instance.Z).IsNull();
    }

    [Test]
    public async Task ReproBug143()
    {
        var instance = testResult.GetInstance("Bug143Child");
        await Assert.That((object?)instance).IsNotNull();
    }

#if NET5_0
    [Test]
    public void RecordWithParameter()
    {
        testResult.GetInstance("RecordWithParameter");
    }
#endif
}