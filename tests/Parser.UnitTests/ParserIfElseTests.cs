using Blang.Common;
using Blang.Execution;
using Blang.Interpreter;

namespace Blang.Parser.UnitTests;

public class ParserIfElseTests
{
    private const int Precision = 5;
    private static readonly decimal Tolerance = (decimal)Math.Pow(0.1, Precision);

    private readonly Context _context;
    private readonly FakeEnvironment _environment;

    public ParserIfElseTests()
    {
        _context = new Context();
        _environment = new FakeEnvironment();
    }

    [Fact]
    public void Can_parse_if_without_else()
    {
        string code = "if (true) {}";
        List<RuntimeValue> expected = [];

        BlangInterpreter blang = new(_environment);
        blang.Execute(code);

        IReadOnlyList<RuntimeValue> actual = _environment.Results;
        AssertResults(expected, actual);
    }

    [Fact]
    public void Can_execute_if_body_when_condition_true()
    {
        string code = "if (true) { write(1 + 5 - 3) }";
        List<RuntimeValue> expected = [RuntimeValue.Number(3)];

        BlangInterpreter blang = new(_environment);
        blang.Execute(code);

        IReadOnlyList<RuntimeValue> actual = _environment.Results;
        AssertResults(expected, actual);
    }

    [Fact]
    public void Can_skip_if_body_when_condition_false()
    {
        string code = "if (false) { write(1 + 5 - 3) }";
        List<RuntimeValue> expected = [];

        BlangInterpreter blang = new(_environment);
        blang.Execute(code);

        IReadOnlyList<RuntimeValue> actual = _environment.Results;
        AssertResults(expected, actual);
    }

    [Fact]
    public void Can_parse_less_than_operator_in_condition()
    {
        string code = "if (3 < 5) { write(1) }";
        List<RuntimeValue> expected = [RuntimeValue.Number(1)];

        BlangInterpreter blang = new(_environment);
        blang.Execute(code);

        IReadOnlyList<RuntimeValue> actual = _environment.Results;
        AssertResults(expected, actual);
    }

    [Fact]
    public void Can_parse_complex_expression_with_comparison_and_arithmetic()
    {
        string code = "if (1 + 2 * 6^2 < 321) { write(1) }";
        List<RuntimeValue> expected = [RuntimeValue.Number(1)];

        BlangInterpreter blang = new(_environment);
        blang.Execute(code);

        IReadOnlyList<RuntimeValue> actual = _environment.Results;
        AssertResults(expected, actual);
    }

    [Fact]
    public void Can_parse_if_with_else_both_branches()
    {
        string code = "if (true) { write(1) } else { write(2) }";
        List<RuntimeValue> expected = [RuntimeValue.Number(1)];

        BlangInterpreter blang = new(_environment);
        blang.Execute(code);

        IReadOnlyList<RuntimeValue> actual = _environment.Results;
        AssertResults(expected, actual);
    }

    [Fact]
    public void Can_parse_if_with_else_execute_else_branch()
    {
        string code = "if (false) { write(1) } else { write(2) }";
        List<RuntimeValue> expected = [RuntimeValue.Number(2)];

        BlangInterpreter blang = new(_environment);
        blang.Execute(code);

        IReadOnlyList<RuntimeValue> actual = _environment.Results;
        AssertResults(expected, actual);
    }

    [Fact]
    public void Can_parse_nested_if_else()
    {
        string code = "if (true) { if (false) { write(1) } else { write(2) } }";
        List<RuntimeValue> expected = [RuntimeValue.Number(2)];

        BlangInterpreter blang = new(_environment);
        blang.Execute(code);

        IReadOnlyList<RuntimeValue> actual = _environment.Results;
        AssertResults(expected, actual);
    }

    [Fact]
    public void Can_parse_if_with_variable_condition()
    {
        string code = "num x = 10; if (x > 5) { write(1) }";
        List<RuntimeValue> expected = [RuntimeValue.Number(1)];

        BlangInterpreter blang = new(_environment);
        blang.Execute(code);

        IReadOnlyList<RuntimeValue> actual = _environment.Results;
        AssertResults(expected, actual);
    }

    [Fact]
    public void Can_parse_if_with_equality_condition()
    {
        string code = "if (5 == 5) { write(1) }";
        List<RuntimeValue> expected = [RuntimeValue.Number(1)];

        BlangInterpreter blang = new(_environment);
        blang.Execute(code);

        IReadOnlyList<RuntimeValue> actual = _environment.Results;
        AssertResults(expected, actual);
    }

    [Fact]
    public void Can_parse_if_with_inequality_condition()
    {
        string code = "if (5 != 3) { write(1) }";
        List<RuntimeValue> expected = [RuntimeValue.Number(1)];

        BlangInterpreter blang = new(_environment);
        blang.Execute(code);

        IReadOnlyList<RuntimeValue> actual = _environment.Results;
        AssertResults(expected, actual);
    }

    [Fact]
    public void Can_parse_if_with_greater_than_or_equal_condition()
    {
        string code = "if (5 >= 5) { write(1) }";
        List<RuntimeValue> expected = [RuntimeValue.Number(1)];

        BlangInterpreter blang = new(_environment);
        blang.Execute(code);

        IReadOnlyList<RuntimeValue> actual = _environment.Results;
        AssertResults(expected, actual);
    }

    [Fact]
    public void Can_parse_if_with_less_than_or_equal_condition()
    {
        string code = "if (3 <= 5) { write(1) }";
        List<RuntimeValue> expected = [RuntimeValue.Number(1)];

        BlangInterpreter blang = new(_environment);
        blang.Execute(code);

        IReadOnlyList<RuntimeValue> actual = _environment.Results;
        AssertResults(expected, actual);
    }

    [Fact]
    public void Can_parse_if_with_multiple_statements_in_body()
    {
        string code = "if (true) { write(1); write(2) }";
        List<RuntimeValue> expected = [RuntimeValue.Number(1), RuntimeValue.Number(2)];

        BlangInterpreter blang = new(_environment);
        blang.Execute(code);

        IReadOnlyList<RuntimeValue> actual = _environment.Results;
        AssertResults(expected, actual);
    }

    [Fact]
    public void Can_parse_if_else_with_multiple_statements()
    {
        string code = "if (false) { write(1); write(2) } else { write(3); write(4) }";
        List<RuntimeValue> expected = [RuntimeValue.Number(3), RuntimeValue.Number(4)];

        BlangInterpreter blang = new(_environment);
        blang.Execute(code);

        IReadOnlyList<RuntimeValue> actual = _environment.Results;
        AssertResults(expected, actual);
    }

    [Fact]
    public void Throws_on_missing_condition_parenthesis()
    {
        string code = "if true) { write(1) }";
        BlangInterpreter blang = new(_environment);

        Assert.Throws<UnexpectedLexemeException>(() => blang.Execute(code));
    }

    [Fact]
    public void Throws_on_missing_opening_brace()
    {
        string code = "if (true) write(1) }";
        BlangInterpreter blang = new(_environment);

        Assert.Throws<UnexpectedLexemeException>(() => blang.Execute(code));
    }

    [Fact]
    public void Throws_on_invalid_condition_expression()
    {
        string code = "if (true false) { write(1) }";
        BlangInterpreter blang = new(_environment);

        Assert.Throws<UnexpectedLexemeException>(() => blang.Execute(code));
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
                    if (Math.Abs((decimal)expectedValue - (decimal)actualValue) >= Tolerance)
                    {
                        Assert.Fail($"Expected does not match actual at index {i}: {expectedValue.Value} != {actualValue.Value}");
                    }

                    break;

                case RuntimeValue.ValueType.String:
                    Assert.Equal((string)expectedValue, (string)actualValue );
                    break;
            }
        }
    }
}