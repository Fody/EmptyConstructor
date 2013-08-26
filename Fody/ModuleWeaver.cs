using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

public class ModuleWeaver
{
    public Action<string> LogInfo { get; set; }
    public Action<string> LogWarning { get; set; }
    public ModuleDefinition ModuleDefinition { get; set; }
    public IAssemblyResolver AssemblyResolver { get; set; }
    public XElement Config { get; set; }
    
    public ModuleWeaver()
    {
        LogInfo = s => { };
        LogWarning = s => { };
    }

    public void Execute()
    {
        var queue = new Queue<TypeDefinition>(ModuleDefinition.GetTypes());

        var processed = new Dictionary<TypeDefinition, MethodReference>();
        var external = new Dictionary<TypeReference, MethodReference>();
        while (queue.Count != 0)
        {
            var type = queue.Dequeue();
            if (!type.IsClass)
            {
                continue;
            }
            if (type.IsValueType)
            {
                continue;
            }
            if (type.IsDelegate())
            {
                continue;
            }
            var baseType = type.BaseType;
            if (baseType == null)
            {
                continue;
            }

            var typeEmptyConstructor = type.GetEmptyConstructor();

            if (typeEmptyConstructor != null)
            {
                processed.Add(type, typeEmptyConstructor);
                continue;
            }


            MethodReference baseEmptyConstructor;
            var baseTypeDefinition = baseType as TypeDefinition;
            if (baseTypeDefinition != null)
            {
                if (!processed.TryGetValue(baseTypeDefinition, out baseEmptyConstructor))
                {
                    queue.Enqueue(type);
                    continue;
                }
            }
            else
            {
                if (!external.TryGetValue(baseType, out baseEmptyConstructor))
                {
                    var emptyConstructor = baseType.Resolve().GetEmptyConstructor();
                    if (emptyConstructor != null)
                    {
                        baseEmptyConstructor = ModuleDefinition.Import(emptyConstructor);
                    }
                    external.Add(baseType, baseEmptyConstructor);
                }
            }

            if (baseEmptyConstructor != null)
            {
                if (baseEmptyConstructor.Resolve().IsPrivate)
                {
                    processed.Add(type, null);
                    LogWarning(string.Format("Could not inject empty constructor in {0} because the base class has a private parameterless constructor", type.FullName));
                    continue;
                }
                var constructor = AddEmptyConstructor(type, baseEmptyConstructor);
               processed.Add(type, constructor);
            }
            else
            {
                processed.Add(type, null);   
            }

        }
    }

    MethodDefinition AddEmptyConstructor(TypeDefinition type, MethodReference baseEmptyConstructor)
    {
        LogInfo("Processing " + type.FullName);
        var methodAttributes = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName;
        var method = new MethodDefinition(".ctor", methodAttributes, ModuleDefinition.TypeSystem.Void);
        foreach (var instruction in GetFieldInitializations(type))
        {
            var cloned = instruction.Clone();
            method.Body.Instructions.Add(cloned);
        }
        method.Body.Instructions.Add(Instruction.Create(OpCodes.Call, baseEmptyConstructor));
        method.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
        type.Methods.Add(method);
        return method;
    }


    static IEnumerable<Instruction> GetFieldInitializations(TypeDefinition type)
    {
        var list = new List<Instruction>();
        var otherConstructor = type.Methods.First(x => x.IsConstructor && !x.IsStatic);
        foreach (var instruction in otherConstructor.Body.Instructions)
        {
            var isBaseConstructorCall = IsBaseConstructorCall(type, instruction);
            if (isBaseConstructorCall)
            {
                break;
            }
            list.Add(instruction);
        }
        for (var i = list.Count-1; i >= 0; i--)
        {
            var instruction = list[i];
            if (instruction.OpCode == OpCodes.Ldarg_0)
            {
                break;
            }
            list.RemoveAt(i);
        }
        return list;
    }

    static bool IsBaseConstructorCall(TypeDefinition type, Instruction instruction)
    {
        if (instruction.OpCode == OpCodes.Call)
        {
            var methodReference = instruction.Operand as MethodReference;
            if (methodReference != null)
            {
                if (methodReference.Name == ".ctor")
                {
                    if (methodReference.DeclaringType == type.BaseType)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
}