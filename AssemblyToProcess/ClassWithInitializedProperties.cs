public class ClassWithInitializedProperties : ClassWithNoEmptyConstructor
{
    public int X { get; } = 9;
    public string Y { get; } = "aString";
    public Simple Z { get; } = new Simple(1);

    public ClassWithInitializedProperties(string x)
        : base(x)
    {
    }
}