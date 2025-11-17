using Blang.Common;
using Blang.Execution;

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
        { "num x ", [0] },
        { "num x = 3", [3] },
        { "const num c = 3", [3] },
        { "num x = 1, y = 2, z = 3 ; x + y * z", [3, 7] },
        { "num x, y; x = 10; y = 12 ; x + y", [0, 10, 12, 22] },
        { "num a = 1, b = 2 ; a = 5 ; b = a + 1 ", [2, 5, 6] },
        { "1 + 2; 2 * 5; 4.5", [3, 10, 4.5m] },
        { "const num c = 3.14159; num c = 2 ; 4.0 * c * 4.0;", [3.14159m, 2, 32m] },
        { "num x = 1 + 2 * 3", [7] },
        { "num a = 5; num b = a * 2", [5, 10] },
        { "const num PI = 3.14; num radius = 2; PI * radius * radius", [3.14m, 2, 12.56m] },
    };

    [Theory]
    [MemberData(nameof(ValidCodeTestData))]
    public void Can_parse_valid_code(string code, List<decimal> expected)
    {
        // Arrange
        Parser parser = new(_context, _environment, code);

        // Act
        parser.ParseProgram();

        // Assert
        IReadOnlyList<RuntimeValue> actual = _environment.Results;
        AssertResults(expected, actual);
    }

    // Обработка ошибок
    [Fact]
    public void Throws_on_undefined_variable_without_declarations()
    {
        string code = "x + 1";
        Parser parser = new(_context, _environment, code);

        Assert.Throws<ArgumentException>(() => parser.ParseProgram());
    }

    [Fact]
    public void Throws_on_undefined_variable_in_complex_expression()
    {
        string code = "num x, y; x + y + z";
        Parser parser = new(_context, _environment, code);

        Assert.Throws<ArgumentException>(() => parser.ParseProgram());
    }

    [Fact]
    public void Throws_on_invalid_identifier_in_declaration()
    {
        string code = "num 123";
        Parser parser = new(_context, _environment, code);

        Assert.Throws<UnexpectedLexemeException>(() => parser.ParseProgram());
    }

    [Fact]
    public void Throws_on_missing_identifier_in_const_declaration()
    {
        string code = "const num = 5";
        Parser parser = new(_context, _environment, code);

        Assert.Throws<UnexpectedLexemeException>(() => parser.ParseProgram());
    }

    [Fact]
    public void Throws_on_missing_expression_in_assignment()
    {
        string code = "num x = ;";
        Parser parser = new(_context, _environment, code);

        Assert.Throws<UnexpectedLexemeException>(() => parser.ParseProgram());
    }

    [Fact]
    public void Throws_on_assignment_to_undefined_variable()
    {
        string code = "x = 5";
        Parser parser = new(_context, _environment, code);

        Assert.Throws<ArgumentException>(() => parser.ParseProgram());
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