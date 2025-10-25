using Blang.Lexer;

namespace Parser;

/// <summary>
/// Выполняет синтаксический разбор строк кода.
/// </summary>
public class Parser
{
    private readonly TokenStream _tokens;

    private Parser(string code)
    {
        _tokens = new TokenStream(code);
    }

    /// <summary>
    /// Выполняет код и возвращает результат.
    /// </summary>
    public static Row ExecuteCode(string code)
    {
        Parser p = new(code);
        return p.ParseCode();
    }

    /// <summary>
    /// Выполняет код и возвращает результат.
    /// Поддерживает правила:
    ///     program = { statement, [ ";" ] }
    ///
    /// Реализовано:
    ///     program =  write_statement, ";"
    /// </summary>
    private Row ParseCode()
    {
        Row result = ParseStatement();
        ParseCodeDelimiter();

        return result;
    }

    /// <summary>
    /// Разбирает 1 из statement.
    /// statement = write_statement
    ///      | read_statement
    ///      | assignment_statement
    ///      | if_statement
    ///      | while_statement
    ///      | compound_statement
    /// Реализовано:
    ///     statement = write_statemen 
    /// </summary>
    private Row ParseStatement()
    {
        Row result;
        Token keyword = _tokens.Peek();
        switch (keyword.Type)
        {
            case TokenType.Write:
                result = ParseWriteStatement();
                break;

            default:
                throw new UnexpectedLexemeException(keyword.Type, keyword);
        }

        ParseCodeDelimiter();

        return result;
    }

    /// <summary>
    /// Разбирает аргументы команды write
    /// write_statement = "write", "( ", [ expression_list ], " )"
    /// </summary>
    private Row ParseWriteStatement()
    {
        _tokens.Advance();
        SkipExpectedLexeme(TokenType.OpenParenthesis);

        List<RuntimeValue> values = new List<RuntimeValue>();

        // Первое выражение
        values.Add(ParseExpression());

        // Остальные выражения через запятую
        while (_tokens.Peek().Type == TokenType.Comma)
        {
            SkipExpectedLexeme(TokenType.Comma);
            values.Add(ParseExpression());
        }

        SkipExpectedLexeme(TokenType.CloseParenthesis);

        return new Row(values.ToArray());
    }

    /// <summary>
    /// Разбирает одно выражение.
    /// Правила:
    ///     expression = term_expression, { ("+" | "-"), term_expression }
    ///
    /// Реализовано:
    ///     expression = term_expression {}
    /// </summary>
    private RuntimeValue ParseExpression()
    {
        RuntimeValue value = ParseTermExpression();
        while (true)
        {
            switch (_tokens.Peek().Type)
            {
                default:
                    return value;
            }
        }
    }

    /// <summary>
    ///  Разбирает один операнд сложения/вычитания.
    ///  Правила:
    ///     term_expression = factor_expression, { ("*" | "/"), factor_expression }
    ///  Реализовано:
    ///     term_expression = factor_expression {}
    /// </summary>
    private RuntimeValue ParseTermExpression()
    {
        RuntimeValue value = ParseFactorExpression();
        while (true)
        {
            switch (_tokens.Peek().Type)
            {
                default:
                    return value;
            }
        }
    }

    /// <summary>
    ///  Разбирает один операнд умножения / деления.
    ///  Правило:
    ///     factor_expression = [ "+" | "-" ], exponentiation_expression ;
    ///  Реализовано:
    ///     factor_expression = exponentiation_expression
    /// </summary>
    private RuntimeValue ParseFactorExpression()
    {
        switch (_tokens.Peek().Type)
        {
            default:
                return ParseExponentiationExpression();
        }
    }

    /// <summary>
    ///  Разбирает одну операцию возведения в степень.
    ///  Правило:
    ///     exponentiation_expression = simple_expression, { ("^"), exponentiation_expression }
    ///  Реализовано:
    ///     exponentiation_expression = simple_expression
    /// </summary>
    private RuntimeValue ParseExponentiationExpression()
    {
        RuntimeValue value = ParseSimpleExpression();

        return value;
    }

    /// <summary>
    ///  Разбирает простейшую часть выражения.
    ///     simple_expression = number | string | identifier | function_call | "(", expression, ")" | const_expression
    ///  Реализовано:
    ///     simple_expression = string 
    /// </summary>
    private RuntimeValue ParseSimpleExpression()
    {
        Token t = _tokens.Peek();
        if (t.Type == TokenType.StringLiteral)
        {
            _tokens.Advance();
            return RuntimeValue.String(t.Value!.ToString());
        }

        throw new UnexpectedLexemeException(TokenType.StringLiteral, t);
    }

    private void ParseCodeDelimiter()
    {
        Token t = _tokens.Peek();
        switch (t.Type)
        {
            case TokenType.Semicolon:
                _tokens.Advance();
                break;
            case TokenType.EndOfFile:
                break;
            default:
                throw new UnexpectedLexemeException(TokenType.Semicolon, t);
        }
    }

    /// <summary>
    /// Пропускает ожидаемую лексему либо бросает исключение, если встретит иную лексему.
    /// </summary>
    private void SkipExpectedLexeme(TokenType expected)
    {
        Token t = _tokens.Peek();
        if (t.Type != expected)
        {
            throw new UnexpectedLexemeException(expected, t);
        }

        _tokens.Advance();
    }
}