using Blang.Common;
using Blang.Execution;

namespace Blang.Parser.UnitTests;

public class ParseTopLevelStatementsTest
{
    private const decimal Precision = 5m;

    private readonly Context _context;
    private readonly FakeEnvironment _environment;

    public ParseTopLevelStatementsTest()
    {
        _context = new Context();
        _environment = new FakeEnvironment();
    }

    [Fact]
    public void Can_parse_single_mutable_variable_declaration()
    {
        string code = "int x = 3 in x + 1";
        List<decimal> expected = [4];

        Parser parser = new(_context, _environment, code);
        parser.ParseProgram();

        IReadOnlyList<RuntimeValue> actual = _environment.Results;

        AssertResults(expected, actual);
    }

    [Fact]
    public void Can_parse_multiple_mutable_variable_declarations()
    {
        string code = "int x = 1, y = 2, z = 3 in x + y * z";
        List<decimal> expected = [7];

        Parser parser = new(_context, _environment, code);
        parser.ParseProgram();

        IReadOnlyList<RuntimeValue> actual = _environment.Results;

        AssertResults(expected, actual);
    }

    [Fact]
    public void Can_parse_multiple_assignment()
    {
        string code = "int x, y in x = y = 10 : x + y";
        List<decimal> expected = [20];

        Parser parser = new(_context, _environment, code);
        parser.ParseProgram();

        IReadOnlyList<RuntimeValue> actual = _environment.Results;

        AssertResults(expected, actual);
    }

    [Fact]
    public void Can_parse_expressions_separated_by_semicolons()
    {
        string code = "1 + 2; 2 * 5; 4.5";
        List<decimal> expected = [3, 10, 4.5m];

        Parser parser = new(_context, _environment, code);
        parser.ParseProgram();

        IReadOnlyList<RuntimeValue> actual = _environment.Results;

        AssertResults(expected, actual);
    }

    [Fact]
    public void Can_parse_constant_declaration()
    {
        string code = "const int Pi 3.14159; 4.0 * Pi * 4.0;";
        List<decimal> expected = [3.14159m, 50.26544m];

        Parser parser = new(_context, _environment, code);
        parser.ParseProgram();

        IReadOnlyList<RuntimeValue> actual = _environment.Results;

        AssertResults(expected, actual);
    }

    [Fact]
    public void Can_parse_declared_variables_in_complex_expression()
    {
        string code = "int a = 1, b = 2 in (a = 5 : b = a + 1) : b";
        List<decimal> expected = [6];

        Parser parser = new(_context, _environment, code);
        parser.ParseProgram();

        IReadOnlyList<RuntimeValue> actual = _environment.Results;

        AssertResults(expected, actual);
    }

    [Fact]
    public void Can_parse_variables_hiding_constants()
    {
        string code = "const int Pi 3.14159; int Pi = 2 in 4.0 * Pi * 4.0;";
        List<decimal> expected = [3.14159m, 32m];

        Parser parser = new(_context, _environment, code);
        parser.ParseProgram();

        IReadOnlyList<RuntimeValue> actual = _environment.Results;

        AssertResults(expected, actual);
    }

    [Fact]
    public void Throws_on_undefined_variable_no_declarations()
    {
        string code = "x + 1"; // Нет объявления переменных

        Parser parser = new(_context, _environment, code);

        Assert.Throws<ArgumentException>(() => parser.ParseProgram());
    }

    [Fact]
    public void Throws_on_undefined_variable_in_expression()
    {
        string code = "int x, y in x + y + z"; // Одна из переменных не объявлена

        Parser parser = new(_context, _environment, code);

        Assert.Throws<ArgumentException>(() => parser.ParseProgram());
    }

    [Fact]
    public void Throws_on_variable_declaration_without_in()
    {
        string code = "int x = 5"; // Разбор объявления переменной без in

        Parser parser = new(_context, _environment, code);

        Assert.Throws<UnexpectedLexemeException>(() => parser.ParseProgram());
    }

    [Fact]
    public void Throws_on_variable_declaration_with_empty_identifier()
    {
        string code = "int in x + 1"; // Разбор объявления переменной с пустым идентификатором

        Parser parser = new(_context, _environment, code);

        Assert.Throws<UnexpectedLexemeException>(() => parser.ParseProgram());
    }

    [Fact]
    public void Throws_on_variable_declaration_with_empty_expression()
    {
        string code = "int x in"; // Разбор объявления переменной с пустым выражением

        Parser parser = new(_context, _environment, code);

        Assert.Throws<UnexpectedLexemeException>(() => parser.ParseProgram());
    }

    [Fact]
    public void Throws_on_variable_declaration_with_invalid_identifier()
    {
        string code = "int 123 in x + 1"; // Разбор объявления переменной с неправильным идентификатором

        Parser parser = new(_context, _environment, code);

        Assert.Throws<UnexpectedLexemeException>(() => parser.ParseProgram());
    }

    private void AssertResults(List<decimal> expected, IReadOnlyList<RuntimeValue> actual)
    {
        for (int i = 0, iMax = Math.Min(expected.Count, actual.Count); i < iMax; ++i)
        {
            Assert.Equal((double)expected[i], (double)actual[i], (double)Precision);
        }

        if (expected.Count != actual.Count)
        {
            Assert.Fail(
                $"Actual results count does not match expected. Expected: {expected.Count}, Actual: {actual.Count}."
            );
        }
    }
}