namespace Blang.Execution;

/// <summary>
/// Контекст выполнения программы (все переменные, константы и другие символы).
/// </summary>
public class Context
{
    private readonly Stack<Scope> _scopes = [];
    private readonly Dictionary<string, decimal> _constants = [];

    public void PushScope(Scope scope)
    {
        _scopes.Push(scope);
    }

    public void PopScope()
    {
        _scopes.Pop();
    }

    /// <summary>
    /// Возвращает значение переменной или константы.
    /// </summary>
    public decimal GetValue(string name)
    {
        // foreach (Scope s in _scopes.Reverse())
        foreach (Scope s in _scopes)
        {
            if (s.TryGetVariable(name, out decimal variable))
            {
                return variable;
            }
        }

        if (_constants.TryGetValue(name, out decimal constant))
        {
            return constant;
        }

        throw new ArgumentException($"Variable '{name}' is not defined");
    }

    /// <summary>
    /// Присваивает (изменяет) значение переменной.
    /// </summary>
    public void AssignVariable(string name, decimal value)
    {
        // foreach (Scope s in _scopes.Reverse())
        foreach (Scope s in _scopes)
        {
            if (s.TryAssignVariable(name, value))
            {
                return;
            }
        }

        throw new ArgumentException($"Variable '{name}' is not defined");
    }

    /// <summary>
    /// Определяет переменную в текущей области видимости.
    /// </summary>
    public void DefineVariable(string name, decimal value)
    {
        if (!_scopes.Peek().TryDefineVariable(name, value))
        {
            throw new ArgumentException($"Variable '{name}' is already defined in this scope");
        }
    }

    /// <summary>
    /// Определяет константу в глобальной области видимости.
    /// </summary>
    public void DefineConstant(string name, decimal value)
    {
        if (!_constants.TryAdd(name, value))
        {
            throw new ArgumentException($"Constant '{name}' is already defined");
        }
    }
}