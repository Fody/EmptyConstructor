public class ClassWithNullableParam
{
    // ReSharper disable once UnusedParameter.Local
    public ClassWithNullableParam(string x = null) { }
}

public class ClassInheritWithNullableParam : ClassWithNullableParam
{
    // ReSharper disable once UnusedParameter.Local
    public ClassInheritWithNullableParam(string x)
    {
    }
}