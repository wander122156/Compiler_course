using Blang.Common;
using Blang.Execution;
using Blang.Interpreter;
using Blang.Parser;

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
    public void Can_execute_Factorial_program()
    {
        const string code = """
            func num factorial(num n)
            {
                if (n <= 1) 
                {
                    return 1;
                } 
                else 
                {
                    return n * factorial(n - 1);
                }
            };
            num n;
            read(n);
            num result = factorial(n);
            writeln(result)
            """;
        _environment.SetInputLines("5");

        // выполнение программы:
        BlangInterpreter blang = new(_environment);
        blang.Execute(code);

        // получение результата
        IReadOnlyList<RuntimeValue> actual = _environment.Results;

        // ожидаемый результат
        List<RuntimeValue> expected = [
            RuntimeValue.Number(120),
            RuntimeValue.NewLine()
        ];

        // сравнение результататов
        AssertResults(expected, actual);
    }

    [Fact]
    public void Can_execute_GSD_program()
    {
        const string code = """
            num a, b, temp;
            read(a,b);
            while (b != 0) {
                temp = b;
                b = a % b;
                a = temp
            };
            writeln(a)
            """;
        _environment.SetInputLines("49", "28");

        // выполнение программы:
        BlangInterpreter blang = new(_environment);
        blang.Execute(code);

        // получение результата
        IReadOnlyList<RuntimeValue> actual = _environment.Results;

        // ожидаемый результат
        List<RuntimeValue> expected = [
            RuntimeValue.Number(7),
            RuntimeValue.NewLine()
        ];

        // сравнение результататов
        AssertResults(expected, actual);
    }

    [Fact]
    public void Can_execute_SumDigits_program()
    {
        const string code = """
            num n;
            num sum = 0;
            write("Введите целое число: ");
            readln(n);
            n = abs(n);
            while(n > 0){
                sum = sum + n%10;
                n = floor(n/10)
            };
            writeln(sum)
            """;
        _environment.SetInputLines("6123");

        // выполнение программы:
        BlangInterpreter blang = new(_environment);
        blang.Execute(code);

        // получение результата
        IReadOnlyList<RuntimeValue> actual = _environment.Results;

        // ожидаемый результат
        List<RuntimeValue> expected = [
            RuntimeValue.String("Введите целое число: "),
            RuntimeValue.Number(12),
            RuntimeValue.NewLine()
        ];

        // сравнение результататов
        AssertResults(expected, actual);
    }

    [Fact]
    public void Can_execute_SumNums_program()
    {
        const string code = """
            num a, b, sum;
            
            write("First num: ");
            read(a);
            
            write("Second num: ");
            read(b);
            
            write("Sum is: ");
            
            sum = a + b;
            write(sum)
            """;
        _environment.SetInputLines("0.1", "0.2");

        // выполнение программы:
        BlangInterpreter blang = new(_environment);
        blang.Execute(code);

        // получение результата
        IReadOnlyList<RuntimeValue> actual = _environment.Results;

        // ожидаемый результат
        List<RuntimeValue> expected = [
            RuntimeValue.String("First num: "),
            RuntimeValue.String("Second num: "),
            RuntimeValue.String("Sum is: "),
            RuntimeValue.Number(0.3m),
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
        _environment.SetInputLines("10");

        // выполнение программы:
        BlangInterpreter blang = new(_environment);
        blang.Execute(code);

        // получение результата
        IReadOnlyList<RuntimeValue> actual = _environment.Results;

        // ожидаемый результат
        List<RuntimeValue> expected = [
            RuntimeValue.String("Введите радиус окружности: "),
            RuntimeValue.String("Площадь окружности: "),
            RuntimeValue.Number(314.15926535897900m),
            RuntimeValue.NewLine(),
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
        _environment.SetInputLines("120");

        // выполнение программы:
        BlangInterpreter blang = new(_environment);
        blang.Execute(code);

        // получение результата
        IReadOnlyList<RuntimeValue> actual = _environment.Results;

        // ожидаемый результат
        List<RuntimeValue> expected = [
            RuntimeValue.String("Введите количество милей: "),
            RuntimeValue.Number(193.2m),
            RuntimeValue.NewLine(),
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