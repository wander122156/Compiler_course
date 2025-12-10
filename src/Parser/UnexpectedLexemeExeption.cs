using Blang.Lexer;

namespace Blang.Parser;

#pragma warning disable RCS1194 // Конструкторы исключения не нужны, т.к. это не класс общего назначения.
public class UnexpectedLexemeException : Exception
{
    public UnexpectedLexemeException(TokenType expected, Token actual)
        : base($"Unexpected lexeme {actual} where expected {expected}")
    {
    }

    public UnexpectedLexemeException(TokenType expected)
        : base($"Unexpected lexeme {expected}")
    {
    }
}
#pragma warning restore RCS1194
