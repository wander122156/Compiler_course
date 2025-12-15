using Blang.Common;
using Blang.Execution;
using Blang.Interpreter;

namespace Blang.Parser.UnitTests;

public class ParserExpressionsTests
{
    private const int Precision = 5;

    private readonly Context _context;
    private readonly FakeEnvironment _environment;

    public ParserExpressionsTests()
    {
        _context = new Context();
        _environment = new FakeEnvironment();
    }

    public static TheoryData<string, decimal> GetParseTestData()
    {
        return new TheoryData<string, decimal>
        {
            // Арифметические операции с приоритетами
            { "1 + 2 * 3", 7m },
            { "2 + 3 * 4 % 2 - 2.5", -0.5m },
            { "10 - 3 - 2", 5m },

            // Унарные операции
            { "+5 + -4", 1m },
            { "--5", 5m },
            { "-5 + 3 * -2", -11m },

            // Скобки
            { "(1 + 2) * 3", 9m },
            { "((1 + 2) * (3 - 1)) - 2", 4m },

            // Встроенные константы
            { "Pi", (decimal)Math.PI },
            { "MathE * 1", (decimal)Math.E },

            // Встроенные функции
            { "min(7, 10 - 4, 8)", 6m },
            { "max(1, 3)", 3m },
            { "abs(-4)", 4m },
            { "pow(5, 3)", 125m },
            { "length(\"four\")", 4m },
            { "length(\"\")", 0m },
        };
    }

    [Theory]
    [MemberData(nameof(GetParseTestData))]
    public void Can_parse_expressions(string expression, decimal expected)
    {
        string code = $"num result = {expression}; write(result)";

        BlangInterpreter blang = new(_environment);
        blang.Execute(code);

        RuntimeValue result = Assert.Single(_environment.Results);
        Assert.Equal(expected, (decimal)result, Precision);
    }
}