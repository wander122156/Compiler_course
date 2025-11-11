using System.Globalization;

using Blang.Common;
using Blang.Execution;
using Blang.Parser;

using Xunit;

namespace Interpreter.Specs;

public class InterpreterTests
{
    // Допустимая абсолютная погрешность сравнения чисел с плавающей точкой.
    private const int Precision = 5;
    private static readonly decimal Tolerance = (decimal)Math.Pow(0.1, Precision);

    private readonly Context _context;
    private readonly FakeEnvironment _environment;

    public InterpreterTests()
    {
        _context = new Context();
        _environment = new FakeEnvironment();
    }

    [Fact]
    public void Can_execute_SumNums_program()
    {
        string code = """
            num a, b, sum;
            
            write("First num: ");
            read(a);
            
            write("Second num: ");
            read(b);
            
            write("Sum is: ");
            
            sum = a + b;
            write(sum)
            """;
        _environment.SetSimulatedInput("0.1", "0.2");

        // выполнение программы:
        Parser parser = new(_context, _environment, code);
        parser.ParseProgram();

        // получение результата
        IReadOnlyList<RuntimeValue> actual = _environment.Results;

        // ожидаемый результат
        List<RuntimeValue> expected = [
            RuntimeValue.Number(0),
            RuntimeValue.String("First num: "),
            RuntimeValue.Number(0.1m),
            RuntimeValue.String("Second num: "),
            RuntimeValue.Number(0.2m),
            RuntimeValue.String("Sum is: "),
            RuntimeValue.Number(0.3m), // результат asigment
            RuntimeValue.Number(0.3m), // результат write
        ];

        // сравнение результататов
        AssertResults(expected, actual);
    }

    [Fact]
    public void Can_execute_CircleSquare_program()
    {
        string code = """
            num r;
            num S;
            write("Введите радиус окружности: ");
            readln(r);
            S = Pi * r^2;
            writeln("Площадь окружности: ", S)
            """;
        _environment.SetSimulatedInput("10");

        // выполнение программы:
        Parser parser = new(_context, _environment, code);
        parser.ParseProgram();

        // получение результата
        IReadOnlyList<RuntimeValue> actual = _environment.Results;

        // ожидаемый результат
        List<RuntimeValue> expected = [
            RuntimeValue.Number(0),
            RuntimeValue.Number(0),
            RuntimeValue.String("Введите радиус окружности: "),
            RuntimeValue.Number(10),
            RuntimeValue.Number(314.15926535897900m), // результат assigment
            RuntimeValue.Number(314.15926535897900m), // результат последнего выражения в writeln
        ];

        // сравнение результататов
        AssertResults(expected, actual);
    }

    [Fact]
    public void Can_execute_MilesToKm_program()
    {
        string code = """
            num miles;
            num kms;
            write("Введите количество милей: ");
            readln(miles);
            kms = miles * 1.61;
            writeln(kms)
            """;
        _environment.SetSimulatedInput("120");

        // выполнение программы:
        Parser parser = new(_context, _environment, code);
        parser.ParseProgram();

        // получение результата
        IReadOnlyList<RuntimeValue> actual = _environment.Results;

        // ожидаемый результат
        List<RuntimeValue> expected = [
            RuntimeValue.Number(0),
            RuntimeValue.Number(0),
            RuntimeValue.String("Введите количество милей: "),
            RuntimeValue.Number(120),
            RuntimeValue.Number(193.2m), // результат assigment
            RuntimeValue.Number(193.2m), // результат writeln
        ];

        // сравнение результататов
        AssertResults(expected, actual);
    }

    private void AssertResults(List<RuntimeValue> expected, IReadOnlyList<RuntimeValue> actual)
    {
        // Сначала проверяем количество
        if (expected.Count != actual.Count)
        {
            Assert.Fail(
                $"Actual results count does not match expected. Expected: {expected.Count}, Actual: {actual.Count}."
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
                        Assert.Fail($"Expected does not match actual at index {i}: {expectedValue.Value} != {expectedValue.Value}");
                    }

                    Assert.Equal((decimal)expectedValue.Value, (decimal)actualValue.Value, Precision);
                    break;

                case RuntimeValue.ValueType.String:
                    Assert.Equal((string)expectedValue.Value, (string)actualValue.Value);
                    break;
            }
        }
    }
}