using Blang.Common;
using Blang.Execution;

namespace Blang.Parser.UnitTests;

public class ParserLoopsTests
{
    private const int Precision = 5;
    private static readonly decimal Tolerance = (decimal)Math.Pow(0.1, Precision);

    private readonly Context _context;
    private readonly FakeEnvironment _environment;

    public ParserLoopsTests()
    {
        _context = new Context();
        _environment = new FakeEnvironment();
    }

    [Fact]
    public void Can_execute_do_while_loop_minimum_one_iteration()
    {
        string code = @"
        num i = 0;
        do {
            write(i);
            i = i + 1
        } while (i < 3)
        ";
        List<RuntimeValue> expected = [
            RuntimeValue.Number(0),
        RuntimeValue.Number(1),
        RuntimeValue.Number(2)
        ];

        Parser parser = new(_context, _environment, code);
        parser.ParseProgram();

        IReadOnlyList<RuntimeValue> actual = _environment.Results;
        AssertResults(expected, actual);
    }

    [Fact]
    public void Can_execute_do_while_loop_with_false_condition()
    {
        string code = @"
        do {
            write(1)
        } while (false)
        ";
        List<RuntimeValue> expected = [RuntimeValue.Number(1)];

        Parser parser = new(_context, _environment, code);
        parser.ParseProgram();

        IReadOnlyList<RuntimeValue> actual = _environment.Results;
        AssertResults(expected, actual);
    }

    [Fact]
    public void Can_execute_nested_do_while_loops()
    {
        string code = @"
        num i = 0;
        do {
            num j = 0;
            do {
                write(i * 10 + j);
                j = j + 1
            } while (j < 2);
            i = i + 1
        } while (i < 2)
        ";
        List<RuntimeValue> expected = [
            RuntimeValue.Number(0),
        RuntimeValue.Number(1),
        RuntimeValue.Number(10),
        RuntimeValue.Number(11)
        ];

        Parser parser = new(_context, _environment, code);
        parser.ParseProgram();

        IReadOnlyList<RuntimeValue> actual = _environment.Results;
        AssertResults(expected, actual);
    }

    [Fact]
    public void Can_execute_for_loop_body()
    {
        string code = "for (num i = 0; i < 3; i = i + 1) { write(i) }";
        List<RuntimeValue> expected = [
            RuntimeValue.Number(0),
            RuntimeValue.Number(1),
            RuntimeValue.Number(2)
        ];

        Parser parser = new(_context, _environment, code);
        parser.ParseProgram();

        IReadOnlyList<RuntimeValue> actual = _environment.Results;
        AssertResults(expected, actual);
    }

    [Fact]
    public void Can_execute_for_loop_with_single_iteration()
    {
        string code = "for (num i = 5; i < 6; i = i + 1) { write(i) }";
        List<RuntimeValue> expected = [RuntimeValue.Number(5)];

        Parser parser = new(_context, _environment, code);
        parser.ParseProgram();

        IReadOnlyList<RuntimeValue> actual = _environment.Results;
        AssertResults(expected, actual);
    }

    [Fact]
    public void Can_execute_for_loop_with_no_iterations()
    {
        string code = "for (num i = 10; i < 5; i = i + 1) { write(i) }";
        List<RuntimeValue> expected = [];

        Parser parser = new(_context, _environment, code);
        parser.ParseProgram();

        IReadOnlyList<RuntimeValue> actual = _environment.Results;
        AssertResults(expected, actual);
    }

    [Fact]
    public void Can_execute_for_loop_with_negative_step()
    {
        string code = "for (num i = 5; i > 0; i = i - 1) { write(i) }";
        List<RuntimeValue> expected = [
            RuntimeValue.Number(5),
            RuntimeValue.Number(4),
            RuntimeValue.Number(3),
            RuntimeValue.Number(2),
            RuntimeValue.Number(1)
        ];

        Parser parser = new(_context, _environment, code);
        parser.ParseProgram();

        IReadOnlyList<RuntimeValue> actual = _environment.Results;
        AssertResults(expected, actual);
    }

    [Fact]
    public void Can_execute_for_loop_with_multiplication_step()
    {
        string code = "for (num i = 1; i < 10; i = i * 2) { write(i) }";
        List<RuntimeValue> expected = [
            RuntimeValue.Number(1),
            RuntimeValue.Number(2),
            RuntimeValue.Number(4),
            RuntimeValue.Number(8)
        ];

        Parser parser = new(_context, _environment, code);
        parser.ParseProgram();

        IReadOnlyList<RuntimeValue> actual = _environment.Results;
        AssertResults(expected, actual);
    }

    [Fact]
    public void Can_execute_nested_for_loops()
    {
        string code = @"
            for (num i = 0; i < 2; i = i + 1) {
                for (num j = 0; j < 2; j = j + 1) {
                    write(i * 10 + j)
                }
            }
        ";
        List<RuntimeValue> expected = [
            RuntimeValue.Number(0),
            RuntimeValue.Number(1),
            RuntimeValue.Number(10),
            RuntimeValue.Number(11)
        ];

        Parser parser = new(_context, _environment, code);
        parser.ParseProgram();

        IReadOnlyList<RuntimeValue> actual = _environment.Results;
        AssertResults(expected, actual);
    }

    [Fact]
    public void Can_execute_for_loop_with_external_variable()
    {
        string code = @"
            num count = 3;
            for (num i = 0; i < count; i = i + 1) { write(i) }
        ";
        List<RuntimeValue> expected = [
            RuntimeValue.Number(0),
            RuntimeValue.Number(1),
            RuntimeValue.Number(2)
        ];

        Parser parser = new(_context, _environment, code);
        parser.ParseProgram();

        IReadOnlyList<RuntimeValue> actual = _environment.Results;
        AssertResults(expected, actual);
    }

    [Fact]
    public void Can_execute_for_loop_modifying_external_variable()
    {
        string code = @"
            num sum = 0;
            for (num i = 1; i <= 3; i = i + 1) { sum = sum + i };
            write(sum)
        ";
        List<RuntimeValue> expected = [RuntimeValue.Number(6)];

        Parser parser = new(_context, _environment, code);
        parser.ParseProgram();

        IReadOnlyList<RuntimeValue> actual = _environment.Results;
        AssertResults(expected, actual);
    }

    [Fact]
    public void Throws_when_accessing_loop_variable_outside_scope()
    {
        string code = @"
            for (num i = 0; i < 3; i = i + 1) { write(i) };
            write(i)
        ";

        Parser parser = new(_context, _environment, code);

        Assert.Throws<ArgumentException>(() => parser.ParseProgram());
    }

    [Fact]
    public void Can_execute_for_loop_with_step_2()
    {
        string code = "for (num i = 0; i < 6; i = i + 2) { write(i) }";
        List<RuntimeValue> expected = [
            RuntimeValue.Number(0),
            RuntimeValue.Number(2),
            RuntimeValue.Number(4)
        ];

        Parser parser = new(_context, _environment, code);
        parser.ParseProgram();

        IReadOnlyList<RuntimeValue> actual = _environment.Results;
        AssertResults(expected, actual);
    }

    [Fact]
    public void Can_execute_for_loop_with_complex_increment()
    {
        string code = "for (num i = 0; i < 10; i = i * 2 + 1) { write(i) }";
        List<RuntimeValue> expected = [
            RuntimeValue.Number(0),
            RuntimeValue.Number(1),
            RuntimeValue.Number(3),
            RuntimeValue.Number(7)
        ];

        Parser parser = new(_context, _environment, code);
        parser.ParseProgram();

        IReadOnlyList<RuntimeValue> actual = _environment.Results;
        AssertResults(expected, actual);
    }

    [Fact]
    public void Can_execute_for_loop_with_nested_if()
    {
        string code = @"
            for (num i = 0; i < 5; i = i + 1) {
                if (i % 2 == 0) { write(i) }
            }
        ";
        List<RuntimeValue> expected = [
            RuntimeValue.Number(0),
            RuntimeValue.Number(2),
            RuntimeValue.Number(4)
        ];

        Parser parser = new(_context, _environment, code);
        parser.ParseProgram();

        IReadOnlyList<RuntimeValue> actual = _environment.Results;
        AssertResults(expected, actual);
    }

    [Fact]
    public void Can_execute_for_loop_with_variable_in_condition()
    {
        string code = @"
            num limit = 4;
            for (num i = 0; i < limit; i = i + 1) { write(i) }
        ";
        List<RuntimeValue> expected = [
            RuntimeValue.Number(0),
            RuntimeValue.Number(1),
            RuntimeValue.Number(2),
            RuntimeValue.Number(3)
        ];

        Parser parser = new(_context, _environment, code);
        parser.ParseProgram();

        IReadOnlyList<RuntimeValue> actual = _environment.Results;
        AssertResults(expected, actual);
    }

    [Fact]
    public void Can_execute_different_variables_with_same_name_in_nested_loops()
    {
        string code = @"
            for (num i = 0; i < 2; i = i + 1) {
                for (num i = 10; i < 12; i = i + 1) {
                    write(i)
                }
            }
        ";
        List<RuntimeValue> expected = [
            RuntimeValue.Number(10),
            RuntimeValue.Number(11),
            RuntimeValue.Number(10),
            RuntimeValue.Number(11)
        ];

        Parser parser = new(_context, _environment, code);
        parser.ParseProgram();

        IReadOnlyList<RuntimeValue> actual = _environment.Results;
        AssertResults(expected, actual);
    }

    [Fact]
    public void Can_execute_while_loop()
    {
        string code = @"
            num i = 0;
            while(i < 5)
            {
                write(i);
                i = i + 1
            }

        ";
        List<RuntimeValue> expected = [
            RuntimeValue.Number(0),
            RuntimeValue.Number(1),
            RuntimeValue.Number(2),
            RuntimeValue.Number(3),
            RuntimeValue.Number(4),
        ];

        Parser parser = new(_context, _environment, code);
        parser.ParseProgram();

        IReadOnlyList<RuntimeValue> actual = _environment.Results;
        AssertResults(expected, actual);
    }

    [Fact]
    public void Can_execute_while_loop_body()
    {
        string code = "num i = 0; while (i < 3) { write(i); i = i + 1 }";
        List<RuntimeValue> expected = [
            RuntimeValue.Number(0),
        RuntimeValue.Number(1),
        RuntimeValue.Number(2)
        ];

        Parser parser = new(_context, _environment, code);
        parser.ParseProgram();

        IReadOnlyList<RuntimeValue> actual = _environment.Results;
        AssertResults(expected, actual);
    }

    [Fact]
    public void Can_execute_while_loop_with_single_iteration()
    {
        string code = "num i = 5; while (i < 6) { write(i); i = i + 1 }";
        List<RuntimeValue> expected = [RuntimeValue.Number(5)];

        Parser parser = new(_context, _environment, code);
        parser.ParseProgram();

        IReadOnlyList<RuntimeValue> actual = _environment.Results;
        AssertResults(expected, actual);
    }

    [Fact]
    public void Can_execute_while_loop_with_no_iterations()
    {
        string code = "while (false) { write(1) }";
        List<RuntimeValue> expected = [];

        Parser parser = new(_context, _environment, code);
        parser.ParseProgram();

        IReadOnlyList<RuntimeValue> actual = _environment.Results;
        AssertResults(expected, actual);
    }

    [Fact]
    public void Can_execute_while_loop_with_negative_step()
    {
        string code = "num i = 5; while (i > 0) { write(i); i = i - 1 }";
        List<RuntimeValue> expected = [
            RuntimeValue.Number(5),
        RuntimeValue.Number(4),
        RuntimeValue.Number(3),
        RuntimeValue.Number(2),
        RuntimeValue.Number(1)
        ];

        Parser parser = new(_context, _environment, code);
        parser.ParseProgram();

        IReadOnlyList<RuntimeValue> actual = _environment.Results;
        AssertResults(expected, actual);
    }

    [Fact]
    public void Can_execute_nested_while_loops()
    {
        string code = @"
        num i = 0;
        while (i < 2) {
            num j = 0;
            while (j < 2) {
                write(i * 10 + j);
                j = j + 1
            };
            i = i + 1
        }
    ";
        List<RuntimeValue> expected = [
            RuntimeValue.Number(0),
        RuntimeValue.Number(1),
        RuntimeValue.Number(10),
        RuntimeValue.Number(11)
        ];

        Parser parser = new(_context, _environment, code);
        parser.ParseProgram();

        IReadOnlyList<RuntimeValue> actual = _environment.Results;
        AssertResults(expected, actual);
    }

    [Fact]
    public void Can_execute_while_loop_modifying_external_variable()
    {
        string code = @"
        num sum = 0;
        num i = 1;
        while (i <= 3) { sum = sum + i; i = i + 1 };
        write(sum)
    ";
        List<RuntimeValue> expected = [RuntimeValue.Number(6)];

        Parser parser = new(_context, _environment, code);
        parser.ParseProgram();

        IReadOnlyList<RuntimeValue> actual = _environment.Results;
        AssertResults(expected, actual);
    }

    [Fact]
    public void Throws_on_missing_opening_parenthesis()
    {
        string code = "for num i = 0; i < 10; i = i + 1) {}";
        Parser parser = new(_context, _environment, code);

        Assert.Throws<UnexpectedLexemeException>(() => parser.ParseProgram());
    }

    [Fact]
    public void Throws_on_missing_closing_parenthesis()
    {
        string code = "for (num i = 0; i < 10; i = i + 1 {}";
        Parser parser = new(_context, _environment, code);

        Assert.Throws<UnexpectedLexemeException>(() => parser.ParseProgram());
    }

    [Fact]
    public void Throws_on_missing_semicolon_between_conditions()
    {
        string code = "for (num i = 0 i < 10; i = i + 1) {}";
        Parser parser = new(_context, _environment, code);

        Assert.Throws<UnexpectedLexemeException>(() => parser.ParseProgram());
    }

    [Fact]
    public void Throws_on_invalid_initialization_expression()
    {
        string code = "for (num i = ; i < 10; i = i + 1) {}";
        Parser parser = new(_context, _environment, code);

        Assert.Throws<UnexpectedLexemeException>(() => parser.ParseProgram());
    }

    [Fact]
    public void Throws_on_invalid_condition_expression()
    {
        string code = "for (num i = 0; ; i = i + 1) {}";
        Parser parser = new(_context, _environment, code);

        Assert.Throws<UnexpectedLexemeException>(() => parser.ParseProgram());
    }

    [Fact]
    public void Throws_on_invalid_increment_expression()
    {
        string code = "for (num i = 0; i < 10; ) {}";
        Parser parser = new(_context, _environment, code);

        Assert.Throws<UnexpectedLexemeException>(() => parser.ParseProgram());
    }

    [Fact]
    public void Throws_on_using_undeclared_variable_in_condition()
    {
        string code = "for (num i = 0; j < 10; i = i + 1) {}";
        Parser parser = new(_context, _environment, code);

        Assert.Throws<ArgumentException>(() => parser.ParseProgram());
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

                case RuntimeValue.ValueType.Boolean:
                    Assert.Equal((bool)expectedValue.Value, (bool)actualValue.Value);
                    break;
            }
        }
    }
}