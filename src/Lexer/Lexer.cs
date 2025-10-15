using System.Globalization;

namespace Lexer;

/// <summary>
///  Лексический анализатор code. Поддерживает подмножество code, необходимое этому проекту.
/// </summary>
public class Lexer(string code)
{
    private static readonly Dictionary<string, TokenType> Keywords = new()
    {
        {
            "if", TokenType.If
        },
        {
            "else", TokenType.Else
        },
        {
            "while", TokenType.While
        },
        {
            "do", TokenType.Do
        },
        {
            "write", TokenType.Write
        },
        {
            "writeln", TokenType.Writeln
        },
        {
            "read", TokenType.Read
        },
        {
            "and", TokenType.And
        },
        {
            "or", TokenType.Or
        },
    };

    private readonly TextScanner _scanner = new(code);

    /// <summary>
    ///  Распознаёт следующий токен.
    ///  Дополнительные правила:
    ///   1) Если ввод закончился, то возвращаем токен EndOfFile
    ///   2) Пробельные символы пропускаются.
    /// </summary>
    public Token ParseToken()
    {
        SkipWhiteSpacesAndComments();

        if (_scanner.IsEnd())
        {
            return new Token(TokenType.EndOfFile);
        }

        char c = _scanner.Peek();
        if (char.IsLetter(c) || c == '_')
        {
            return ParseIdentifierOrKeyword();
        }

        if (char.IsAsciiDigit(c))
        {
            return ParseNumericLiteral();
        }

        if (c == '\"')
        {
            return ParseStringLiteral();
        }

        switch (c)
        {
            case ';':
                _scanner.Advance();
                return new Token(TokenType.Semicolon);
            case ',':
                _scanner.Advance();
                return new Token(TokenType.Comma);
            case '+':
                _scanner.Advance();
                return new Token(TokenType.PlusSign);
            case '-':
                _scanner.Advance();
                return new Token(TokenType.MinusSign);
            case '*':
                _scanner.Advance();
                return new Token(TokenType.MultiplySign);
            case '/':
                _scanner.Advance();
                return new Token(TokenType.DivideSign);
            case '%':
                _scanner.Advance();
                return new Token(TokenType.ModuloSign);
            case '^':
                _scanner.Advance();
                return new Token(TokenType.ExponentiationSign);
            case '=':
                _scanner.Advance();
                if (_scanner.Peek() == '=')
                {
                    _scanner.Advance();
                    return new Token(TokenType.LooseEquality);
                }

                return new Token(TokenType.Assignment);
            case '!':
                _scanner.Advance();
                if (_scanner.Peek() == '=')
                {
                    _scanner.Advance();
                    return new Token(TokenType.NotEqual);
                }

                return new Token(TokenType.Not);
            case '<':
                _scanner.Advance();
                if (_scanner.Peek() == '=')
                {
                    _scanner.Advance();
                    return new Token(TokenType.LessThanOrEqual);
                }

                return new Token(TokenType.LessThan);
            case '>':
                _scanner.Advance();
                if (_scanner.Peek() == '=')
                {
                    _scanner.Advance();
                    return new Token(TokenType.GreaterThanOrEqual);
                }

                return new Token(TokenType.GreaterThan);
            case '(':
                _scanner.Advance();
                return new Token(TokenType.OpenParenthesis);
            case ')':
                _scanner.Advance();
                return new Token(TokenType.CloseParenthesis);
            case '{':
                _scanner.Advance();
                return new Token(TokenType.OpenBraces);
            case '}':
                _scanner.Advance();
                return new Token(TokenType.CloseBraces);
        }

        _scanner.Advance();
        return new Token(TokenType.Error, new TokenValue(c.ToString()));
    }

    /// <summary>
    ///  Распознаёт идентификаторы и ключевые слова.
    ///  Идентификаторы обрабатываются по правилам:
    ///     identifier = [letter | '_' ], { letter | digit | '_' } ;
    ///     letter = "a" | "b" | .. | "z" | unicode_letter ;
    ///     digit = "0" | "1" | .. | "9" ;
    ///     unicode_letter — любая буква Unicode.
    /// </summary>
    private Token ParseIdentifierOrKeyword()
    {
        string value = _scanner.Peek().ToString();
        _scanner.Advance();

        for (char c = _scanner.Peek();
             char.IsLetter(c) || c == '_' || char.IsAsciiDigit(c);
             c = _scanner.Peek())
        {
            value += c;
            _scanner.Advance();
        }

        // Проверяем на совпадение с ключевым словом.
        if (Keywords.TryGetValue(value.ToLower(CultureInfo.InvariantCulture), out TokenType type))
        {
            return new Token(type);
        }

        // Возвращаем токен идентификатора.
        return new Token(TokenType.Identifier, new TokenValue(value));
    }

    /// <summary>
    ///  Распознаёт литерал числа по правилам:
    ///     number = digits_sequence, [ ".", digits_sequence ] ;
    ///     digits_sequence = digit { digit } ;
    ///     digit = "0" | "1" | ... | "9" ;
    /// </summary>
    private Token ParseNumericLiteral()
    {
        decimal value = GetDigitValue(_scanner.Peek());
        _scanner.Advance();

        // Читаем целую часть числа.
        for (char c = _scanner.Peek(); char.IsAsciiDigit(c); c = _scanner.Peek())
        {
            value = value * 10 + GetDigitValue(c);
            _scanner.Advance();
        }

        // Читаем дробную часть числа.
        if (_scanner.Peek() == '.')
        {
            _scanner.Advance();
            decimal factor = 0.1m;
            for (char c = _scanner.Peek(); char.IsAsciiDigit(c); c = _scanner.Peek())
            {
                _scanner.Advance();
                value += factor * GetDigitValue(c);
                factor *= 0.1m;
            }
        }

        return new Token(TokenType.NumericLiteral, new TokenValue(value));

        // Локальная функция для получения числа из символа цифры.
        int GetDigitValue(char c)
        {
            return c - '0';
        }
    }

    /// <summary>
    ///  Распознаёт литерал строки по правилам:
    ///     string = quote, { string_element }, quote ;
    ///     quote = "'" ;
    ///     string_element = char | escape_sequence ;
    ///     char = ^"'" ;
    /// </summary>
    private Token ParseStringLiteral()
    {
        _scanner.Advance();

        string contents = "";
        while (_scanner.Peek() != '\"')
        {
            if (_scanner.IsEnd())
            {
                // Ошибка: строка, не закрытая кавычкой.
                return new Token(TokenType.Error, new TokenValue(contents));
            }

            contents += _scanner.Peek();
            _scanner.Advance();
        }

        _scanner.Advance();

        return new Token(TokenType.StringLiteral, new TokenValue(contents));
    }

    /// <summary>
    ///  Пропускает пробельные символы и комментарии, пока не встретит что-либо иное.
    /// </summary>
    private void SkipWhiteSpacesAndComments()
    {
        do
        {
            SkipWhiteSpaces();
        }
        while (TryParseMultilineComment() || TryParseSingleLineComment());
    }

    /// <summary>
    ///  Пропускает пробельные символы, пока не встретит иной символ.
    /// </summary>
    private void SkipWhiteSpaces()
    {
        while (char.IsWhiteSpace(_scanner.Peek()))
        {
            _scanner.Advance();
        }
    }

    /// <summary>
    ///  Пропускает многострочный комментарий в виде `$$ ...текст $$`,
    ///  пока не встретит `$$`.
    /// </summary>
    private bool TryParseMultilineComment()
    {
        if (_scanner.Peek() == '$' && _scanner.Peek(1) == '$')
        {
            do
            {
                _scanner.Advance();
            }
            while (!(_scanner.Peek() == '$' && _scanner.Peek(1) == '$'));

            _scanner.Advance();
            _scanner.Advance();
            return true;
        }

        return false;
    }

    /// <summary>
    ///  Пропускает однострочный комментарий в виде `$ ...текст`,
    ///  пока не встретит конец строки (его оставляет) или конец файла.
    /// </summary>
    private bool TryParseSingleLineComment()
    {
        if (_scanner.Peek() == '$' && _scanner.Peek(1) != '$' )
        {
            do
            {
                _scanner.Advance();
            }
            while (_scanner.Peek() != '\n' && _scanner.Peek() != '\r' && !_scanner.IsEnd());
            return true;
        }

        return false;
    }
}