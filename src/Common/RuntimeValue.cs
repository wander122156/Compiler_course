namespace Blang.Common;

public class RuntimeValue
{
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
        Null,
        Undefined,
    }

    public ValueType Type { get; }

    public object Value { get; }

    public static RuntimeValue Number(decimal value)
        => new RuntimeValue(ValueType.Number, value);

    public static RuntimeValue String(string value)
        => new RuntimeValue(ValueType.String, value);

    public static RuntimeValue Boolean(bool value)
        => new RuntimeValue(ValueType.Boolean, value);

    public static RuntimeValue Null()
        => new RuntimeValue(ValueType.Null, null!);

    public static RuntimeValue Undefined()
        => new RuntimeValue(ValueType.Undefined, null!);

    public static explicit operator decimal(RuntimeValue v)
    {
        throw new NotImplementedException();
    }
}