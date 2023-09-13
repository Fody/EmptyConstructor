using System;
using System.Linq;
using System.Reflection;

static class AssemblyExtensions
{
    public static dynamic GetInstance(this Assembly assembly, string className)
    {
        var type = assembly.GetType(className, true);
        //dynamic instance = FormatterServices.GetUninitializedObject(type);
        return Activator.CreateInstance(type);
    }

    public static ConstructorInfo GetConstructor(this Assembly assembly, string className)
    {
        var type = assembly.GetType(className, true);
        return type
            .GetConstructors(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
            .Single(_ => _.GetParameters().Length == 0);
    }
}