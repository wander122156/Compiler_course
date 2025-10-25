using Blang.Lexer;

namespace Parser;

/// <summary>
/// Выполняет синтаксический разбор строк кода.
/// </summary>
public class Parser
{
    private Lexer _lexer;

    private Parser(string sql)
    {
        _lexer = new Lexer(sql);
    }

    /// <summary>
    /// Выполняет код и возвращает результат.
    /// </summary>
    public static string ExecuteCode(string code)
    {
        Parser p = new(code);
        return p.Parsecode();
    }

    /// <summary>
    /// Выполняет SQL-запрос и возвращает результат.
    /// Поддерживает правила:
    ///  code = select_statement, [ ";" ] ;
    /// </summary>
    private string Parsecode()
    {
        Token keyword = _lexer.ParseToken();
        if (keyword.Type != TokenType.Write)
        {
            _tokens.Advance();
            value = ParseExpression();
        }
        else
        {
            throw new UnexpectedLexemeException(TokenType.Write, keyword);
        }

        Token openParenthesis = _lexer.ParseToken();
        if (openParenthesis.Type != TokenType.OpenParenthesis)
        {
            throw new UnexpectedLexemeException(TokenType.StringLiteral, openParenthesis);
        }

        Token value = _lexer.ParseToken();
        if (value.Type != TokenType.StringLiteral)
        {
            throw new UnexpectedLexemeException(TokenType.StringLiteral, value);
        }

        Token closeParenthesis = _lexer.ParseToken();
        if (closeParenthesis.Type != TokenType.CloseParenthesis)
        {
            throw new UnexpectedLexemeException(TokenType.StringLiteral, closeParenthesis);
        }

        Token end = _lexer.ParseToken();
        if (end.Type == TokenType.Semicolon)
        {
            end = _lexer.ParseToken();
        }

        if (end.Type != TokenType.EndOfFile)
        {
            throw new UnexpectedLexemeException(TokenType.EndOfFile, end);
        }

        return value.Value!.ToString();
    }
}
