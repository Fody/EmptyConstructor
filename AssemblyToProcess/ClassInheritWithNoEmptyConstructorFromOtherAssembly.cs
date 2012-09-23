public class ClassInheritWithNoEmptyConstructorFromOtherAssembly : AssemblyToReference.ClassWithNoEmptyConstructor
{
    public ClassInheritWithNoEmptyConstructorFromOtherAssembly(int x)
        : base(x)
    {
    }
}