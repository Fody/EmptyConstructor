// ReSharper disable NotAccessedField.Local
public class ClassWithInnerClass
{
    // ReSharper disable once UnusedParameter.Local
    public ClassWithInnerClass(int x) { }
    public ClassWithInnerClass() { }

// ReSharper disable once UnusedMember.Local
    class InnerClass
    {
        readonly int x;

        public InnerClass(int x)
        {
            this.x = x;
        }
    }
}