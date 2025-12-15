using ValueType = Blang.Common.RuntimeValue.ValueType;

namespace Blang.Semantic;

public static class BuiltinTypeChecker
{
    public record FunctionTypeInfo(ValueType returnType, List<ValueType> parameterTypes, int minArguments = -1);

    private static readonly Dictionary<string, FunctionTypeInfo> TypeInfo = new()
    {
        // ТОЛЬКО математические функции
        { "abs", new FunctionTypeInfo(ValueType.Number, new List<ValueType> { ValueType.Number }) },
        { "min", new FunctionTypeInfo(ValueType.Number, new List<ValueType> { ValueType.Number, ValueType.Number }, minArguments: 1) },
        { "max", new FunctionTypeInfo(ValueType.Number, new List<ValueType> { ValueType.Number, ValueType.Number }, minArguments: 1) },
        { "pow", new FunctionTypeInfo(ValueType.Number, new List<ValueType> { ValueType.Number, ValueType.Number }) },
        { "floor", new FunctionTypeInfo(ValueType.Number, new List<ValueType> { ValueType.Number }) },
        { "sqrt", new FunctionTypeInfo(ValueType.Number, new List<ValueType> { ValueType.Number }) },
        { "sin", new FunctionTypeInfo(ValueType.Number, new List<ValueType> { ValueType.Number }) },
        { "cos", new FunctionTypeInfo(ValueType.Number, new List<ValueType> { ValueType.Number }) },
        { "length", new FunctionTypeInfo(ValueType.Number, new List<ValueType> { ValueType.String }) },
    };

    public static bool IsBuiltin(string name) => TypeInfo.ContainsKey(name);

    public static FunctionTypeInfo GetTypeInfo(string name)
    {
        if (TypeInfo.TryGetValue(name, out FunctionTypeInfo? info))
            return info;

        throw new TypeException($"Unknown builtin function: {name}");
    }

    public static bool CheckArgumentCount(string name, int argCount)
    {
        if (!TypeInfo.TryGetValue(name, out FunctionTypeInfo? info))
            return false;

        // Если указан MinArguments, проверяем минимальное количество
        if (info.minArguments > 0 && argCount < info.minArguments)
            return false;

        // Если не указан Min, проверяем точное совпадение
        if (info.minArguments == -1)
            return argCount == info.parameterTypes.Count;

        return true;
    }
}