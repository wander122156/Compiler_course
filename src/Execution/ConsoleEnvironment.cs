using System.Globalization;

using Blang.Common;

namespace Blang.Execution;

public class ConsoleEnvironment : IEnvironment
{
    public void AddResult(RuntimeValue result)
    {
        // Console.Write("Result: " + result.Value.ToString(CultureInfo.InvariantCulture));
        Console.Write("Result: " + result.Value);
    }
}