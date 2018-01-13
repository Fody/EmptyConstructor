public class ClassWithGeneric<T>
{
}

public class ClassInheritWithGeneric : ClassWithGeneric<string>
{
// ReSharper disable once UnusedParameter.Local
    public ClassInheritWithGeneric(string x)
    {
    }
}