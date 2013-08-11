using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

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


    public static Instruction Clone(this Instruction instruction)
    {
        var cloned = Instruction.Create(OpCodes.Ldarg_0);
        cloned.OpCode = instruction.OpCode;
        cloned.Operand = instruction.Operand;
        return cloned;
    }

    public static bool IsEmptyConstructor(this MethodDefinition x)
    {
        if (!x.IsConstructor)
        {
            return false;
        }
        if (x.IsStatic)
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