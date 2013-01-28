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

        assemblyPath = assemblyPath.Replace("Debug", "Release");
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
        var type = assembly.GetType("ClassInheritWithBothConstructors");
        Activator.CreateInstance(type);
    }
    [Test]
    public void ClassInheritWithEmptyConstructorFromOtherAssembly()
    {
        var type = assembly.GetType("ClassInheritWithEmptyConstructorFromOtherAssembly");
        Activator.CreateInstance(type);
    }
    [Test]
    public void ClassInheritAbstractWithEmptyConstructor()
    {
        var type = assembly.GetType("ClassInheritAbstractWithEmptyConstructor");
        Activator.CreateInstance(type);
    }
    [Test]
    public void ClassInheritWithEmptyConstructor()
    {
        var type = assembly.GetType("ClassInheritWithEmptyConstructor");
        Activator.CreateInstance(type);
    }
    [Test]
    public void ClassInheritWithNoEmptyConstructor()
    {
        var type = assembly.GetType("ClassInheritWithNoEmptyConstructor");
        Activator.CreateInstance(type);
    }
    [Test]
    public void ClassWithBothConstructors()
    {
        var type = assembly.GetType("ClassWithBothConstructors");
        Activator.CreateInstance(type);
    }
    [Test]
    public void ClassWithEmptyConstructor()
    {
        var type = assembly.GetType("ClassWithEmptyConstructor");
        Activator.CreateInstance(type);
    }
    [Test]
    public void ClassWithNoEmptyConstructor()
    {
        var type = assembly.GetType("ClassWithNoEmptyConstructor");
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