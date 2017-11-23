using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Mono.Cecil;
using NUnit.Framework;

[TestFixture]
public class IntegrationTests
{
    Assembly assembly;
    List<string> warnings = new List<string>();
    string beforeAssemblyPath;
    string afterAssemblyPath;

    public IntegrationTests()
    {
        beforeAssemblyPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "AssemblyToProcess.dll");

        afterAssemblyPath = beforeAssemblyPath.Replace(".dll", "2.dll");
        File.Copy(beforeAssemblyPath, afterAssemblyPath, true);

        var assemblyResolver = new MockAssemblyResolver
        {
            Directory = Path.GetDirectoryName(beforeAssemblyPath)
        };
        using (var moduleDefinition = ModuleDefinition.ReadModule(beforeAssemblyPath, new ReaderParameters
        {
            AssemblyResolver = assemblyResolver
        }))
        {
            var weavingTask = new ModuleWeaver
            {
                ModuleDefinition = moduleDefinition,
                AssemblyResolver = assemblyResolver,
                LogWarning = s => warnings.Add(s)
            };

            weavingTask.Execute();
            moduleDefinition.Write(afterAssemblyPath);
        }

        assembly = Assembly.LoadFile(afterAssemblyPath);
    }

    [Test]
    public void ClassInheritWithBothConstructors()
    {
        var type = assembly.GetType("ClassInheritWithBothConstructors", true);
        Activator.CreateInstance(type);
    }

    [Test]
    public void ClassInheritWithEmptyConstructorFromOtherAssembly()
    {
        var type = assembly.GetType("ClassInheritWithEmptyConstructorFromOtherAssembly", true);
        Activator.CreateInstance(type);
    }

    [Test]
    public void ClassInheritAbstractWithEmptyConstructor()
    {
        var type = assembly.GetType("ClassInheritAbstractWithEmptyConstructor", true);
        Activator.CreateInstance(type);
    }

    [Test]
    public void ClassInheritWithNullableParam()
    {
        var type = assembly.GetType("ClassInheritWithNullableParam", true);
        Activator.CreateInstance(type);
    }

    [Test]
    [Explicit]
    public void ClassWithInitializedFields()
    {
        var type = assembly.GetType("ClassWithInitializedFields", true);
        var instance = (dynamic)Activator.CreateInstance(type);
        Assert.AreEqual(9, instance.X);
        Assert.AreEqual("aString", instance.Y);
        Assert.IsNotNull( instance.Z);
    }

    [Test]
    public void ClassInheritWithEmptyConstructor()
    {
        var type = assembly.GetType("ClassInheritWithEmptyConstructor", true);
        Activator.CreateInstance(type);
    }

    [Test]
    public void ClassInheritWithNonEmptyConstructor()
    {
        var type = assembly.GetType("ClassInheritWithNonEmptyConstructor", true);
        Activator.CreateInstance(type);
    }

    [Test]
    public void ClassWithBothConstructors()
    {
        var type = assembly.GetType("ClassWithBothConstructors", true);
        Activator.CreateInstance(type);
    }

    [Test]
    public void ClassWithDefaultSingleParamConstructor()
    {
        var type = assembly.GetType("ClassWithDefaultSingleParamConstructor", true);
        Assert.AreEqual(2,type.GetConstructors().Length);
        Activator.CreateInstance(type,"aString");
    }

    [Test]
    public void ClassWithEmptyConstructor()
    {
        var type = assembly.GetType("ClassWithEmptyConstructor", true);
        Activator.CreateInstance(type);
    }

    [Test]
    public void ClassWithNoEmptyConstructor()
    {
        var type = assembly.GetType("ClassWithNoEmptyConstructor", true);
        Activator.CreateInstance(type);
    }

    [Test]
    public void ClassWithPrivateEmptyConstructor()
    {
        var type = assembly.GetType("ClassWithPrivateConstructor", true);
        Assert.Throws<MissingMethodException>(() => Activator.CreateInstance(type));
    }

    [Test]
    public void ClassWithProtectedEmptyConstructor()
    {
        var type = assembly.GetType("ClassWithProtectedConstructor", true);
        Assert.Throws<MissingMethodException>(() => Activator.CreateInstance(type));
    }

    [Test]
    public void ClassAbstractWithPrivateEmptyConstructor()
    {
        var type = assembly.GetType("ClassAbstractWithPrivateConstructor", true);
        var constructorInfo = type
            .GetConstructors(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
            .Single(x => x.GetParameters().Length == 0);

        Assert.IsFalse(constructorInfo.IsPublic);
        Assert.IsFalse(constructorInfo.IsFamily);
    }

    [Test]
    public void ClassAbstractWithProtectedEmptyConstructor()
    {
        var type = assembly.GetType("ClassAbstractWithProtectedConstructor", true);
        var constructorInfo = type
            .GetConstructors(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
            .Single(x => x.GetParameters().Length == 0);

        Assert.IsTrue(constructorInfo.IsFamily);
        Assert.IsFalse(constructorInfo.IsPublic);
    }

    [Test]
    public void PeVerify()
    {
        Verifier.Verify(beforeAssemblyPath,afterAssemblyPath);
    }
}