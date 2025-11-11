using Blang.Common;
using Blang.Execution;

namespace Blang.Parser.UnitTests;

public class ParseTopLevelStatementsTest
{
    private readonly Context _context;
    private readonly FakeEnvironment _environment;

    public ParseTopLevelStatementsTest()
    {
        _context = new Context();
        _environment = new FakeEnvironment();
    }

    [Fact]
    public void Can_parse_single_variable_declaration_without_initialization()
    {
        string code = "num x ";
        List<decimal> expected = [0];

        Parser parser = new(_context, _environment, code);
        parser.ParseProgram();

        IReadOnlyList<RuntimeValue> actual = _environment.Results;

        AssertResults(expected, actual);
    }

    [Fact]
    public void Can_parse_single_variable_declaration_with_initialization()
    {
        string code = "num x = 3";
        List<decimal> expected = [3];

        Parser parser = new(_context, _environment, code);
        parser.ParseProgram();

        IReadOnlyList<RuntimeValue> actual = _environment.Results;

        AssertResults(expected, actual);
    }

    [Fact]
    public void Can_parse_multiple_variable_declarations_with_expressions()
    {
        string code = "num x = 1, y = 2, z = 3 ; x + y * z";
        List<decimal> expected = [3, 7];

        Parser parser = new(_context, _environment, code);
        parser.ParseProgram();

        IReadOnlyList<RuntimeValue> actual = _environment.Results;

        AssertResults(expected, actual);
    }

    [Fact]
    public void Can_parse_variable_declarations_and_assignments()
    {
        string code = "num x, y; x = 10; y = 12 ; x + y";
        List<decimal> expected = [0, 10, 12, 22];

        Parser parser = new(_context, _environment, code);
        parser.ParseProgram();

        IReadOnlyList<RuntimeValue> actual = _environment.Results;

        AssertResults(expected, actual);
    }

    [Fact]
    public void Can_parse_multiple_expressions_separated_by_semicolons()
    {
        string code = "1 + 2; 2 * 5; 4.5";
        List<decimal> expected = [3, 10, 4.5m];

        Parser parser = new(_context, _environment, code);
        parser.ParseProgram();

        IReadOnlyList<RuntimeValue> actual = _environment.Results;

        AssertResults(expected, actual);
    }

    [Fact]
    public void Can_parse_constant_declaration_with_initialization()
    {
        string code = "const num c = 3";
        List<decimal> expected = [3];

        Parser parser = new(_context, _environment, code);
        parser.ParseProgram();

        IReadOnlyList<RuntimeValue> actual = _environment.Results;

        AssertResults(expected, actual);
    }

    [Fact]
    public void Can_parse_variable_reassignment_in_sequence()
    {
        string code = "num a = 1, b = 2 ; a = 5 ; b = a + 1 ";
        List<decimal> expected = [2, 5, 6];

        Parser parser = new(_context, _environment, code);
        parser.ParseProgram();

        IReadOnlyList<RuntimeValue> actual = _environment.Results;

        AssertResults(expected, actual);
    }

    [Fact]
    public void Can_parse_variable_shadowing_constant()
    {
        string code = "const num c = 3.14159; num c = 2 ; 4.0 * c * 4.0;";
        List<decimal> expected = [3.14159m, 2, 32m];

        Parser parser = new(_context, _environment, code);
        parser.ParseProgram();

        IReadOnlyList<RuntimeValue> actual = _environment.Results;

        AssertResults(expected, actual);
    }

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

    private void AssertResults(List<decimal> expected, IReadOnlyList<RuntimeValue> actual)
    {
        for (int i = 0, iMax = Math.Min(expected.Count, actual.Count); i < iMax; ++i)
        {
            Assert.Equal(expected[i], (decimal)actual[i].Value, 5);
        }

        if (expected.Count != actual.Count)
        {
            Assert.Fail(
                $"Actual results count does not match expected. Expected: {expected.Count}, Actual: {actual.Count}."
            );
        }
    }
}