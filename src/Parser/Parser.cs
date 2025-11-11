using System;

using Blang.Common;
using Blang.Execution;
using Blang.Lexer;

namespace Blang.Parser;

/// <summary>
/// Выполняет синтаксический разбор строк кода.
/// </summary>
public class Parser
{
    private readonly Context _context;
    private readonly IEnvironment _environment;
    private readonly TokenStream _tokens;

    public Parser(Context context, IEnvironment environment, string code)
    {
        _context = context;
        _environment = environment;
        _tokens = new TokenStream(code);
    }

    /// <summary>
    /// Выполняет синтаксический разбор строк кода по правилу.
    /// program = statement, { ";", statement }, [ ";" ]
    /// </summary>
    public void ParseProgram()
    {
        do
        {
            RuntimeValue result = ParseStatement();

            if (_tokens.Peek().Type == TokenType.Semicolon)
            {
                Match(TokenType.Semicolon);
            }
            else if (_tokens.Peek().Type != TokenType.Semicolon &&
                     _tokens.Peek().Type != TokenType.EndOfFile)
            {
                throw new UnexpectedLexemeException(TokenType.Semicolon, _tokens.Peek());
            }

            _environment.AddResult(result);
        }
        while (_tokens.Peek().Type != TokenType.EndOfFile);
    }

    /// <summary>
    /// Разбирает 1 из statement.
    /// statement =
    ///      | variable_declaration
    ///      | const_defenition
    ///      | assignment
    ///      | if_statement
    ///      | write_statement
    ///      | read_statement
    ///      | while_statement
    ///      | compound_statement
    /// Реализовано:
    ///     statement = if_statement
    ///             | variable_declaration
    ///             | const_defenition
    ///             | write_statement
    ///             | read_statement
    ///             | assignment
    ///             | expression (временно)
    /// </summary>
    private RuntimeValue ParseStatement()
    {
        if (_tokens.Peek().Type == TokenType.Identifier &&
            _tokens.Peek(1).Type == TokenType.Assignment)
        {
            return ParseAssignment();
        }

        Token keyword = _tokens.Peek();
        switch (keyword.Type)
        {
            case TokenType.Const:
                return ParseConstantDefinition();
            case TokenType.Int:
                return ParseVariableDeclaration();
            case TokenType.If:
                return ParseIfStatement();
            case TokenType.Write:
                return ParseWriteStatement();
            case TokenType.Read:
                return ParseReadStatement();

            default:
                return ParseExpression();
        }
    }

    /// <summary>
    /// Разбирает readln.
    /// Правило:
    ///     read_statement = "read", "(", identifier, {"," ,identifier } ")" ;
    /// Реализовано:
    ///     read_statement = "read", "(", identifier, ")"
    /// </summary>
    private RuntimeValue ParseReadStatement()
    {
        // сделать Readln
        Match(TokenType.Read);

        Match(TokenType.OpenParenthesis);
        string name = Match(TokenType.Identifier).Value!.ToString();
        Match(TokenType.CloseParenthesis);

        // читаем значение из окружения (пока что только decimal)
        RuntimeValue value = _environment.Read();

        if (value.Type == RuntimeValue.ValueType.Number)
        {
            _context.AssignVariable(name, (decimal)value.Value);
        }

        return value;
    }

    /// <summary>
    /// Разбирает writeln.
    /// Правило:
    ///     write_statement = "write", "(" expression_list ")" ;
    /// Реализовано:
    ///     write_statement = "write", "(" expression ")"
    /// </summary>
    private RuntimeValue ParseWriteStatement()
    {
        Match(TokenType.Write);

        Match(TokenType.OpenParenthesis);

        RuntimeValue value = ParseExpression();

        Match(TokenType.CloseParenthesis);

        _environment.Write(value);

        return value;
    }

    /// <summary>
    /// Разбирает объявление константы.
    /// Правило:
    ///      constant_definition = "const", "int", identifier, "=", expression
    /// </summary>
    private RuntimeValue ParseConstantDefinition()
    {
        Match(TokenType.Const);
        Match(TokenType.Int);

        string name = Match(TokenType.Identifier).Value!.ToString();

        Match(TokenType.Assignment);

        RuntimeValue value = ParseExpression();

        // пока только decimal
        _context.DefineConstant(name, (decimal)value.Value);

        return value;
    }

    /// <summary>
    /// Разбирает присваивание существующей переменной
    /// Правило:
    ///     assignment = identifier, "=", expression
    /// </summary>
    private RuntimeValue ParseAssignment()
    {
        string name = Match(TokenType.Identifier).Value!.ToString();
        Match(TokenType.Assignment);

        RuntimeValue value = ParseExpression();
        _context.AssignVariable(name, (decimal)value.Value);

        return value;
    }

    /// <summary>
    /// Разбирает объявление переменных и следующее за ним выражение.
    /// Правило:
    ///     variable_declaration = "int", identifier, [ "=", expression ], { ",", identifier, [ "=", expression ] }
    ///     Возвращает результат последнего присваивания
    /// </summary>
    private RuntimeValue ParseVariableDeclaration()
    {
        Match(TokenType.Int);
        _context.PushScope(new Scope());

        List<RuntimeValue> results = new List<RuntimeValue>();

        // Первая переменная
        string firstName = Match(TokenType.Identifier).Value!.ToString();
        RuntimeValue firstValue = ParseOptionalAssignment();
        _context.DefineVariable(firstName, (decimal)firstValue.Value);

        results.Add(firstValue);

        // Остальные
        while (_tokens.Peek().Type == TokenType.Comma)
        {
            Match(TokenType.Comma);

            string name = Match(TokenType.Identifier).Value!.ToString();
            RuntimeValue value = ParseOptionalAssignment();
            _context.DefineVariable(name, (decimal)value.Value);

            results.Add(value);
        }

        // Возвращаем результат последнего присваивания
        return results.Last();
    }

    private RuntimeValue ParseOptionalAssignment()
    {
        if (_tokens.Peek().Type == TokenType.Assignment)
        {
            Match(TokenType.Assignment);
            return ParseExpression();
        }
        else
        {
            return RuntimeValue.Number(0);
        }
    }

    /// <summary>
    /// Разбирает составной statement в фигурных скобках
    /// compound_statement = "{", { statement, [ ";" ] }, "}"
    /// Возвращает результат ПОСЛЕДНЕГО statement в блоке
    /// </summary>
    private List<RuntimeValue> ParseCompoundStatement()
    {
        Match(TokenType.OpenBraces);

        List<RuntimeValue> result = new();

        while (_tokens.Peek().Type != TokenType.CloseBraces)
        {
            result.Add(ParseStatement());

            if (_tokens.Peek().Type == TokenType.EndOfFile)
            {
                throw new UnexpectedLexemeException(TokenType.CloseBraces, _tokens.Peek());
            }
        }

        Match(TokenType.CloseBraces);
        return result;
    }

    /// <summary>
    /// Разбирает аргументы команды If
    ///     if_statement = "if", "(", condition, ")", statement, [ "else", statement ]
    /// Реализовано:
    ///     "if", "(", condition, ")", statement
    /// </summary>
    private RuntimeValue ParseIfStatement()
    {
        Match(TokenType.If);

        Match(TokenType.OpenParenthesis);
        RuntimeValue conditionResult = ParseCondition();
        Match(TokenType.CloseParenthesis);

        // Выполняем then
        List<RuntimeValue> thenResult = ParseCompoundStatement();

        // else?
        if (_tokens.Peek().Type == TokenType.Else)
        {
            _tokens.Advance();
            List<RuntimeValue> elseResult = ParseCompoundStatement();
            return ConvertToBoolean(conditionResult)
               ? thenResult.Last()
               : elseResult.Last(); // возврат последнего
        }

        // если условие false и нет else и блок then пустой - возвращаем conditionResult
        return ConvertToBoolean(conditionResult)
            ? thenResult.Count > 0
                ? thenResult.Last()
                : conditionResult
            : conditionResult;
    }

    /// <summary>
    /// Разбирает условие
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
    /// Разбирает список выражений, разделённый запятыми.
    /// Правила:
    ///     expression_list = expression, { ",", expression } ;
    /// </summary>
    private List<RuntimeValue> ParseExpressionList()
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

        List<RuntimeValue> result = new(values.ToArray());
        return result;
    }

    /// <summary>
    /// Разбирает одно выражение.
    /// Правила:
    ///     expression = multiplicative_expression, { ("+" | "-"), multiplicative_expression }
    /// </summary>
    private RuntimeValue ParseExpression()
    {
        RuntimeValue value = ParseMultiplicativeExpression();
        while (true)
        {
            switch (_tokens.Peek().Type)
            {
                case TokenType.PlusSign:
                    _tokens.Advance();
                    value = EvaluateArithmetic(value, ParseMultiplicativeExpression(), TokenType.PlusSign);
                    break;
                case TokenType.MinusSign:
                    _tokens.Advance();
                    value = EvaluateArithmetic(value, ParseMultiplicativeExpression(), TokenType.MinusSign);
                    break;
                default:
                    return value;
            }
        }
    }

    /// <summary>
    ///  Разбирает один операнд сложения/вычитания.
    ///  Правила:
    ///     multiplicative_expression = unary_expression, { ("*" | "/" | "%"), unary_expression }
    /// </summary>
    private RuntimeValue ParseMultiplicativeExpression()
    {
        RuntimeValue value = ParseUnaryExpression();
        while (true)
        {
            switch (_tokens.Peek().Type)
            {
                case TokenType.MultiplySign:
                    _tokens.Advance();
                    value = EvaluateArithmetic(value, ParseMultiplicativeExpression(), TokenType.MultiplySign);
                    break;
                case TokenType.DivideSign:
                    _tokens.Advance();
                    value = EvaluateArithmetic(value, ParseMultiplicativeExpression(), TokenType.DivideSign);
                    break;
                case TokenType.ModuloSign:
                    _tokens.Advance();
                    value = EvaluateArithmetic(value, ParseMultiplicativeExpression(), TokenType.ModuloSign);
                    break;

                default:
                    return value;
            }
        }
    }

    /// <summary>
    ///  Разбирает один операнд умножения / деления.
    ///  Правило:
    ///     unary_expression = ("+" | "-"), unary_expression | exponentiation_expression
    private RuntimeValue ParseUnaryExpression()
    {
        if (_tokens.Peek().Type == TokenType.MinusSign)
        {
            _tokens.Advance();
            RuntimeValue operand = ParseUnaryExpression(); // Рекурсивно

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
            return ParseUnaryExpression(); // Рекурсивно (если + просто скипаем, они ничего не меняют)
        }
        else
        {
            return ParseExponentiationExpression();
        }
    }

    /// <summary>
    ///  Разбирает одну операцию возведения в степень.
    ///  Правило:
    ///     exponentiation_expression = primary_expression, { ("^"), exponentiation_expression }
    ///  Реализовано:
    ///     exponentiation_expression = primary_expression
    /// </summary>
    private RuntimeValue ParseExponentiationExpression()
    {
        RuntimeValue value = ParsePrimaryExpression();

        return value;
    }

    /// <summary>
    ///  Разбирает простейшую часть выражения.
    ///     primary_expression = number | string | identifier | function_call | "(", expression, ")" | const_expression(MathE, Pi)
    private RuntimeValue ParsePrimaryExpression()
    {
        Token t = _tokens.Peek();

        if (t.Type == TokenType.OpenParenthesis)
        {
            Match(TokenType.OpenParenthesis);
            RuntimeValue expression = ParseExpression();
            Match(TokenType.CloseParenthesis);
            return expression;
        }

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

        if (t.Type == TokenType.True)
        {
            _tokens.Advance();
            return RuntimeValue.Boolean(true);
        }

        if (t.Type == TokenType.False)
        {
            _tokens.Advance();
            return RuntimeValue.Boolean(false);
        }

        // Пока что встроенные константы и встроенные функции
        if (t.Type == TokenType.Identifier)
        {
            string identifier = t.Value!.ToString();

            switch (identifier)
            {
                case "MathE":
                    _tokens.Advance();
                    return RuntimeValue.Number((decimal)Math.E);
                case "Pi":
                    _tokens.Advance();
                    return RuntimeValue.Number((decimal)Math.PI);
                default:
                    // func_call (встроенные функции)
                    if (_tokens.Peek(1).Type == TokenType.OpenParenthesis)
                    {
                        _tokens.Advance();
                        Match(TokenType.OpenParenthesis);
                        List<RuntimeValue> arguments = ParseExpressionList();
                        Match(TokenType.CloseParenthesis);
                        return RuntimeValue.Number(BuiltinFunctions.Invoke(t.Value!.ToString(), ConvertToDecimalList(arguments)));
                    }

                    // получить существующую перменную
                    else
                    {
                        string name = Match(TokenType.Identifier).Value!.ToString();
                        RuntimeValue variable = RuntimeValue.Number(_context.GetValue(name));
                        return variable;
                    }
            }
        }

        throw new UnexpectedLexemeException(TokenType.Identifier, t);
    }

    private List<decimal> ConvertToDecimalList(List<RuntimeValue> runtimeValues)
    {
        List<decimal> decimals = new List<decimal>();

        for (int i = 0; i < runtimeValues.Count; i++)
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
    private Token Match(TokenType expected)
    {
        Token t = _tokens.Peek();
        if (t.Type != expected)
        {
            throw new UnexpectedLexemeException(expected, t);
        }

        _tokens.Advance();

        return t;
    }

    /// <summary>
    /// Вычисляет арифметическую бинарную операцию.
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
}