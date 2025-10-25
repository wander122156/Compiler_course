using Blang.Lexer;

namespace Parser;

/// <summary>
/// Представляет поток токенов с двумя операциями:
///  - Peek() возвращает текущий токен
///  - Advance() переходит к следующему токену
/// </summary>
public class TokenStream
{
    private readonly Lexer _lexer;
    private Token _nextToken;

    public TokenStream(string sql)
    {
        _lexer = new Lexer(sql);
        _nextToken = _lexer.ParseToken();
    }

    public Token Peek()
    {
        return _nextToken;
    }

    public void Advance()
    {
        _nextToken = _lexer.ParseToken();
    }
}
