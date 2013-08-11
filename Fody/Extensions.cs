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
        if (!x.IsConstructor)
        {
            return false;
        }
        if (x.Parameters.Count == 0)
        {
            return true;
        }
        if (x.Parameters.Count == 1)
        {
            var attributes = x.Parameters.First().Attributes;
            if (
                ((attributes & ParameterAttributes.Optional) != 0) &&
                ((attributes & ParameterAttributes.HasDefault) != 0)
                )
            {
                return true;
            }
        }
        return false;
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