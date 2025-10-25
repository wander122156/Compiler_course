namespace Parser;


public class Row
{
    private readonly decimal[] _values;

    public Row(params decimal[] values)
    {
        _values = values;
    }

    public int ColumnCount => _values.Length;

    public decimal this[int index]
    {
        get => _values[index];
    }

    public static RuntimeValue Number(decimal value)
        => new RuntimeValue(ValueType.Number, value);

    public static RuntimeValue String(string value)
        => new RuntimeValue(ValueType.String, value);

    public static RuntimeValue Boolean(bool value)
        => new RuntimeValue(ValueType.Boolean, value);

    public static RuntimeValue Null()
        => new RuntimeValue(ValueType.Null, null);
}