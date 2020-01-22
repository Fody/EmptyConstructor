using System.Collections.Generic;
using System.Linq;
using Fody;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

public partial class ModuleWeaver:BaseModuleWeaver
{
    public MethodAttributes Visibility = MethodAttributes.Public;
    public bool MakeExistingEmptyConstructorsVisible;
    public bool PreserveInitializers = true;

    public override void Execute()
    {
        ReadConfig();
        ProcessIncludesExcludes();


        var internalTypes = ModuleDefinition.GetTypes().ToList();
        var queue = new Queue<TypeDefinition>(internalTypes);

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
                MakeConstructorVisibleIfConfiguredAndNecessary(typeEmptyConstructor);

                processed.Add(type, typeEmptyConstructor);
                continue;
            }

            if (!ShouldIncludeType(type))
            {
                continue;
            }

            MethodReference baseEmptyConstructor;
            var baseTypeDefinition = baseType.Resolve();
            if (!internalTypes.Contains(baseTypeDefinition))
            {
                if (!external.TryGetValue(baseType, out baseEmptyConstructor))
                {
                    var emptyConstructor = baseTypeDefinition.GetEmptyConstructor();
                    if (emptyConstructor != null)
                    {
                        baseEmptyConstructor = ModuleDefinition.ImportReference(emptyConstructor);
                    }
                    external.Add(baseType, baseEmptyConstructor);
                }

                if (baseEmptyConstructor == null)
                {
                    processed.Add(type, null);
                    LogDebug($"Could not inject empty constructor in {type.FullName} because base class does not have a parameterless constructor and is from an external assembly");
                    continue;
                }
            }
            else
            {
                if (!processed.TryGetValue(baseTypeDefinition, out baseEmptyConstructor))
                {
                    queue.Enqueue(type);
                    continue;
                }
            }

            if (baseEmptyConstructor.Resolve().IsPrivate)
            {
                processed.Add(type, null);
                LogWarning($"Could not inject empty constructor in {type.FullName} because the base class has a private parameterless constructor");
            }
            else
            {
                processed.Add(type, AddEmptyConstructor(type));
            }
        }
    }

    public override bool ShouldCleanReference => true;

    public override IEnumerable<string> GetAssembliesForScanning()
    {
        return Enumerable.Empty<string>();
    }

    MethodDefinition AddEmptyConstructor(TypeDefinition type)
    {
        LogDebug("Processing " + type.FullName);

        var methodAttributes = Visibility | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName;
        var method = new MethodDefinition(".ctor", methodAttributes, TypeSystem.VoidReference);

        TryInjectPropertyOrFieldInitializers(type, method);
        
        method.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
        var methodReference = new MethodReference(".ctor", TypeSystem.VoidReference, type.BaseType){HasThis = true};
        method.Body.Instructions.Add(Instruction.Create(OpCodes.Call, methodReference));
        method.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
        type.Methods.Add(method);
        return method;
    }

    void TryInjectPropertyOrFieldInitializers(TypeDefinition type, MethodDefinition method)
    {
        if (!PreserveInitializers)
        {
            return;
        }

        var existingConstructor = type.GetConstructors().FirstOrDefault();
        if (existingConstructor != null) // any constructor exists - try to clone its initializers part (should be before calling base.ctor())
        {
            var initializerInstructions = existingConstructor
                .Body
                .Instructions
                .TakeWhile(i => !IsBaseConstructorCallInstruction(i, type))
                .Reverse() // cut also instructions preparing constructor call
                .SkipWhile(i => !IsLdArg0Instruction(i)) // first of them should be ldarg.0
                .Skip(1) // skip also ldarg.0
                .Reverse();

            foreach (var instruction in initializerInstructions)
            {
                method.Body.Instructions.Add(instruction);
            }
        }
    }

    static bool IsLdArg0Instruction(Instruction instruction)
    {
        return instruction.OpCode == OpCodes.Ldarg_0;
    }

    static bool IsBaseConstructorCallInstruction(Instruction instruction, TypeDefinition type)
    {
        return instruction.OpCode == OpCodes.Call
               && instruction.Operand is MethodReference methodReference
               && methodReference.DeclaringType == type.BaseType
               && methodReference.Name == ".ctor";
    }

    void MakeConstructorVisibleIfConfiguredAndNecessary(MethodDefinition typeEmptyConstructor)
    {
        if (!MakeExistingEmptyConstructorsVisible)
        {
            return;
        }
        if (typeEmptyConstructor.IsPublic)
        {
            return;
        }
        if (typeEmptyConstructor.DeclaringType.IsAbstract)
        {
            return;
        }
        if (typeEmptyConstructor.IsFamily)
        {
            if (Visibility == MethodAttributes.Public)
            {
                typeEmptyConstructor.IsFamily = false;
                typeEmptyConstructor.IsPublic = true;
            }
            return;
        }
        if (typeEmptyConstructor.IsPrivate)
        {
            typeEmptyConstructor.IsPrivate = false;
            typeEmptyConstructor.Attributes = typeEmptyConstructor.Attributes | Visibility;
        }
    }
}