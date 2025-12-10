using Blang.Common;
using Blang.Execution;
using Blang.Interpreter;
using Blang.Semantic;

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

        BlangInterpreter blang = new(_environment);
        blang.Execute(code);

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

        BlangInterpreter blang = new(_environment);
        blang.Execute(code);

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

        BlangInterpreter blang = new(_environment);
        blang.Execute(code);

        IReadOnlyList<RuntimeValue> actual = _environment.Results;
        AssertResults(expected, actual);
    }

    [Fact]
    public void Can_execute_function_without_return_value()
    {
        string code = @"func void printHello() { write(""Hello"") }; printHello()";
        List<RuntimeValue> expected = [
            RuntimeValue.String("Hello"),
        ];

        BlangInterpreter blang = new(_environment);
        blang.Execute(code);

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

        BlangInterpreter blang = new(_environment);
        blang.Execute(code);

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

        BlangInterpreter blang = new(_environment);
        blang.Execute(code);

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

        BlangInterpreter blang = new(_environment);
        blang.Execute(code);

        IReadOnlyList<RuntimeValue> actual = _environment.Results;
        AssertResults(expected, actual);
    }

    [Fact]
    public void Can_execute_recursive_function_calls()
    {
        string code = @"
            func void fir(num n) { sec(n+1) };
            func void sec(num n) 
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

        BlangInterpreter blang = new(_environment);
        blang.Execute(code);

        IReadOnlyList<RuntimeValue> actual = _environment.Results;
        AssertResults(expected, actual);
    }

    [Fact]
    public void Cannot_redefine_function()
    {
        string code = @"
        func num test() { return 1; };
        func num test() { return 2; };
    ";

        BlangInterpreter blang = new(_environment);
        Assert.Throws<ArgumentException>(() => blang.Execute(code));
    }

    [Fact]
    public void Cannot_call_function_with_wrong_argument_count()
    {
        string code = @"
        func num add(num a, num b) { return a + b; };
        write(add(1));
    ";

        BlangInterpreter blang = new(_environment);
        Assert.Throws<TypeException>(() => blang.Execute(code));
    }

    [Fact]
    public void Can_return_from_for_loop_in_function()
    {
        string code = @"
        func num findFirstEven() {
            for (num i = 1; i <= 10; i = i + 1) {
                if (i % 2 == 0) {
                    return i;
                };
            };
            return 0;
        };
        write(findFirstEven());
    ";

        List<RuntimeValue> expected = [
            RuntimeValue.Number(2),
    ];

        BlangInterpreter blang = new(_environment);
        blang.Execute(code);

        IReadOnlyList<RuntimeValue> actual = _environment.Results;
        AssertResults(expected, actual);
    }

    [Fact]
    public void Throws_on_void_function_with_return()
    {
        string code = @"func void printHello() { write(""Hello""); return 1 }; printHello()";

        BlangInterpreter blang = new(_environment);

        Assert.Throws<TypeException>(() => blang.Execute(code));
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
                    Assert.Equal((string)expectedValue, (string)actualValue);
                    break;

                case RuntimeValue.ValueType.Boolean:
                    Assert.Equal((bool)expectedValue, (bool)actualValue);
                    break;
            }
        }
    }
}