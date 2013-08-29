using System;
using System.Collections.Generic;
using System.IO;
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
        beforeAssemblyPath = Path.GetFullPath(@"..\..\..\AssemblyToProcess\bin\Debug\AssemblyToProcess.dll");
#if (!DEBUG)
        beforeAssemblyPath = beforeAssemblyPath.Replace("Debug", "Release");
#endif

        afterAssemblyPath = beforeAssemblyPath.Replace(".dll", "2.dll");
        File.Copy(beforeAssemblyPath, afterAssemblyPath, true);

        var assemblyResolver = new MockAssemblyResolver
            {
                Directory = Path.GetDirectoryName(beforeAssemblyPath)
            };
        var moduleDefinition = ModuleDefinition.ReadModule(afterAssemblyPath,new ReaderParameters
            {
                AssemblyResolver = assemblyResolver
            });
        var weavingTask = new ModuleWeaver
                              {
                                  ModuleDefinition = moduleDefinition,
                                  AssemblyResolver = assemblyResolver,
                                  LogWarning =s => warnings.Add(s)
                              };

        weavingTask.Execute();
        moduleDefinition.Write(afterAssemblyPath);

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
        Assert.AreEqual(1,type.GetConstructors().Length);
        Activator.CreateInstance(type,"aString");
    }

    [Test]
    public void ClassWithEmptyConstructor()
    {
        var type = assembly.GetType("ClassWithEmptyConstructor", true);
        Activator.CreateInstance(type);
    }

    [Test]
    public void ClassWithNonEmptyConstructor()
    {
        var type = assembly.GetType("ClassWithNonEmptyConstructor", true);
        Activator.CreateInstance(type);
    }



#if(DEBUG)
    [Test]
    public void PeVerify()
    {
        Verifier.Verify(beforeAssemblyPath,afterAssemblyPath);
    }
#endif

}