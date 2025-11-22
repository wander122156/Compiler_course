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
        /// Представляет нулевое значение (отсутствие значения)
        /// </summary>
        Null,

        /// <summary>
        /// Представляет неопределенное значение (неинициализированная переменная)
        /// </summary>
        Undefined,

        /// <summary>
        /// Представляет символ новой строки (используется для вывода writeln)
        /// </summary>
        NewLine,
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

    public static RuntimeValue NewLine()
    => new RuntimeValue(ValueType.NewLine, null!);

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

    public override string ToString()
    {
        return Type switch
        {
            ValueType.Number => $"Number:{Value}",
            ValueType.String => $"String:{Value}",
            ValueType.Boolean => $"Boolean:{Value}",
            ValueType.NewLine => "NewLine",
            ValueType.Null => "Null",
            ValueType.Undefined => "Undefined",
            _ => $"Unknown:{Type}",
        };
    }
}