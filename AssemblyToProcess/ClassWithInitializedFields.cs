public class ClassWithInitializedFields : ClassWithNonEmptyConstructor
{
    public int X = 9;
    public string Y = "aString";
    public Simple Z = new Simple(1);

    public ClassWithInitializedFields(int x)
        : base(x)
    {
    }
}