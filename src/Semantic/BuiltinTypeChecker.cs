using ValueType = Blang.Common.RuntimeValue.ValueType;

namespace Blang.Semantic;

public static class BuiltinTypeChecker
{
    public record FunctionTypeInfo(ValueType ReturnType, List<ValueType> ParameterTypes);

    private static readonly Dictionary<string, FunctionTypeInfo> TypeInfo = new()
    {
        // ТОЛЬКО математические функции
        { "abs", new FunctionTypeInfo(ValueType.Number, new List<ValueType> { ValueType.Number }) },
        { "min", new FunctionTypeInfo(ValueType.Number, new List<ValueType> { ValueType.Number, ValueType.Number }) },
        { "max", new FunctionTypeInfo(ValueType.Number, new List<ValueType> { ValueType.Number, ValueType.Number }) },
        { "pow", new FunctionTypeInfo(ValueType.Number, new List<ValueType> { ValueType.Number, ValueType.Number }) },
        { "floor", new FunctionTypeInfo(ValueType.Number, new List<ValueType> { ValueType.Number }) },
        { "sqrt", new FunctionTypeInfo(ValueType.Number, new List<ValueType> { ValueType.Number }) },
        { "sin", new FunctionTypeInfo(ValueType.Number, new List<ValueType> { ValueType.Number }) },
        { "cos", new FunctionTypeInfo(ValueType.Number, new List<ValueType> { ValueType.Number }) },
    };

    public static bool IsBuiltin(string name) => TypeInfo.ContainsKey(name);

    public static FunctionTypeInfo GetTypeInfo(string name)
    {
        if (TypeInfo.TryGetValue(name, out FunctionTypeInfo? info))
            return info;

        throw new TypeException($"Unknown builtin function: {name}");
    }
}