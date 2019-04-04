// This is a repro for #42 "Class Sequence breaks adding Empty Constructor to Class inheriting from Generic Class" (https://github.com/Fody/EmptyConstructor/issues/42)
// WARNING: do not change the sequence of the classes in this file as the sequence is critical (it determines the sequence the weavers processes classes).
// Also, do not extract a class from this file.
public class ClassInheritWithGenericInReverseDeclarationOrder : ClassWithGenericInReverseDeclarationOrder<string>
{
    public ClassInheritWithGenericInReverseDeclarationOrder(string x)
    : base(x)
    {
    }
}

public class ClassWithGenericInReverseDeclarationOrder<T>
{
    protected ClassWithGenericInReverseDeclarationOrder(T x)
    {
    }
}