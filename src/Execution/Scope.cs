using Blang.Common;

namespace Blang.Execution;

public class Scope
{
    private readonly Dictionary<string, RuntimeValue> _variables = [];

    /// <summary>
    /// Читает переменную из этой области видимости.
    /// Возвращает false, если переменная не объявлена в этой области видимости.
    /// </summary>
    public bool TryGetVariable(string name, out RuntimeValue? value)
    {
        if (_variables.TryGetValue(name, out RuntimeValue? v))
        {
            value = v;
            return true;
        }

        value = RuntimeValue.Undefined();

        return false;
    }

    /// <summary>
    /// Присваивает переменную в этой области видимости.
    /// Возвращает false, если переменная не объявлена в этой области видимости.
    /// </summary>
    public bool TryAssignVariable(string name, RuntimeValue value)
    {
        if (_variables.ContainsKey(name))
        {
            _variables[name] = value;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Объявляет переменную в этой области видимости.
    /// Возвращает false, если переменная уже объявлена в этой области видимости.
    /// </summary>
    public bool TryDefineVariable(string name, RuntimeValue value)
    {
        return _variables.TryAdd(name, value);
    }
}