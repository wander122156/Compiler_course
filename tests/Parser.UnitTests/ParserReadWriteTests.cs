using Blang.Common;
using Blang.Execution;

namespace Blang.Parser.UnitTests;

public class ParserReadWriteTests
{
    private const int Precision = 5;
    private static readonly decimal Tolerance = (decimal)Math.Pow(0.1, Precision);

    private readonly Context _context;
    private readonly FakeEnvironment _environment;

    public ParserReadWriteTests()
    {
        _context = new Context();
        _environment = new FakeEnvironment();
    }

    [Fact]
    public void Can_parse_write_without_semicolon()
    {
        string code = "write (\"hello\")";
        List<RuntimeValue> expected = [RuntimeValue.String("hello")];

        Parser parser = new(_context, _environment, code);
        parser.ParseProgram();

        IReadOnlyList<RuntimeValue> actual = _environment.Results;
        AssertResults(expected, actual);
    }

    [Fact]
    public void Can_parse_write_with_multiple_expressions()
    {
        string code = "write (\"hello, \", \"i am \", \"Blang\")";
        List<RuntimeValue> expected = [
            RuntimeValue.String("hello, "),
            RuntimeValue.String("i am "),
            RuntimeValue.String("Blang")
        ];

        Parser parser = new(_context, _environment, code);
        parser.ParseProgram();

        IReadOnlyList<RuntimeValue> actual = _environment.Results;
        AssertResults(expected, actual);
    }

    [Fact]
    public void Can_parse_writeln_with_expression()
    {
        string code = "writeln (\"sum:\", 2 + 2)";
        List<RuntimeValue> expected = [
            RuntimeValue.String("sum:"),
            RuntimeValue.Number(4),
            RuntimeValue.NewLine(),
        ];

        Parser parser = new(_context, _environment, code);
        parser.ParseProgram();

        IReadOnlyList<RuntimeValue> actual = _environment.Results;
        AssertResults(expected, actual);
    }

    [Fact]
    public void Can_parse_read_single_variable()
    {
        string code = "num a; write(a); read(a); write(a)";
        _environment.SetInputLines("42");

        List<RuntimeValue> expected = [
            RuntimeValue.Undefined(),
            RuntimeValue.Number(42)
        ];

        Parser parser = new(_context, _environment, code);
        parser.ParseProgram();

        IReadOnlyList<RuntimeValue> actual = _environment.Results;
        AssertResults(expected, actual);
    }

    [Fact]
    public void Can_parse_read_multiple_variables()
    {
        string code = "num a, b, c; read(a, b, c); write(a); write(b); write(c)";
        _environment.SetInputLines("10", "20", "30");

        List<RuntimeValue> expected = [
            RuntimeValue.Number(10),
            RuntimeValue.Number(20),
            RuntimeValue.Number(30)
        ];

        Parser parser = new(_context, _environment, code);
        parser.ParseProgram();

        IReadOnlyList<RuntimeValue> actual = _environment.Results;
        AssertResults(expected, actual);
    }

    [Fact]
    public void Can_parse_readln_multiple_lines()
    {
        string code = "num first, second; readln(first); readln(second); write(first); write(second)";
        _environment.SetInputLines("100", "200");

        List<RuntimeValue> expected = [
            RuntimeValue.Number(100),
            RuntimeValue.Number(200)
        ];

        Parser parser = new(_context, _environment, code);
        parser.ParseProgram();

        IReadOnlyList<RuntimeValue> actual = _environment.Results;
        AssertResults(expected, actual);
    }

    [Fact]
    public void Can_parse_write_with_arithmetic_expression()
    {
        string code = "write(2 * 3 + 1)";
        List<RuntimeValue> expected = [RuntimeValue.Number(7)];

        Parser parser = new(_context, _environment, code);
        parser.ParseProgram();

        IReadOnlyList<RuntimeValue> actual = _environment.Results;
        AssertResults(expected, actual);
    }

    [Fact]
    public void Can_parse_write_with_variable_expression()
    {
        string code = "num x = 5; write(x * 2)";
        List<RuntimeValue> expected = [RuntimeValue.Number(10)];

        Parser parser = new(_context, _environment, code);
        parser.ParseProgram();

        IReadOnlyList<RuntimeValue> actual = _environment.Results;
        AssertResults(expected, actual);
    }

    [Fact]
    public void Can_parse_writeln_without_arguments()
    {
        string code = "writeln()";
        List<RuntimeValue> expected = [RuntimeValue.NewLine()];

        Parser parser = new(_context, _environment, code);
        parser.ParseProgram();

        IReadOnlyList<RuntimeValue> actual = _environment.Results;
        AssertResults(expected, actual);
    }

    [Fact]
    public void Can_parse_read_with_different_types()
    {
        string code = "num a; read(a); write(a)";
        _environment.SetInputLines("3.14");

        List<RuntimeValue> expected = [RuntimeValue.Number(3.14m)];

        Parser parser = new(_context, _environment, code);
        parser.ParseProgram();

        IReadOnlyList<RuntimeValue> actual = _environment.Results;
        AssertResults(expected, actual);
    }

    [Fact]
    public void Can_parse_readln_with_string_input()
    {
        string code = "num number; readln(number); write(number)";
        _environment.SetInputLines("123.456");

        List<RuntimeValue> expected = [RuntimeValue.Number(123.456m)];

        Parser parser = new(_context, _environment, code);
        parser.ParseProgram();

        IReadOnlyList<RuntimeValue> actual = _environment.Results;
        AssertResults(expected, actual);
    }

    [Fact]
    public void Can_parse_mixed_read_write_operations()
    {
        string code = """
            num a, b;
            write("Enter first number: ");
            read(a);
            write("Enter second number: ");
            read(b);
            write("Sum: ", a + b)
            """;

        _environment.SetInputLines("15", "25");

        List<RuntimeValue> expected = [
            RuntimeValue.String("Enter first number: "),
            RuntimeValue.String("Enter second number: "),
            RuntimeValue.String("Sum: "),
            RuntimeValue.Number(40)
        ];

        Parser parser = new(_context, _environment, code);
        parser.ParseProgram();

        IReadOnlyList<RuntimeValue> actual = _environment.Results;
        AssertResults(expected, actual);
    }

    [Fact]
    public void Throws_on_read_undefined_variable()
    {
        string code = "read(undefinedVar)";
        Parser parser = new(_context, _environment, code);

        Assert.Throws<ArgumentException>(() => parser.ParseProgram());
    }

    [Fact]
    public void Throws_on_write_undefined_variable()
    {
        string code = "write(undefinedVar)";
        Parser parser = new(_context, _environment, code);

        Assert.Throws<ArgumentException>(() => parser.ParseProgram());
    }

    [Fact]
    public void Throws_on_missing_parenthesis_in_write()
    {
        string code = "write \"hello\"";
        Parser parser = new(_context, _environment, code);

        Assert.Throws<UnexpectedLexemeException>(() => parser.ParseProgram());
    }

    [Fact]
    public void Throws_on_missing_comma_in_multiple_read()
    {
        string code = "num a, b; read(a b)";
        Parser parser = new(_context, _environment, code);

        Assert.Throws<UnexpectedLexemeException>(() => parser.ParseProgram());
    }

    private void AssertResults(List<RuntimeValue> expected, IReadOnlyList<RuntimeValue> actual)
    {
        if (expected.Count != actual.Count)
        {
            Assert.Fail(
                $"Actual results count does not match expected. Expected: {expected.Count}, Actual: {actual.Count}.\n" +
                $"Expected: [{string.Join(", ", expected.Select(r => $"{r.Type}:{r.Value}"))}]\n" +
                $"Actual: [{string.Join(", ", actual.Select(r => $"{r.Type}:{r.Value}"))}]"
            );
        }

        for (int i = 0; i < expected.Count; i++)
        {
            RuntimeValue expectedValue = expected[i];
            RuntimeValue actualValue = actual[i];

            Assert.Equal(expectedValue.Type, actualValue.Type);

            switch (expectedValue.Type)
            {
                case RuntimeValue.ValueType.Number:
                    if (Math.Abs((decimal)expectedValue.Value - (decimal)actualValue.Value) >= Tolerance)
                    {
                        Assert.Fail($"Expected does not match actual at index {i}: {expectedValue.Value} != {actualValue.Value}");
                    }

                    break;

                case RuntimeValue.ValueType.String:
                    Assert.Equal((string)expectedValue.Value, (string)actualValue.Value);
                    break;
            }
        }
    }
}