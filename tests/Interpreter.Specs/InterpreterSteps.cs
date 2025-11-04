using Parser;

using Xunit;

namespace Interpreter.Specs;

public class InterpreterTests
{
    private const int Precision = 5;

    [Fact]
    public void ExecuteProgram_SimpleArithmetic_ReturnsCorrectResult()
    {
        // Arrange
        string code = "1 + 2 * 3";
        var expectedResults = new[] { 7.0 };

        // Act
        List<RuntimeValue> result = Parser.ExecuteCode(code);

        // Assert
        Assert.Single(result); // Проверяем что одна колонка
        Assert.Equal(expectedResults[0], (double)(decimal)result[0].Value, Precision);
    }

    [Fact]
    public void ExecuteProgram_MultipleExpressions_ReturnsCorrectResults()
    {
        // Arrange
        string code = "2 + 3 : 5 * 2 : 10 - 1";
        var expectedResults = new[] { 5.0, 10.0, 9.0 };

        // Act
        List<RuntimeValue> result = Parser.ExecuteCode(code);

        // Assert
        Assert.Equal(expectedResults.Length, result.Count);
        for (int i = 0; i < expectedResults.Length; i++)
        {
            Assert.Equal(expectedResults[i], (double)(decimal)result[i].Value, Precision);
        }
    }
}