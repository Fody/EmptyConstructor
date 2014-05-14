public class ClassInheritWithNonEmptyConstructor : ClassWithNoEmptyConstructor
{
    public ClassInheritWithNonEmptyConstructor(string x)
        : base(x)
    {
    }
}