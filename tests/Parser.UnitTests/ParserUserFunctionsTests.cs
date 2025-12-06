using Blang.Common;
using Blang.Execution;
using Blang.Parser;

namespace Blang.Parser.UnitTests;

public class ParserUserFunctionsTests
{
    private const int Precision = 5;
    private static readonly decimal Tolerance = (decimal)Math.Pow(0.1, Precision);

    private readonly Context _context;
    private readonly FakeEnvironment _environment;

    public ParserUserFunctionsTests()
    {
        _context = new Context();
        _environment = new FakeEnvironment();
    }

    [Fact]
    public void Can_execute_function_without_parameters()
    {
        string code = @"func num getFive() { return 5 }; write(getFive())";
        List<RuntimeValue> expected = [
            RuntimeValue.Number(5),
        ];

        Parser parser = new(_context, _environment, code);
        parser.ParseProgram();

        IReadOnlyList<RuntimeValue> actual = _environment.Results;
        AssertResults(expected, actual);
    }

    [Fact]
    public void Can_execute_function_with_one_parameter()
    {
        string code = @"func num square(num x) { return x * x }; write(square(4))";
        List<RuntimeValue> expected = [
            RuntimeValue.Number(16),
        ];

        Parser parser = new(_context, _environment, code);
        parser.ParseProgram();

        IReadOnlyList<RuntimeValue> actual = _environment.Results;
        AssertResults(expected, actual);
    }

    [Fact]
    public void Can_execute_function_with_multiple_parameters()
    {
        string code = @"func num multiply(num a, num b) { return a * b }; write(multiply(3, 7))";
        List<RuntimeValue> expected = [
            RuntimeValue.Number(21),
        ];

        Parser parser = new(_context, _environment, code);
        parser.ParseProgram();

        IReadOnlyList<RuntimeValue> actual = _environment.Results;
        AssertResults(expected, actual);
    }

    [Fact]
    public void Can_execute_function_without_return_value()
    {
        string code = @"func num printHello() { write(""Hello"") }; printHello()";
        List<RuntimeValue> expected = [
            RuntimeValue.String("Hello"),
        ];

        Parser parser = new(_context, _environment, code);
        parser.ParseProgram();

        IReadOnlyList<RuntimeValue> actual = _environment.Results;
        AssertResults(expected, actual);
    }

    [Fact]
    public void Can_execute_function_with_local_variables()
    {
        string code = @"func num testScope() { num x = 10; return x }; write(testScope())";
        List<RuntimeValue> expected = [
            RuntimeValue.Number(10),
        ];

        Parser parser = new(_context, _environment, code);
        parser.ParseProgram();

        IReadOnlyList<RuntimeValue> actual = _environment.Results;
        AssertResults(expected, actual);
    }

    [Fact]
    public void Can_execute_function_with_scope_isolation()
    {
        string code = @"num x = 5; func num testIsolation() { num x = 20; return x }; write(testIsolation(), x)";
        List<RuntimeValue> expected = [
            RuntimeValue.Number(20),
            RuntimeValue.Number(5),
        ];

        Parser parser = new(_context, _environment, code);
        parser.ParseProgram();

        IReadOnlyList<RuntimeValue> actual = _environment.Results;
        AssertResults(expected, actual);
    }

    [Fact]
    public void Can_execute_function_with_parameter_shadowing()
    {
        string code = @"num a = 100; func num testParams(num a) { return a }; write(testParams(50), a)";
        List<RuntimeValue> expected = [
            RuntimeValue.Number(50),
            RuntimeValue.Number(100),
        ];

        Parser parser = new(_context, _environment, code);
        parser.ParseProgram();

        IReadOnlyList<RuntimeValue> actual = _environment.Results;
        AssertResults(expected, actual);
    }

    [Fact]
    public void Can_execute_recursive_function_calls()
    {
        string code = @"
            func num fir(num n) { sec(n+1) };
            func num sec(num n) 
            { 
                write(n);
                if (n < 4) fir(n) 
            };

            sec(1);
        ";
        List<RuntimeValue> expected = [
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