using Blang.Common;
using Blang.Execution;
using Blang.Interpreter;
using Blang.Parser;

namespace Blang.Semantic.UnitTests;

public class TypesTests
{
    private const int Precision = 5;
    private static readonly decimal Tolerance = (decimal)Math.Pow(0.1, Precision);

    private readonly Context _context;
    private readonly FakeEnvironment _environment;

    public TypesTests()
    {
        _context = new Context();
        _environment = new FakeEnvironment();
    }

    // Приведение типов к bool
    [Fact]
    public void If_String_Condition_Convert_to_bool()
    {
        string code = """
            if ("hello") { write(1) }
            """;
        List<RuntimeValue> expected = [
            RuntimeValue.Number(1),
        ];
        BlangInterpreter blang = new(_environment);
        blang.Execute(code);

        IReadOnlyList<RuntimeValue> actual = _environment.Results;
        AssertResults(expected, actual);
    }

    [Fact]
    public void If_Number_Condition_Convert_to_bool()
    {
        string code = """
            if (10) { write(1) }
            """;

        List<RuntimeValue> expected = [
            RuntimeValue.Number(1),
        ];
        BlangInterpreter blang = new(_environment);
        blang.Execute(code);

        IReadOnlyList<RuntimeValue> actual = _environment.Results;
        AssertResults(expected, actual);
    }

    // Ошибка: сложение num и bool → TypeException
    [Fact]
    public void Addition_Num_And_Bool_Should_Fail()
    {
        string code = """
            write(true + 5)
            """;

        Assert.Throws<TypeException>(() =>
        {
            BlangInterpreter blang = new(_environment);
            blang.Execute(code);
        });
    }

    // Ошибка: строка в арифметике → TypeException
    [Fact]
    public void String_In_Arithmetic_Should_Fail()
    {
        string code = """
            write("abc" * 3)
            """;

        Assert.Throws<TypeException>(() =>
        {
            BlangInterpreter blang = new(_environment);
            blang.Execute(code);
        });
    }

    // Ошибка: арифметика с bool → TypeException
    [Fact]
    public void Arithmetic_With_Bool_Should_Fail()
    {
        string code = """
            write(5 - false)
            """;

        Assert.Throws<TypeException>(() =>
        {
            BlangInterpreter blang = new(_environment);
            blang.Execute(code);
        });
    }

    // Ошибка: сравнение разных типов → TypeException
    [Fact]
    public void Equality_Num_And_String_Should_Fail()
    {
        string code = """
            if(1 == "1"){ write(1) }
            """;

        Assert.Throws<TypeException>(() =>
        {
            BlangInterpreter blang = new(_environment);
            blang.Execute(code);
        });
    }

    // Ошибка: сравнение num и string → TypeException
    [Fact]
    public void Less_Num_And_String_Should_Fail()
    {
        string code = """
            if (1 < "abc") { write(1) }
            """;

        Assert.Throws<TypeException>(() =>
        {
            BlangInterpreter blang = new(_environment);
            blang.Execute(code);
        });
    }

    // Ошибка: присваивание неверного типа
    [Fact]
    public void Assignment_Wrong_Type_Should_Fail()
    {
        string code = """
            num x;
            x = "abc"
            """;

        Assert.Throws<TypeException>(() =>
        {
            BlangInterpreter blang = new(_environment);
            blang.Execute(code);
        });
    }

    // Ошибка: неверный тип в объявлении
    [Fact]
    public void Declaration_Wrong_Type_Should_Fail()
    {
        string code = """
            string s = 10
            """;

        Assert.Throws<TypeException>(() =>
        {
            BlangInterpreter blang = new(_environment);
            blang.Execute(code);
        });
    }

    // Ошибка: возврат другого типа
    [Fact]
    public void Return_Wrong_Type_Should_Fail()
    {
        string code = """
            func num f() { return "abc" }
            """;

        Assert.Throws<TypeException>(() =>
        {
            BlangInterpreter blang = new(_environment);
            blang.Execute(code);
        });
    }

    // Ошибка: тип аргумента не совпадает
    [Fact]
    public void Function_Arg_Wrong_Type_Should_Fail()
    {
        string code = """
            func num sqr(num x) { return x*x };
            sqr("str")
            """;

        Assert.Throws<TypeException>(() =>
        {
            BlangInterpreter blang = new(_environment);
            blang.Execute(code);
        });
    }

    // Ошибка: нет return в функции с типом
    [Fact]
    public void Function_No_Return_Should_Fail()
    {
        string code = """
            func num f() { write(1) }
            """;

        Assert.Throws<TypeException>(() =>
        {
            BlangInterpreter blang = new(_environment);
            blang.Execute(code);
        });
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
