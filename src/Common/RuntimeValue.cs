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
        /// <summary>
        /// Представляет числовое значение (decimal)
        /// </summary>
        Number,

        /// <summary>
        /// Представляет строковое значение
        /// </summary>
        String,

        /// <summary>
        /// Представляет логическое значение (true/false)
        /// </summary>
        Boolean,

        /// <summary>
        /// Представляет неопределенное значение (неинициализированная переменная)
        /// </summary>
        Undefined,

        /// <summary>
        /// Представляет символ новой строки (используется для вывода writeln)
        /// </summary>
        NewLine,

        /// <summary>
        /// Представляет символ новой строки (используется для вывода writeln)
        /// </summary>
        Void,
    }

    public ValueType Type { get; }

    public object Value { get; }

    public static RuntimeValue Number(decimal value)
        => new RuntimeValue(ValueType.Number, value);

    public static RuntimeValue String(string value)
        => new RuntimeValue(ValueType.String, value);

    public static RuntimeValue Boolean(bool value)
        => new RuntimeValue(ValueType.Boolean, value);

    public static RuntimeValue Undefined()
        => new RuntimeValue(ValueType.Undefined, null!);

    public static RuntimeValue NewLine()
    => new RuntimeValue(ValueType.NewLine, null!);

    public static RuntimeValue Void()
        => new RuntimeValue(ValueType.Void, null!);

    public static explicit operator decimal(RuntimeValue value)
    {
        if (value.Type != ValueType.Number)
        {
            throw new InvalidCastException($"Cannot convert {value.Type} to decimal");
        }

        return (decimal)value.Value;
    }

    public static explicit operator string(RuntimeValue value)
    {
        if (value.Type != ValueType.String)
        {
            throw new InvalidCastException($"Cannot convert {value.Type} to string");
        }

        return (string)value.Value;
    }

    public static explicit operator bool(RuntimeValue value)
    {
        if (value.Type != ValueType.Boolean)
        {
            throw new InvalidCastException($"Cannot convert {value.Type} to bool");
        }

        return (bool)value.Value;
    }

    /// <summary>
    /// Вовращает значение преобразованное к bool.
    /// </summary>
    public bool ConvertToBoolean()
    {
        return Type switch
        {
            ValueType.Boolean => (bool)Value,
            ValueType.Number => (decimal)Value != 0,
            ValueType.String => !string.IsNullOrEmpty((string)Value),
            _ => false,
        };
    }
}