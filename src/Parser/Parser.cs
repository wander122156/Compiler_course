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
    ///     statement = if_statement
    ///             | expression (временно)
    /// </summary>
    private Row ParseStatement()
    {

        List<RuntimeValue> values = new List<RuntimeValue>();
        Token keyword = _tokens.Peek();
        switch (keyword.Type)
        {
            case TokenType.If:
                values.Add(ParseIfStatement());
                break;

            default:
                values.Add(ParseExpression());
                break;
        }

        ParseCodeDelimiter();
        Row result = new(values.ToArray());
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
    /// Разбирает аргументы команды If
    ///     if_statement = "if", "(", condition, ")", statement, [ "else", statement ]
    /// Реализовано:
    ///     "if", "(", condition, ")", statement 
    /// </summary>
    private RuntimeValue ParseIfStatement()
    {
        _tokens.Advance();

        SkipExpectedLexeme(TokenType.OpenParenthesis);
        RuntimeValue conditionResult = ParseCondition();
        SkipExpectedLexeme(TokenType.CloseParenthesis);

        // Выполняем then
        Row thenResult = ParseCompoundStatement();

        // else?
        if (_tokens.Peek().Type == TokenType.Else)
        {
            //_tokens.Advance(); // Пропускаем "else"
            //Row elseResult = ParseCompoundStatement();
            // Возвращаем результат соответствующей ветки
            //return ConvertToBoolean(conditionResult)
            //    ? thenResult
            //    : elseResult;
        }

        // если условие false и нет else - возвращаем пустой Row
        //return ConvertToBoolean(conditionResult) ? thenResult : new Row();
        return conditionResult;
    }

    /// <summary>
    /// Разбирает условие
    ///     condition = expression, [ comparison_operator, expression ] 
    /// Реализовано:
    ///     condition = expression, [ comparison_operator, expression ] 
    /// </summary>
    private RuntimeValue ParseCondition()
    {
        RuntimeValue left = ParseExpression();

        Token operationToken = _tokens.Peek();
        if (IsComparisonOperator(operationToken.Type))
        {
            _tokens.Advance();
            RuntimeValue right = ParseExpression();

            return EvaluateComparison(left, operationToken.Type, right);
        }

        return ConvertToBooleanValue(left);
    }

    /// <summary>
    /// Проверяет является ли токен оператором сравнения
    /// </summary>
    private bool IsComparisonOperator(TokenType type)
    {
        return type == TokenType.LooseEquality ||
               type == TokenType.NotEqual ||
               type == TokenType.LessThan ||
               type == TokenType.GreaterThan ||
               type == TokenType.GreaterThanOrEqual ||
               type == TokenType.LessThan ||
               type == TokenType.LessThanOrEqual;
    }

    /// <summary>
    /// Вычисляет операцию сравнения
    /// </summary>
    private RuntimeValue EvaluateComparison(RuntimeValue left, TokenType operation, RuntimeValue right)
    {
        if (operation == TokenType.LessThan)
        {
            if (left.Type == right.Type && left.Type == RuntimeValue.ValueType.Number)
            {
                bool result = (decimal)left.Value < (decimal)right.Value;
                return RuntimeValue.Boolean(result);
            }
        }

        throw new Exception($"Cannot compare {left.Type} and {right.Type} with < operator");
    }

    /// <summary>
    /// Преобразует RuntimeValue в boolean значение
    /// </summary>
    private RuntimeValue ConvertToBooleanValue(RuntimeValue value)
    {
        bool boolValue = ConvertToBoolean(value);
        return RuntimeValue.Boolean(boolValue);
    }

    /// <summary>
    /// Преобразует RuntimeValue в boolean значение
    /// </summary>
    private bool ConvertToBoolean(RuntimeValue value)
    {
        return value.Type switch
        {
            RuntimeValue.ValueType.Boolean => (bool)value.Value,
            RuntimeValue.ValueType.Number => (decimal)value.Value != 0,
            RuntimeValue.ValueType.String => !string.IsNullOrEmpty((string)value.Value),
            RuntimeValue.ValueType.Null => false,
            _ => false,
        };
    }

    /// <summary>
    /// Разбирает составной statement в фигурных скобках
    /// compound_statement = "{", { statement, [ ";" ] }, "}"
    /// Возвращает результат ПОСЛЕДНЕГО statement в блоке
    /// </summary>
    private Row ParseCompoundStatement()
    {
        SkipExpectedLexeme(TokenType.OpenBraces);

        Row result = new(); // Пустой результат по умолчанию

        // Выполняем все statements внутри блока
        // Каждый statement перезаписывает результат, сохраняя последний
        while (_tokens.Peek().Type != TokenType.CloseBraces)
        {
            result = ParseStatement(); // Сохраняем результат последнего statement

            // Если встретили конец файла до закрывающей скобки - ошибка
            if (_tokens.Peek().Type == TokenType.EndOfFile)
            {
                throw new UnexpectedLexemeException(TokenType.CloseBraces, _tokens.Peek());
            }
        }

        SkipExpectedLexeme(TokenType.CloseBraces);
        return result;
    }

    /// <summary>
    /// Разбирает список выражений, разделённый запятыми.
    /// Правила:
    ///     expression = expression, { ",", expression } ;
    /// </summary>
    private Row ParseExpressionList()
    {
        List<RuntimeValue> values =
        [
            ParseExpression(),
        ];
        while (_tokens.Peek().Type == TokenType.Comma)
        {
            _tokens.Advance();
            values.Add(ParseExpression());
        }

        Row result = new(values.ToArray());
        return result;
    }

    /// <summary>
    /// Разбирает одно выражение.
    /// Правила:
    ///     expression = term_expression, { ("+" | "-"), term_expression }
    /// </summary>
    private RuntimeValue ParseExpression()
    {
        RuntimeValue value = ParseTermExpression();
        while (true)
        {
            switch (_tokens.Peek().Type)
            {
                case TokenType.PlusSign:
                    _tokens.Advance();
                    value = EvaluateArithmetic(value, ParseTermExpression(), TokenType.PlusSign);
                    break;
                case TokenType.MinusSign:
                    _tokens.Advance();
                    value = EvaluateArithmetic(value, ParseTermExpression(), TokenType.MinusSign);
                    break;
                default:
                    return value;
            }
        }
    }

    /// <summary>
    ///  Разбирает один операнд сложения/вычитания.
    ///  Правила:
    ///     term_expression = factor_expression, { ("*" | "/" | "%"), factor_expression }
    /// </summary>
    private RuntimeValue ParseTermExpression()
    {
        RuntimeValue value = ParseFactorExpression();
        while (true)
        {
            switch (_tokens.Peek().Type)
            {
                case TokenType.MultiplySign:
                    _tokens.Advance();
                    value = EvaluateArithmetic(value, ParseTermExpression(), TokenType.MultiplySign);
                    break;
                case TokenType.DivideSign:
                    _tokens.Advance();
                    value = EvaluateArithmetic(value, ParseTermExpression(), TokenType.DivideSign);
                    break;
                case TokenType.ModuloSign:
                    _tokens.Advance();
                    value = EvaluateArithmetic(value, ParseTermExpression(), TokenType.ModuloSign);
                    break;

                default:
                    return value;
            }
        }
    }

    /// <summary>
    ///  Разбирает один операнд умножения / деления.
    ///  Правило:
    ///     factor_expression = ("+" | "-"), factor_expression | exponentiation_expression
    private RuntimeValue ParseFactorExpression()
    {
        if (_tokens.Peek().Type == TokenType.MinusSign)
        {
            _tokens.Advance();
            RuntimeValue operand = ParseFactorExpression(); // Рекурсивно

            if (operand.Type == RuntimeValue.ValueType.Number)
            {
                return new RuntimeValue(RuntimeValue.ValueType.Number, -(decimal)operand.Value);
            }
            else
            {
                throw new Exception($"Unary minus cannot be applied to {operand.Type}");
            }
        }
        else if (_tokens.Peek().Type == TokenType.PlusSign)
        {
            _tokens.Advance();
            return ParseFactorExpression(); // Рекурсивно (если + просто скипаем, они ничего не меняют)
        }
        else
        {
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
    ///     simple_expression = number | string | function_call | const_expression(true, false, MathE, Pi)
    /// </summary>
    private RuntimeValue ParseSimpleExpression()
    {
        Token t = _tokens.Peek();

        if (t.Type == TokenType.StringLiteral)
        {
            _tokens.Advance();
            return RuntimeValue.String(t.Value!.ToString());
        }

        if (t.Type == TokenType.NumericLiteral)
        {
            _tokens.Advance();
            return RuntimeValue.Number(t.Value!.ToDecimal());
        }

        // Пока что константы и встроенные функции
        if (t.Type == TokenType.Identifier)
        {
            string identifier = t.Value!.ToString();

            switch (identifier)
            {
                case "true":
                    _tokens.Advance();
                    return RuntimeValue.Boolean(true);
                case "false":
                    _tokens.Advance();
                    return RuntimeValue.Boolean(false);
                case "MathE":
                    _tokens.Advance();
                    return RuntimeValue.Number((decimal)Math.E);
                case "Pi":
                    _tokens.Advance();
                    return RuntimeValue.Number((decimal)Math.PI);
                default:
                    //func_call
                    _tokens.Advance();
                    SkipExpectedLexeme(TokenType.OpenParenthesis);
                    Row arguments = ParseExpressionList();
                    SkipExpectedLexeme(TokenType.CloseParenthesis);
                    return RuntimeValue.Number(BuiltinFunctions.Invoke(t.Value!.ToString(), ConvertToDecimalList(arguments)));
            }
        }

        throw new UnexpectedLexemeException(TokenType.Identifier, t);
    }

    private List<decimal> ConvertToDecimalList(Row runtimeValues)
    {
        List<decimal> decimals = new List<decimal>();

        for(int i = 0; i < runtimeValues.ColumnCount; i++)
        {
            if (runtimeValues[i].Type == RuntimeValue.ValueType.Number)
            {
                decimals.Add((decimal)runtimeValues[i].Value);
            }
            else
            {
                throw new Exception($"Function argument must be number, got {runtimeValues[i].Type}");
            }
        }

        return decimals;
    }

    private void ParseCodeDelimiter()
    {
        Token t = _tokens.Peek();
        switch (t.Type)
        {
            case TokenType.CloseBraces:
                //_tokens.Advance();
                break;
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

    /// <summary>
    /// Вычисляет арифметическую бинарную операцию
    /// </summary>
    private RuntimeValue EvaluateArithmetic(RuntimeValue left, RuntimeValue right, TokenType operation)
    {
        if (left.Type == RuntimeValue.ValueType.Number && right.Type == RuntimeValue.ValueType.Number)
        {
            decimal leftNum = (decimal)left.Value;
            decimal rightNum = (decimal)right.Value;

            decimal result = operation switch
            {
                TokenType.PlusSign => leftNum + rightNum,
                TokenType.MinusSign => leftNum - rightNum,
                TokenType.MultiplySign => leftNum * rightNum,
                TokenType.DivideSign => rightNum != 0 ? leftNum / rightNum : throw new DivideByZeroException("Division by zero"),
                TokenType.ModuloSign => rightNum != 0 ? leftNum % rightNum : throw new DivideByZeroException("Modulo by zero"),
                _ => 0
            };

            return RuntimeValue.Number(result);
        }

        throw new Exception($"Unsupported arithmetic operation between {left.Type} and {right.Type}");
    }
}