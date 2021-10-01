#if NET5_0
public record RecordWithParameterAndInitializedField(string Parameter)
{
    public int X = 9;
}
#endif