using Blang.Common;
using Blang.Execution;
using Blang.Interpreter;
using Blang.Parser;

namespace Blang.Semantic.UnitTests;

public class TypesTests
{
    private const int Precision = 5;
    private static readonly decimal Tolerance = (decimal)Math.Pow(0.1, Precision);
    private readonly FakeEnvironment _environment;

    public TypesTests()
    {
        _environment = new FakeEnvironment();
    }

    // Приведение типов к bool
    [Fact]
    public void If_String_Condition_Convert_to_bool_true()
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
    public void If_Number_Condition_Convert_to_bool_true()
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

    [Fact]
    public void If_String_Condition_Convert_to_bool_false()
    {
        string code = """
            if ("") {write(2)} else {write(1)}
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
    public void If_Number_Condition_Convert_to_bool_false()
    {
        string code = """
            if (0) {write(2)} else {write(1)}
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
    public void CompareStrings()
    {
        string code = """
        if ("apple" == "apple") { write(1) };
        if ("apple" != "apples") { write(2) };
        if ("apple" < "apple") { write(3) };
        if ("apples" > "apple") { write(4) };
        if ("apples" >= "apple") { write(5) };
        if ("apple" <= "apples") { write(6) };
        """;

        List<RuntimeValue> expected = [
            RuntimeValue.Number(1),
            RuntimeValue.Number(2),
            RuntimeValue.Number(4),
            RuntimeValue.Number(5),
            RuntimeValue.Number(6),
        ];

        BlangInterpreter blang = new(_environment);
        blang.Execute(code);

        IReadOnlyList<RuntimeValue> actual = _environment.Results;
        AssertResults(expected, actual);
    }

    [Fact]
    public void Write_Boolean_Values()
    {
        string code = """
        write(true);
        write(false);
        """;

        List<RuntimeValue> expected = [
            RuntimeValue.Boolean(true),
            RuntimeValue.Boolean(false),
        ];

        BlangInterpreter blang = new(_environment);
        blang.Execute(code);

        IReadOnlyList<RuntimeValue> actual = _environment.Results;
        AssertResults(expected, actual);
    }

    [Fact]
    public void Read_String_Values()
    {
        string code = """
        string s;
        read(s);
        write(s);
        """;

        List<RuntimeValue> expected = [
        RuntimeValue.String("qwerty"),
        ];

        _environment.SetInputLines("qwerty");

        BlangInterpreter blang = new(_environment);
        blang.Execute(code);

        IReadOnlyList<RuntimeValue> actual = _environment.Results;
        AssertResults(expected, actual);
    }

    [Fact]
    public void Concat_String_Values()
    {
        string code = """
        string abc = "ab" + "c";
        write(abc);
        string fg = "fg";
        string abcfg = abc + fg;
        write(abcfg);
        """;

        List<RuntimeValue> expected = [
        RuntimeValue.String("abc"),
        RuntimeValue.String("abcfg"),
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
