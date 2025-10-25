namespace Parser;

public class Row
{
    private readonly RuntimeValue[] _values;

    public Row(params RuntimeValue[] values)
    {
        _values = values;
    }

    public int ColumnCount => _values.Length;

    public RuntimeValue this[int index]
    {
        get => _values[index];
    }
}

public class RuntimeValue
{
    public ValueType Type { get; }

    public object Value { get; }

    public RuntimeValue(ValueType type, object value)
    {
        Type = type;
        Value = value;
    }

    public enum ValueType
    {
        Number,
        String,
        Boolean,
        Null
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