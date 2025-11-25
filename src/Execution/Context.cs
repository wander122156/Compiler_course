using Blang.Ast.Declarations;

namespace Blang.Execution;

/// <summary>
/// Контекст выполнения программы (все переменные, константы и другие символы).
/// </summary>
public class Context
{
    private readonly Stack<Scope> _scopes = [];
    private readonly Dictionary<string, decimal> _constants = [];
    private readonly Dictionary<string, FunctionDeclaration> _functions = [];

    public void PushScope(Scope scope)
    {
        _scopes.Push(scope);
    }

    public void PopScope()
    {
        _scopes.Pop();
    }

    /// <summary>
    /// Регистрирует пользовательскую функцию.
    /// </summary>
    public void DefineFunction(string name, FunctionDeclaration function)
    {
        if (_functions.ContainsKey(name))
        {
            throw new ArgumentException($"Function '{name}' is already defined");
        }

        _functions[name] = function;
    }

    /// <summary>
    /// Проверяет, существует ли функция с указанным именем.
    /// </summary>
    public bool HasFunction(string name)
    {
        return _functions.ContainsKey(name);
    }

    /// <summary>
    /// Получает пользовательскую функцию по имени.
    /// </summary>
    public FunctionDeclaration GetFunction(string name)
    {
        if (_functions.ContainsKey(name))
        {
            return _functions[name];
        }

        throw new ArgumentException($"Function '{name}' is not defined");
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