using Blang.Common;

using ValueType = Blang.Common.RuntimeValue.ValueType;

namespace Blang.Semantic;
public class TypeContext
{
    private readonly Stack<Dictionary<string, ValueType>> _scopes = new();
    private readonly Dictionary<string, FunctionInfo> _functions = new();

    public record FunctionInfo(
        ValueType returnType,
        List<(string Name, ValueType Type)> parameters
    );

    public TypeContext()
    {
        // Глобальная область видимости
        _scopes.Push([]);
    }

    public void PushScope()
    {
        _scopes.Push(new Dictionary<string, ValueType>());
    }

    public void PopScope()
    {
        _scopes.Pop();
    }

    public void DefineVariableType(string name, ValueType type)
    {
        if (_scopes.Peek().ContainsKey(name))
            throw new TypeException($"Variable '{name}' is already defined");

        _scopes.Peek()[name] = type;
    }

    public ValueType GetVariableType(string name)
    {
        foreach (Dictionary<string, ValueType> scope in _scopes)
        {
            if (scope.TryGetValue(name, out ValueType type))
                return type;
        }

        throw new ArgumentException($"Variable '{name}' is not defined");
    }

    public void DefineFunction(
        string name,
        ValueType returnType,
        List<(string, ValueType)> parameters)
    {
        if (_functions.ContainsKey(name))
        {
            throw new ArgumentException($"Function '{name}' is already defined");
        }

        _functions[name] = new FunctionInfo(returnType, parameters);
    }

    public FunctionInfo GetFunctionInfo(string name)
    {
        if (_functions.TryGetValue(name, out FunctionInfo? info))
            return info;

        throw new TypeException($"Function '{name}' is not defined");
    }

    public bool HasFunction(string name) => _functions.ContainsKey(name);
}
