using Blang.Common;
using Blang.Execution;
using Blang.Interpreter;

namespace Blang.Parser.UnitTests;

public class ParserVariableAndConstantTests
{
    private readonly Context _context;
    private readonly FakeEnvironment _environment;

    public ParserVariableAndConstantTests()
    {
        _context = new Context();
        _environment = new FakeEnvironment();
    }

    public static TheoryData<string, List<decimal>> ValidCodeTestData => new()
    {
        // Объявления переменных
        { "num x = 3; write(x)", [3] },
        { "const num c = 3; write(c)", [3] },

        // Множественные объявления
        { "num x = 1, y = 2, z = 3; write(x); write(y); write(z)", [1, 2, 3] },
        { "num x, y; x = 10; y = 12; write(x); write(y)", [10, 12] },
        { "num a = 1, b = 2; a = 5; b = a + 1; write(a); write(b)", [5, 6] },

        // Выражения с переменными
        { "num x = 1, y = 2, z = 3; num result = x + y * z; write(result)", [7] },
        { "num x, y; x = 10; y = 12; num sum = x + y; write(sum)", [22] },
        { "num a = 1 + 2 * 3; write(a)", [7] },
        { "num a = 5; num b = a * 2; write(b)", [10] },

        // Константы и выражения
        { "const num PI = 3.14; num radius = 2; num area = PI * radius * radius; write(area)", [12.56m] },
        { "const num c = 3.14159; num c = 2; num result = 4.0 * c * 4.0; write(result)", [32m] },

        // Множественные выражения с выводом
        { "num a = 1 + 2; write(a); num b = 2 * 5; write(b); num c = 4.5; write(c)", [3, 10, 4.5m] },
    };

    [Theory]
    [MemberData(nameof(ValidCodeTestData))]
    public void Can_parse_valid_code(string code, List<decimal> expected)
    {
        BlangInterpreter blang = new(_environment);
        blang.Execute(code);

        IReadOnlyList<RuntimeValue> actual = _environment.Results;
        AssertResults(expected, actual);
    }

    // Обработка ошибок
    [Fact]
    public void Throws_on_invalid_identifier_in_declaration()
    {
        string code = "num 123";
        BlangInterpreter blang = new(_environment);

        Assert.Throws<UnexpectedLexemeException>(() => blang.Execute(code));
    }

    [Fact]
    public void Throws_on_missing_identifier_in_const_declaration()
    {
        string code = "const num = 5";
        BlangInterpreter blang = new(_environment);

        Assert.Throws<UnexpectedLexemeException>(() => blang.Execute(code));
    }

    [Fact]
    public void Throws_on_missing_expression_in_assignment()
    {
        string code = "num x = ;";
        BlangInterpreter blang = new(_environment);

        Assert.Throws<UnexpectedLexemeException>(() => blang.Execute(code));
    }

    [Fact]
    public void Throws_on_assignment_to_undefined_variable()
    {
        string code = "x = 5";
        BlangInterpreter blang = new(_environment);

        Assert.Throws<ArgumentException>(() => blang.Execute(code));
    }

    [Fact]
    public void Throws_on_assignment_to_constant()
    {
        string code = "const num c = 5; c = 10";
        BlangInterpreter blang = new(_environment);

        Assert.Throws<ArgumentException>(() => blang.Execute(code));
    }

    [Fact]
    public void Throws_on_not_initialized_constant()
    {
        string code = "const num c;";
        BlangInterpreter blang = new(_environment);

        Assert.Throws<UnexpectedLexemeException>(() => blang.Execute(code));
    }

    private void AssertResults(List<decimal> expected, IReadOnlyList<RuntimeValue> actual)
    {
        if (expected.Count != actual.Count)
        {
            Assert.Fail(
                $"Actual results count does not match expected. Expected: {expected.Count}, Actual: {actual.Count}.\n" +
                $"Expected: [{string.Join(", ", expected)}]\n" +
                $"Actual: [{string.Join(", ", actual.Select(r => $"{r.Value}"))}]"
            );
        }

        for (int i = 0; i < expected.Count; i++)
        {
            Assert.Equal(expected[i], (decimal)actual[i].Value, 5);
        }
    }
}