namespace Blang.Common;
public static class BuiltinFunctions
{
    private static readonly Dictionary<string, Func<List<RuntimeValue>, RuntimeValue>> Functions = new()
    {
        { "abs", Abs },
        { "min", Min },
        { "max", Max },
        { "pow", Pow },
        { "floor", Floor },
        { "length", Length },
    };

    public static bool IsBuiltin(string name)
    {
        return Functions.ContainsKey(name);
    }

    public static RuntimeValue Invoke(string name, List<RuntimeValue> arguments)
    {
        if (!Functions.TryGetValue(name, out Func<List<RuntimeValue>, RuntimeValue>? function))
        {
            throw new ArgumentException($"Unknown builtin function {name}");
        }

        return function(arguments);
    }

    private static RuntimeValue Abs(List<RuntimeValue> arguments)
    {
        if (arguments.Count != 1)
        {
            throw new ArgumentException("ABS function requires 1 argument");
        }

        return RuntimeValue.Number(Math.Abs((decimal)arguments[0]));
    }

    private static RuntimeValue Min(List<RuntimeValue> arguments)
    {
        if (arguments.Count == 0)
        {
            throw new ArgumentException("MIN function requires at least 1 argument");
        }

        decimal min = (decimal)arguments[0];
        for (int i = 1; i < arguments.Count; i++)
        {
            decimal current = (decimal)arguments[i];
            if (current < min) min = current;
        }

        return RuntimeValue.Number(min);
    }

    private static RuntimeValue Max(List<RuntimeValue> arguments)
    {
        if (arguments.Count == 0)
        {
            throw new ArgumentException("MAX function requires at least 1 argument");
        }

        decimal max = (decimal)arguments[0];
        for (int i = 1; i < arguments.Count; i++)
        {
            decimal current = (decimal)arguments[i];
            if (current > max) max = current;
        }

        return RuntimeValue.Number(max);
    }

    private static RuntimeValue Pow(List<RuntimeValue> arguments)
    {
        if (arguments.Count != 2)
        {
            throw new ArgumentException("POW function requires 2 arguments");
        }

        double baseValue = Convert.ToDouble(arguments[0].Value);
        double exponentValue = Convert.ToDouble(arguments[1].Value);
        double result = Math.Pow(baseValue, exponentValue);

        return RuntimeValue.Number(Convert.ToDecimal(result));
    }

    private static RuntimeValue Floor(List<RuntimeValue> arguments)
    {
        if (arguments.Count != 1)
        {
            throw new ArgumentException("FLOOR function requires 1 argument");
        }

        return RuntimeValue.Number(Math.Floor((decimal)arguments[0]));
    }

    private static RuntimeValue Length(List<RuntimeValue> arguments)
    {
        if (arguments.Count != 1)
        {
            throw new ArgumentException("LENGTH function requires 1 argument");
        }

        RuntimeValue arg = arguments[0];

        string str = (string)arg;
        return RuntimeValue.Number(str.Length);
    }
}