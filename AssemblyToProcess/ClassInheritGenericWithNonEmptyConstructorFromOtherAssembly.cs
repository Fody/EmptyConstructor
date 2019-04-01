public class ClassInheritGenericWithNonEmptyConstructorFromOtherAssembly : AssemblyToReference.GenericClassWithNonEmptyConstructor<int>
{
    public ClassInheritGenericWithNonEmptyConstructorFromOtherAssembly(int x)
        : base(x)
    {
    }
}