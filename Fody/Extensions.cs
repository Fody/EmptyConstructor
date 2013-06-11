using System.Linq;
using Mono.Cecil;

public static class Extensions
{
    public static bool HasEmptyConstructor(this TypeDefinition typeDefinition)
    {
        return typeDefinition.Methods.Any(IsEmptyConstructor);
    }

    public static MethodDefinition GetEmptyConstructor(this TypeDefinition typeDefinition)
    {
        return typeDefinition.Methods.FirstOrDefault(IsEmptyConstructor);
    }

    public static bool IsEmptyConstructor(this MethodDefinition x)
    {
        return x.IsConstructor && x.Parameters.Count == 0;
    }

    public static bool IsDelegate(this TypeDefinition typeDefinition)
    {
        if (typeDefinition.BaseType == null)
        {
            return false;
        }
        return typeDefinition.BaseType.FullName == "System.MulticastDelegate";
    }

}