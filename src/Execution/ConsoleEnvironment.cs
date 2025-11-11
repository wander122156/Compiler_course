using System.Globalization;
using System.Text;

using Blang.Common;

namespace Blang.Execution;

public class ConsoleEnvironment : IEnvironment
{
    public void AddResult(RuntimeValue result)
    {
        // Console.Write("Result: " + result.Value.ToString(CultureInfo.InvariantCulture));
        Console.WriteLine(" @Result: " + result.Value);
    }

    public void Write(RuntimeValue value)
    {
        switch (value.Type)
        {
            case RuntimeValue.ValueType.Number:
                Console.Write((decimal)value.Value);
                break;
            case RuntimeValue.ValueType.String:
                Console.Write((string)value.Value);
                break;

            default:
                throw new InvalidOperationException($"Unsupported type for write: {value.Type}");
        }
    }

    public void Writeln(RuntimeValue value)
    {
        switch (value.Type)
        {
            case RuntimeValue.ValueType.Null:
                Console.WriteLine();
                break;
            case RuntimeValue.ValueType.Number:
                Console.WriteLine((decimal)value.Value);
                break;
            case RuntimeValue.ValueType.String:
                Console.WriteLine((string)value.Value);
                break;

            default:
                throw new InvalidOperationException($"Unsupported type for writeln: {value.Type}");
        }
    }

    public RuntimeValue Read()
    {
        StringBuilder input = new();

        ConsoleKeyInfo key;
        while ((key = Console.ReadKey(intercept: true)).Key != ConsoleKey.Enter
                && key.Key != ConsoleKey.Spacebar
                && key.Key != ConsoleKey.Tab)
        {
            input.Append(key.KeyChar);
            Console.Write(key.KeyChar);
        }

        string result = input.ToString();

        if (decimal.TryParse(result, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal number))
        {
            return RuntimeValue.Number(number);
        }
        else
        {
            // по дефолту читаем как строку
            return new RuntimeValue(RuntimeValue.ValueType.String, input);
        }

        // else остальные типы
    }

    public RuntimeValue Readln()
    {
        string input = Console.ReadLine() ?? string.Empty;

        if (decimal.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal number))
        {
            return RuntimeValue.Number(number);
        }
        else
        {
            // по дефолту читаем как строку
            return new RuntimeValue(RuntimeValue.ValueType.String, input);
        }

        // else остальные типы
    }
}