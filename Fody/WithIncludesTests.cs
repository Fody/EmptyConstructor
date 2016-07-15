using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Mono.Cecil;
using NUnit.Framework;

[TestFixture]
public class WithIncludesTests
{
    Assembly assembly;
    List<string> warnings = new List<string>();
    string beforeAssemblyPath;
    string afterAssemblyPath;

    public WithIncludesTests()
    {
        beforeAssemblyPath = Path.GetFullPath(Path.Combine(TestContext.CurrentContext.TestDirectory,@"..\..\..\AssemblyToProcess\bin\Debug\AssemblyToProcess.dll"));
#if (!DEBUG)
        beforeAssemblyPath = beforeAssemblyPath.Replace("Debug", "Release");
#endif

        afterAssemblyPath = beforeAssemblyPath.Replace(".dll", "3.dll");
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
                                  IncludeNamespaces = new List<string>{"MyNameSpace"},
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
        Assert.AreEqual(1, type.GetConstructors().Length);
    }
    [Test]
    public void ClassInheritWithNonEmptyConstructorInNamespace()
    {
        var type = assembly.GetType("MyNameSpace.ClassWithNoEmptyConstructorInNamespace", true);
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