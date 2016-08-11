using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;

public partial class ModuleWeaver
{
    public Action<string> LogDebug { get; set; }
    public Action<string> LogWarning { get; set; }
    public ModuleDefinition ModuleDefinition { get; set; }
    public IAssemblyResolver AssemblyResolver { get; set; }
    public MethodAttributes Visibility = MethodAttributes.Public;

    public ModuleWeaver()
    {
        LogDebug = s => { };
        LogWarning = s => { };
    }

    public void Execute()
    {
        ReadConfig();
        ProcessIncludesExcludes();
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
            if (type.IsStaticClass())
            {
                continue;
            }
            if (Visibility == MethodAttributes.Family && type.IsSealed)
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

            if (!ShouldIncludeType(type))
            {
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
                        baseEmptyConstructor = ModuleDefinition.ImportReference(emptyConstructor);
                    }
                    external.Add(baseType, baseEmptyConstructor);
                }
            }

            if (baseEmptyConstructor != null)
            {
                if (baseEmptyConstructor.Resolve().IsPrivate)
                {
                    processed.Add(type, null);
                    LogWarning($"Could not inject empty constructor in {type.FullName} because the base class has a private parameterless constructor");
                    continue;
                }
                var constructor = AddEmptyConstructor(type);
                processed.Add(type, constructor);
            }
            else
            {
                processed.Add(type, null);
            }

        }
    }

    MethodDefinition AddEmptyConstructor(TypeDefinition type)
    {
        LogDebug("Processing " + type.FullName);
        var methodAttributes = Visibility | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName;
        var method = new MethodDefinition(".ctor", methodAttributes, ModuleDefinition.TypeSystem.Void);
        method.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
        var methodReference = new MethodReference(".ctor",ModuleDefinition.TypeSystem.Void,type.BaseType){HasThis = true};
        method.Body.Instructions.Add(Instruction.Create(OpCodes.Call, methodReference));
        method.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
        type.Methods.Add(method);
        return method;
    }


}