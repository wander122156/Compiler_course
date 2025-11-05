using Blang.Common;
using Blang.Execution;

namespace Blang.Parser.UnitTests;

public class ParserExpressionsTest
{
    private const int Precision = 5;

    private readonly Context _context;
    private readonly FakeEnvironment _environment;

    public ParserExpressionsTest()
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
        };
    }

    public static TheoryData<string, bool> GetBooleanExpressionsTestData()
    {
        return new TheoryData<string, bool>
        {
            // Условные выражения и сравнения
            { "true", true },
            { "false", false },
        };
    }

    public static TheoryData<string> GetIfStatementsTestData()
    {
        return new TheoryData<string>
        {
            // Операторы if
            "if (true) {}",
            "if (false) { 1 + 5 - 3 }",
            "if (3 < 5) {}",
            "if (1 + 2 < 5) {}",
            "if (1 + 2 * 3 < 7) {}",
            "if (1 < 2 < 3) {}",
        };
    }

    [Theory]
    [MemberData(nameof(GetParseTestData))]
    public void Can_parse_expressions(string code, decimal expected)
    {
        Parser parser = new(_context, _environment, code);
        parser.ParseProgram();

        RuntimeValue result = Assert.Single(_environment.Results);
        Assert.Equal((decimal)expected, (decimal)result.Value, Precision);
    }

    [Theory]
    [MemberData(nameof(GetBooleanExpressionsTestData))]
    public void Can_parse_boolean_expressions(string code, bool expected)
    {
        Parser parser = new(_context, _environment, code);
        parser.ParseProgram();

        RuntimeValue result = Assert.Single(_environment.Results);
        Assert.Equal(RuntimeValue.ValueType.Boolean, result.Type);
        Assert.Equal(expected, result.Value);
    }

    [Theory]
    [MemberData(nameof(GetIfStatementsTestData))]
    public void Can_parse_if_statements(string code)
    {
        Parser parser = new(_context, _environment, code);
        parser.ParseProgram();

        RuntimeValue result = Assert.Single(_environment.Results);
        Assert.Equal(RuntimeValue.ValueType.Boolean, result.Type);
        Assert.IsType<bool>(result.Value);
    }
}