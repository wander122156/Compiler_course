using System.Globalization;

using Blang.Common;

namespace Blang.Parser;

/// <summary>
/// Поддельное окружение: работает как настоящее, но не совершает реального ввода/вывода.
/// </summary>
public class FakeEnvironment : IEnvironment
{
    private readonly List<RuntimeValue> _results = [];

    private readonly Queue<string> _simulatedInput = new();

    public IReadOnlyList<RuntimeValue> Results => _results;

    public void SetSimulatedInput(params string[] inputs)
    {
        _simulatedInput.Clear();
        foreach (string input in inputs)
        {
            _simulatedInput.Enqueue(input);
        }
    }

    public void AddResult(RuntimeValue result)
    {
        _results.Add(result);
    }

    public void Write(RuntimeValue value)
    {
        // _results.Add(value);
    }

    public void Writeln(RuntimeValue value)
    {
        // _results.Add(value);
    }

    public RuntimeValue Read()
    {
        if (_simulatedInput.Count == 0)
        {
            // Если ввод не настроен, возвращаем значение по умолчанию
            RuntimeValue defaultValue = RuntimeValue.Number(0);
            return defaultValue;
        }

        string input = _simulatedInput.Dequeue();

        if (decimal.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal number))
        {
            return RuntimeValue.Number(number);
        }
        else
        {
            return RuntimeValue.String(input);
        }
    }

    public RuntimeValue Readln()
    {
        return Read();
    }
}
