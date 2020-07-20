public class Bug143Child : Bug143Base<int>
{
    #pragma warning disable CS0414
    bool value;

    private Bug143Child(int enumValue)
        : base(enumValue)
    {
        value = true;
    }
}

public abstract class Bug143Base<TValue>
{
    protected Bug143Base(TValue value)
    {
    }
}