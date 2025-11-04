using Blang.Common;

namespace Blang.Parser;

/// <summary>
/// Поддельное окружение: работает как настоящее, но не совершает реального ввода/вывода.
/// </summary>
public class FakeEnvironment : IEnvironment
{
    private readonly List<RuntimeValue> _results = [];

    public IReadOnlyList<RuntimeValue> Results => _results;

    public void AddResult(RuntimeValue result)
    {
        _results.Add(result);
    }
}
