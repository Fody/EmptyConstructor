public class ClassWithInitializedFields : ClassWithNoEmptyConstructor
{
    public int X = 9;
    public string Y = "aString";
    public Simple Z = new(1);

    public ClassWithInitializedFields(string x)
        : base(x)
    {
    }
}