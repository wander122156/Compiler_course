using Blang.Lexer;

namespace Parser;

/// <summary>
/// Представляет поток токенов с двумя операциями:
///  - Peek() возвращает текущий токен
///  - Advance() переходит к следующему токену
/// </summary>
public class TokenStream(string code)
{
    private readonly Lexer _lexer = new(code);

    private Token _nextToken;

    public Token Peek()
    {
        return _nextToken;
    }

    public void Advance()
    {
        _nextToken = _lexer.ParseToken();
    }
}
