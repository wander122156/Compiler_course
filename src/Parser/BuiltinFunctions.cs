namespace Parser;
public static class BuiltinFunctions
{
    private static readonly Dictionary<string, Func<List<decimal>, decimal>> Functions = new()
    {
        {
            "abs", Abs
        },
        {
            "min", Min
        },
        {
            "max", Max
        },
        {
            "pow", Pow
        },
    };

    public static decimal Invoke(string name, List<decimal> arguments)
    {
        if (!Functions.TryGetValue(name, out Func<List<decimal>, decimal>? function))
        {
            throw new ArgumentException($"Unknown builtin function {name}");
        }

        return function(arguments);
    }

    private static decimal Abs(List<decimal> arguments)
    {
        if (arguments.Count != 1)
        {
            throw new ArgumentException("ABS function requires 1 argument");
        }

        return Math.Abs(arguments[0]);
    }

    private static decimal Min(List<decimal> arguments)
    {
        return arguments.Min();
    }

    private static decimal Max(List<decimal> arguments)
    {
        return arguments.Max();
    }

    private static decimal Pow(List<decimal> arguments)
    {
        if (arguments.Count != 2)
        {
            throw new ArgumentException("POW function requires 2 arguments");
        }

        double baseValue = (double)arguments[0];
        double exponentValue = (double)arguments[1];
        double result = Math.Pow(baseValue, exponentValue);
        return (decimal)result;
    }
}
