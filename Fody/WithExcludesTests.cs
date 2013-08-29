using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Mono.Cecil;
using NUnit.Framework;

[TestFixture]
public class WithExcludesTests
{
    Assembly assembly;
    List<string> warnings = new List<string>();
    string beforeAssemblyPath;
    string afterAssemblyPath;

    public WithExcludesTests()
    {
        beforeAssemblyPath = Path.GetFullPath(@"..\..\..\AssemblyToProcess\bin\Debug\AssemblyToProcess.dll");
#if (!DEBUG)
        beforeAssemblyPath = beforeAssemblyPath.Replace("Debug", "Release");
#endif

        afterAssemblyPath = beforeAssemblyPath.Replace(".dll", "4.dll");
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
                                  ExcludeNamespaces = new List<string>{"MyNameSpace"},
                                  LogWarning =s => warnings.Add(s)
                              };

        weavingTask.Execute();
        moduleDefinition.Write(afterAssemblyPath);

        assembly = Assembly.LoadFile(afterAssemblyPath);
    }

    [Test]
    public void ClassInheritWithNonEmptyConstructor()
    {
        var type = assembly.GetType("ClassInheritWithNonEmptyConstructor", true);
        Activator.CreateInstance(type);
    }

    [Test]
    public void ClassInheritWithNonEmptyConstructorInNamespace()
    {
        var type = assembly.GetType("MyNameSpace.ClassWithNoEmptyConstructorInNamespace", true);
        Assert.AreEqual(1, type.GetConstructors().Length);
    }


#if(DEBUG)
    [Test]
    public void PeVerify()
    {
        Verifier.Verify(beforeAssemblyPath,afterAssemblyPath);
    }
#endif

}