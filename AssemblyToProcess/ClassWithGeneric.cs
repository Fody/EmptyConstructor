public class ClassWithGeneric<T>
{
    protected ClassWithGeneric(T x)
    {
    }
}

public class ClassInheritWithGeneric : ClassWithGeneric<string>
{
    // ReSharper disable once UnusedParameter.Local
    public ClassInheritWithGeneric(string x)
    : base(x)
    {
    }
}

public class ClassWithGenericInheritWithGeneric<T> : ClassWithGeneric<T>
{
    protected ClassWithGenericInheritWithGeneric(T x)
    : base(x)
    {
    }
}

public class ClassInheritWithGenericInheritWithGeneric : ClassWithGenericInheritWithGeneric<string>
{
    public ClassInheritWithGenericInheritWithGeneric(string x)
    : base(x)
    {
    }
}