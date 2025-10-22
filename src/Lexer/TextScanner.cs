namespace Blang.Lexer;

/// <summary>
///  Сканирует текст code-запроса, предоставляя три операции: Peek(N), Advance() и IsEnd().
/// </summary>
public class TextScanner(string code)
{
    private readonly string _code = code;
    private int _position;

    /// <summary>
    ///  Читает на N символов вперёд текущей позиции (по умолчанию N=0).
    /// </summary>
    public char Peek(int n = 0)
    {
        int position = _position + n;
        return position >= _code.Length ? '\0' : _code[position];
    }

    /// <summary>
    ///  Сдвигает текущую позицию на один символ.
    /// </summary>
    public void Advance()
    {
        _position++;
    }

    public bool IsEnd()
    {
        return _position >= _code.Length;
    }
}
