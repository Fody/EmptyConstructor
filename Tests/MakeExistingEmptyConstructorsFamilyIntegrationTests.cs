using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Mono.Cecil;
using NUnit.Framework;
using MethodAttributes = Mono.Cecil.MethodAttributes;

[TestFixture]
public class MakeExistingEmptyConstructorsFamilyIntegrationTests
{
    Assembly assembly;
    List<string> warnings = new List<string>();
    string beforeAssemblyPath;
    string afterAssemblyPath;

    public MakeExistingEmptyConstructorsFamilyIntegrationTests()
    {
        beforeAssemblyPath = Path.GetFullPath(Path.Combine(TestContext.CurrentContext.TestDirectory, @"..\..\..\AssemblyToProcess\bin\Debug\AssemblyToProcess.dll"));
#if (!DEBUG)
        beforeAssemblyPath = beforeAssemblyPath.Replace("Debug", "Release");
#endif

        afterAssemblyPath = beforeAssemblyPath.Replace(".dll", "5.dll");
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
                LogWarning = s => warnings.Add(s),
                Visibility = MethodAttributes.Family,
                MakeExistingEmptyConstructorsVisible = true
            };

            weavingTask.Execute();
            moduleDefinition.Write(afterAssemblyPath);
        }

        assembly = Assembly.LoadFile(afterAssemblyPath);
    }

    [Test]
    public void ClassWithPrivateEmptyConstructor_MustNotBeAbleToConstruct()
    {
        var type = assembly.GetType("ClassWithPrivateConstructor", true);
        Assert.Throws<MissingMethodException>(() => Activator.CreateInstance(type));
    }

    [Test]
    public void ClassWithPrivateEmptyConstructor_MustHaveCorrectAccessModifier()
    {
        var type = assembly.GetType("ClassWithPrivateConstructor", true);
        var constructorInfo = type
            .GetConstructors(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
            .Single(x => x.GetParameters().Length == 0);

        Assert.IsFalse(constructorInfo.IsPrivate);
        Assert.IsTrue(constructorInfo.IsFamily);
        Assert.IsFalse(constructorInfo.IsPublic);
    }

    [Test]
    public void ClassWithProtectedEmptyConstructor_MustNotBeAbleToConstruct()
    {
        var type = assembly.GetType("ClassWithProtectedConstructor", true);
        Assert.Throws<MissingMethodException>(() => Activator.CreateInstance(type));
    }

    [Test]
    public void ClassWithProtectedEmptyConstructor_MustHaveCorrectAccessModifier()
    {
        var type = assembly.GetType("ClassWithProtectedConstructor", true);
        var constructorInfo = type
            .GetConstructors(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
            .Single(x => x.GetParameters().Length == 0);

        Assert.IsFalse(constructorInfo.IsPrivate);
        Assert.IsTrue(constructorInfo.IsFamily);
        Assert.IsFalse(constructorInfo.IsPublic);
    }

    [Test]
    public void ClassAbstractWithPrivateEmptyConstructor()
    {
        var type = assembly.GetType("ClassAbstractWithPrivateConstructor", true);
        var constructorInfo = type
            .GetConstructors(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
            .Single(x => x.GetParameters().Length == 0);

        Assert.IsFalse(constructorInfo.IsFamily);
        Assert.IsFalse(constructorInfo.IsPublic);
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
}