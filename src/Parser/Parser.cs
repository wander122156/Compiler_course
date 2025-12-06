using System.Collections.Generic;
using System.Data;
using System.Xml.Linq;

using Blang.Ast;
using Blang.Ast.Declarations;
using Blang.Ast.Expressions;
using Blang.Ast.Statement;
using Blang.Common;
using Blang.Execution;
using Blang.Lexer;

using static Blang.Ast.Expressions.BinaryOperationExpression;

using Expression = Blang.Ast.Expressions.Expression;

namespace Blang.Parser;

/// <summary>
/// Выполняет синтаксический разбор строк кода.
/// </summary>
public class Parser
{
    private readonly TokenStream _tokens;
    private readonly AstEvaluator _evaluator;
    private readonly Stack<ParserContext> _parserContext = new();

    public Parser(Context context, IEnvironment environment, string code)
    {
        _tokens = new TokenStream(code);
        _evaluator = new AstEvaluator(context, environment);
        _parserContext.Push(ParserContext.Global);
    }

    private enum ParserContext
    {
        Global,
        Function,
        Loop,
        Switch,
    }

    /// <summary>
    /// Выполняет синтаксический разбор строк кода по правилу.
    /// program = statement, { ";", statement }, [ ";" ]
    /// </summary>
    public void ParseProgram()
    {
        do
        {
             // нужно переделать чтобы возвращал IAstElement[]
            IAstElement node = ParseStatement();
            _evaluator.Evaluate(node);

            if (_tokens.Peek().Type == TokenType.Semicolon)
            {
                Match(TokenType.Semicolon);
            }
            else if (_tokens.Peek().Type != TokenType.Semicolon &&
                     _tokens.Peek().Type != TokenType.EndOfFile)
            {
                throw new UnexpectedLexemeException(TokenType.Semicolon, _tokens.Peek());
            }
        }
        while (_tokens.Peek().Type != TokenType.EndOfFile);
    }

    /// <summary>
    /// Разбирает 1 из statement.
    /// statement = variable_declaration
    ///       | const_defenition
    ///       | assignment
    ///       | if_statement
    ///       | function_declaration
    ///       | write_statement
    ///       | writeln_statement
    ///       | read_statement
    ///       | readln_statement
    ///       | while_statement
    ///       | do_while_statement
    ///       | for_statement
    ///       | compound_statement
    ///       | return__statement
    ///       | break_statement
    ///       | continue_statement
    ///
    /// Осталось реализовать :
    /// statement =
    /// </summary>
    private IAstElement ParseStatement()
    {
        if (_tokens.Peek().Type == TokenType.Identifier &&
            _tokens.Peek(1).Type == TokenType.Assignment)
        {
            return ParseAssignment();
        }

        // для вызова void функций (не в expression)
        if (_tokens.Peek().Type == TokenType.Identifier &&
        _tokens.Peek(1).Type == TokenType.OpenParenthesis)
        {
            return ParseFunctionCallExpression();
        }

        Token keyword = _tokens.Peek();
        switch (keyword.Type)
        {
            case TokenType.Const:
                return ParseConstantDefinition();
            case TokenType.Num:
                return ParseVariableDeclaration();
            case TokenType.If:
                return ParseIfStatement();
            case TokenType.For:
                return ParseForLoopStatement();
            case TokenType.While:
                return ParseWhileLoopStatement();
            case TokenType.Do:
                return ParseDoWhileLoopStatement();
            case TokenType.Func:
                return ParseFunctionDeclaration();
            case TokenType.Write:
                return ParseWriteStatement();
            case TokenType.Writeln:
                return ParseWriteLineStatement();
            case TokenType.Read:
                return ParseReadStatement();
            case TokenType.Readln:
                return ParseReadLineStatement();
            case TokenType.Return:
                return ParseReturnStatement();
            case TokenType.Break:
                return ParseBreakStatement();
            case TokenType.Continue:
                return ParseContinueStatement();

            default:
                throw new UnexpectedLexemeException(keyword.Type, keyword); // как сделать?
        }
    }

    /// <summary>
    /// Разбирает read.
    /// Правило:
    ///     read_statement = "read", "(", identifier, {"," ,identifier } ")"
    /// </summary>
    private ReadStatement ParseReadStatement()
    {
        Match(TokenType.Read);
        Match(TokenType.OpenParenthesis);
        List<string> variableNames = new();

        variableNames.Add(Match(TokenType.Identifier).Value!.ToString());

        while(_tokens.Peek().Type == TokenType.Comma)
        {
            _tokens.Advance();
            variableNames.Add(Match(TokenType.Identifier).Value!.ToString());
        }

        Match(TokenType.CloseParenthesis);
        return new ReadStatement(variableNames);
    }

    /// <summary>
    /// Разбирает readln.
    /// Правило:
    ///     readln_statement = "readln", "(", identifier, {"," ,identifier } ")"
    /// </summary>
    private ReadLineStatement ParseReadLineStatement()
    {
        Match(TokenType.Readln);
        Match(TokenType.OpenParenthesis);

        List<string> variableNames = new();

        variableNames.Add(Match(TokenType.Identifier).Value!.ToString());

        while (_tokens.Peek().Type == TokenType.Comma)
        {
            _tokens.Advance();
            variableNames.Add(Match(TokenType.Identifier).Value!.ToString());
        }

        Match(TokenType.CloseParenthesis);
        return new ReadLineStatement(variableNames);
    }

    /// <summary>
    /// Разбирает write.
    /// Правило:
    ///     write_statement = "write", "(" expression_list ")" ;
    /// </summary>
    private WriteStatement ParseWriteStatement()
    {
        Match(TokenType.Write);
        Match(TokenType.OpenParenthesis);

        List<Expression> expressions = ParseExpressionList();
        Match(TokenType.CloseParenthesis);

        return new WriteStatement(expressions);
    }

    /// <summary>
    /// Разбирает writeln.
    /// Правило:
    ///     writeln_statement = "writeln", "(" expression_list ")"
    ///                       | "writeln", "(", ")"
    /// </summary>
    private WriteLineStatement ParseWriteLineStatement()
    {
        Match(TokenType.Writeln);
        Match(TokenType.OpenParenthesis);

        if (_tokens.Peek().Type == TokenType.CloseParenthesis)
        {
            Match(TokenType.CloseParenthesis);
            return new WriteLineStatement(new List<Expression>()); // writeln()
        }

        List<Expression> expressions = ParseExpressionList();
        Match(TokenType.CloseParenthesis);

        return new WriteLineStatement(expressions);
    }

    /// <summary>
    /// Разбирает объявление константы.
    /// Правило:
    ///      constant_definition = "const", "num", identifier, "=", expression
    /// </summary>
    private ConstantDeclaration ParseConstantDefinition()
    {
        Match(TokenType.Const);
        Match(TokenType.Num);

        string name = Match(TokenType.Identifier).Value!.ToString();

        Match(TokenType.Assignment);

        Expression value = ParseExpression();

        // пока только decimal
        return new ConstantDeclaration(name, value);
    }

    /// <summary>
    /// Разбирает присваивание существующей переменной
    /// Правило:
    ///     assignment = identifier, "=", expression
    /// </summary>
    private AssignmentExpression ParseAssignment()
    {
        string name = Match(TokenType.Identifier).Value!.ToString();
        Match(TokenType.Assignment);

        Expression value = ParseExpression();
        return new AssignmentExpression(name, value);
    }

    /// <summary>
    /// Разбирает объявление переменных и следующее за ним выражение.
    /// Правило:
    ///     variable_declaration = "num", identifier, [ "=", expression ], { ",", identifier, [ "=", expression ] }
    /// </summary>
    private VariableDeclarationStatement ParseVariableDeclaration()
    {
        Match(TokenType.Num);

        List<VariableDeclaration> declarations = new();

        // Первая переменная
        string firstName = Match(TokenType.Identifier).Value!.ToString();
        Expression? firstValue = ParseOptionalAssignment();
        declarations.Add(new VariableDeclaration(firstName, firstValue));

        // Остальные переменные
        while (_tokens.Peek().Type == TokenType.Comma)
        {
            Match(TokenType.Comma);
            string name = Match(TokenType.Identifier).Value!.ToString();
            Expression? value = ParseOptionalAssignment();
            declarations.Add(new VariableDeclaration(name, value));
        }

        return new VariableDeclarationStatement(declarations);
    }

    private Expression? ParseOptionalAssignment()
    {
        if (_tokens.Peek().Type == TokenType.Assignment)
        {
            Match(TokenType.Assignment);
            return ParseExpression();
        }
        else
        {
            return null; // Нет инициализации
        }
    }

    /// <summary>
    /// Разбирает объявление переменных и следующее за ним выражение.
    /// Правило:
    ///     function_declaration = "func", "num", identifier, "(", [ parameter_list ], ")", compound_statement ;
    ///     parameter_list = "num", identifier, { ",", "num", identifier } ;
    /// </summary>
    private FunctionDeclaration ParseFunctionDeclaration()
    {
        _tokens.Advance();
        Match(TokenType.Num);
        string funcName = Match(TokenType.Identifier).Value!.ToString();

        Match(TokenType.OpenParenthesis);

        List<(string name, string type)> parameters = new();

        while (_tokens.Peek().Type != TokenType.CloseParenthesis)
        {
            // TODO: сильно переделать типы
            Match(TokenType.Num);
            string paramType = "num";

            string paramName = Match(TokenType.Identifier).Value!.ToString();
            parameters.Add((paramName, paramType));

            if (_tokens.Peek().Type == TokenType.Comma)
            {
                _tokens.Advance();
            }
            else
            {
                break;
            }
        }

        Match(TokenType.CloseParenthesis);

        _parserContext.Push(ParserContext.Function);
        try
        {
            CompoundStatement body = ParseCompoundStatement();
            return new FunctionDeclaration(funcName, parameters, body);
        }
        finally
        {
            _parserContext.Pop();
        }
    }

    /// <summary>
    /// Разбирает return.
    /// Правило:
    ///     return_statement = "return", expression ;
    /// </summary>
    private ReturnStatement ParseReturnStatement()
    {
        if(_parserContext.Peek() != ParserContext.Function)
        {
            throw new SyntaxErrorException("'return' can only be used inside functions");
        }

        _tokens.Advance();

        Expression result = ParseExpression();

        return new ReturnStatement(result);
    }

    /// <summary>
    /// Разбирает break.
    /// Правило:
    ///     break_statement = "break" ;
    /// </summary>
    private BreakStatement ParseBreakStatement()
    {
        if (_parserContext.Peek() != ParserContext.Loop)
        {
            throw new SyntaxErrorException("'break' can only be used inside loops");
        }

        _tokens.Advance();
        return new BreakStatement();
    }

    /// <summary>
    /// Разбирает continue.
    /// Правило:
    ///     continue_statement = "continue" ;
    /// </summary>
    private ContinueStatement ParseContinueStatement()
    {
        if (_parserContext.Peek() != ParserContext.Loop)
        {
            throw new SyntaxErrorException("'continue' can only be used inside loops");
        }

        _tokens.Advance();
        return new ContinueStatement();
    }

    /// <summary>
    /// Разбирает for цикл
    /// Правило:
    ///     for_statement = "for", "(", for_initialization, ";", for_condition, ";", for_increment, ")", compound_statement
    ///     for_initialization = variable_declaration | assignment
    ///     for_condition = expression
    ///     for_increment = assignment
    /// </summary>
    private ForLoopStatement ParseForLoopStatement()
    {
        _tokens.Advance();
        Match(TokenType.OpenParenthesis);

        IAstElement initialization;

        if (_tokens.Peek().Type == TokenType.Num)
        {
            initialization = ParseVariableDeclaration();
        }
        else if (_tokens.Peek().Type == TokenType.Identifier &&
                 _tokens.Peek(1).Type == TokenType.Assignment)
        {
            initialization = ParseAssignment();
        }
        else
        {
            throw new UnexpectedLexemeException(TokenType.Num, _tokens.Peek());
        }

        Match(TokenType.Semicolon);

        Expression condition = ParseCondition();
        Match(TokenType.Semicolon);

        AssignmentExpression increment = ParseAssignment();

        Match(TokenType.CloseParenthesis);

        _parserContext.Push(ParserContext.Loop);
        try
        {
            CompoundStatement body = ParseCompoundStatement();
            return new ForLoopStatement(initialization, condition, increment, body);
        }
        finally
        {
            _parserContext.Pop();
        }
    }

    /// <summary>
    /// Разбирает while цикл:
    ///     while_statement = "while", "(", condition, ")", compound_statement ;
    /// </summary>
    private WhileLoopStatement ParseWhileLoopStatement()
    {
        _tokens.Advance();

        Match(TokenType.OpenParenthesis);
        Expression condition = ParseCondition();
        Match(TokenType.CloseParenthesis);

        _parserContext.Push(ParserContext.Loop);
        try
        {
            CompoundStatement body = ParseCompoundStatement();
            return new WhileLoopStatement(condition, body);
        }
        finally
        {
            _parserContext.Pop();
        }
    }

    /// <summary>
    /// Разбирает do-while цикл:
    ///     do_while_statement = "do", compound_statement, "while", "(", condition, ")" ;
    /// </summary>
    private DoWhileLoopStatement ParseDoWhileLoopStatement()
    {
        _tokens.Advance();

        _parserContext.Push(ParserContext.Loop);
        try
        {
            CompoundStatement body = ParseCompoundStatement();
            Match(TokenType.While);

            Match(TokenType.OpenParenthesis);
            Expression condition = ParseCondition();
            Match(TokenType.CloseParenthesis);

            return new DoWhileLoopStatement(condition, body);
        }
        finally
        {
            _parserContext.Pop();
        }
    }

    /// <summary>
    /// Разбирает if statement:
    ///     if_statement = "if", "(", condition, ")", statement_or_block, [ "else", statement_or_block ]
    ///     statement_or_block = compound_statement | statement
    ///     compound_statement = "{", { statement, [ ";" ] }, "}"
    /// </summary>
    private IfElseStatement ParseIfStatement()
    {
        _tokens.Advance();

        Match(TokenType.OpenParenthesis);
        Expression condition = ParseCondition();
        Match(TokenType.CloseParenthesis);

        // Выполняем then
        IAstElement thenBranch = ParseStatementOrBlock();

        // else?
        IAstElement? elseBranch = null;
        if (_tokens.Peek().Type == TokenType.Else)
        {
            _tokens.Advance();
            elseBranch = ParseStatementOrBlock();
        }

        return new IfElseStatement(condition, thenBranch, elseBranch);
    }

    /// <summary>
    /// Парсит statement или блок в фигурных скобках
    /// </summary>
    private IAstElement ParseStatementOrBlock()
    {
        // Если следующая лексема - открывающая фигурная скобка, парсим compound statement
        if (_tokens.Peek().Type == TokenType.OpenBraces)
        {
            return ParseCompoundStatement();
        }
        else
        {
            return ParseStatement();
        }
    }

    /// <summary>
    /// Разбирает составной statement в фигурных скобках
    /// compound_statement = "{", statement, { ";", statement }, [ ";" ], "}"
    /// Возвращает результат ПОСЛЕДНЕГО statement в блоке
    /// </summary>
    private CompoundStatement ParseCompoundStatement()
    {
        Match(TokenType.OpenBraces);

        List<IAstElement> statements = new();

        while (_tokens.Peek().Type != TokenType.CloseBraces)
        {
            statements.Add(ParseStatement());
            if (_tokens.Peek().Type != TokenType.CloseBraces)
            {
                Match(TokenType.Semicolon);
            }

            if (_tokens.Peek().Type == TokenType.EndOfFile)
            {
                throw new UnexpectedLexemeException(TokenType.CloseBraces, _tokens.Peek());
            }
        }

        Match(TokenType.CloseBraces);
        return new CompoundStatement(statements);
    }

    /// <summary>
    /// Разбирает условие
    ///     condition = expression, [ comparison_operator, expression ]
    /// </summary>
    private Expression ParseCondition()
    {
        Expression left = ParseExpression();

        Token operationToken = _tokens.Peek();
        if (IsComparisonOperator(operationToken.Type))
        {
            _tokens.Advance();
            Expression right = ParseExpression();

            return CreateComparisonExpression(left, operationToken.Type, right);
        }

        return left;
    }

    private Expression CreateComparisonExpression(Expression left, TokenType operation, Expression right)
    {
        BinaryOperation binaryOp = operation switch
        {
            TokenType.LessThan => BinaryOperation.LessThan,
            TokenType.GreaterThan => BinaryOperation.GreaterThan,
            TokenType.LooseEquality => BinaryOperation.LooseEquality,
            TokenType.NotEqual => BinaryOperation.NotEqual,
            TokenType.LessThanOrEqual => BinaryOperation.LessThanOrEqual,
            TokenType.GreaterThanOrEqual => BinaryOperation.GreaterThanOrEqual,
            _ => throw new Exception($"Unsupported comparison operator: {operation}")
        };

        return new BinaryOperationExpression(left, binaryOp, right);
    }

    /// <summary>
    /// Разбирает список выражений, разделённый запятыми.
    /// Правила:
    ///     expression_list = expression, { ",", expression } ;
    /// </summary>
    private List<Expression> ParseExpressionList()
    {
        List<Expression> expressions =
        [
            ParseExpression(),
        ];
        while (_tokens.Peek().Type == TokenType.Comma)
        {
            _tokens.Advance();
            expressions.Add(ParseExpression());
        }

        return expressions;
    }

    /// <summary>
    /// Разбирает одно выражение.
    /// Правила:
    ///     expression = multiplicative_expression, { ("+" | "-"), multiplicative_expression }
    /// </summary>
    private Expression ParseExpression()
    {
        Expression left = ParseMultiplicativeExpression();
        while (true)
        {
            switch (_tokens.Peek().Type)
            {
                case TokenType.PlusSign:
                    _tokens.Advance();
                    left = new BinaryOperationExpression(
                        left,
                        BinaryOperation.Plus,
                        ParseMultiplicativeExpression()
                    );
                    break;
                case TokenType.MinusSign:
                    _tokens.Advance();
                    left = new BinaryOperationExpression(
                        left,
                        BinaryOperation.Minus,
                        ParseMultiplicativeExpression()
                    );
                    break;
                default:
                    return left;
            }
        }
    }

    /// <summary>
    ///  Разбирает один операнд сложения/вычитания.
    ///  Правила:
    ///     multiplicative_expression = unary_expression, { ("*" | "/" | "%"), unary_expression }
    /// </summary>
    private Expression ParseMultiplicativeExpression()
    {
        Expression left = ParseUnaryExpression();
        while (true)
        {
            switch (_tokens.Peek().Type)
            {
                case TokenType.MultiplySign:
                    _tokens.Advance();
                    left = new BinaryOperationExpression(
                        left,
                        BinaryOperation.Multiply,
                        ParseUnaryExpression()
                    );
                    break;
                case TokenType.DivideSign:
                    _tokens.Advance();
                    left = new BinaryOperationExpression(
                        left,
                        BinaryOperation.Divide,
                        ParseUnaryExpression()
                    );
                    break;
                case TokenType.ModuloSign:
                    _tokens.Advance();
                    left = new BinaryOperationExpression(
                        left,
                        BinaryOperation.Modulo,
                        ParseUnaryExpression()
                    );
                    break;
                default:
                    return left;
            }
        }
    }

    /// <summary>
    ///  Разбирает один операнд умножения / деления.
    ///  Правило:
    ///     unary_expression = ("+" | "-"), unary_expression | exponentiation_expression
    private Expression ParseUnaryExpression()
    {
        if (_tokens.Peek().Type == TokenType.MinusSign)
        {
            _tokens.Advance();
            Expression operand = ParseUnaryExpression(); // Рекурсивно

            return new UnaryOperationExpression(UnaryOperation.Minus, operand);
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
    /// </summary>
    private Expression ParseExponentiationExpression()
    {
        Expression left = ParsePrimaryExpression();

        while (_tokens.Peek().Type == TokenType.ExponentiationSign)
        {
            _tokens.Advance();
            Expression right = ParseExponentiationExpression(); // правая ассициотивность
            left = new BinaryOperationExpression(
                left,
                BinaryOperation.Exponentiation,
                right
            );
        }

        return left;
    }

    /// <summary>
    ///  Разбирает простейшую часть выражения.
    ///     primary_expression = number | string | identifier | function_call | "(", expression, ")" | const_expression(MathE, Pi)
    private Expression ParsePrimaryExpression()
    {
        Token t = _tokens.Peek();

        if (t.Type == TokenType.OpenParenthesis)
        {
            Match(TokenType.OpenParenthesis);
            Expression expression = ParseExpression();
            Match(TokenType.CloseParenthesis);
            return expression;
        }

        if (t.Type == TokenType.StringLiteral)
        {
            _tokens.Advance();
            return new StringLiteralExpression(t.Value!.ToString());
        }

        if (t.Type == TokenType.NumericLiteral)
        {
            _tokens.Advance();
            return new NumericLiteralExpression(t.Value!.ToDecimal());
        }

        if (t.Type == TokenType.True)
        {
            _tokens.Advance();
            return new BooleanLiteralExpression(true);
        }

        if (t.Type == TokenType.False)
        {
            _tokens.Advance();
            return new BooleanLiteralExpression(false);
        }

        if (t.Type == TokenType.Identifier)
        {
            string identifier = Match(TokenType.Identifier).Value!.ToString();

            switch (identifier)
            {
                case "MathE":
                    return new NumericLiteralExpression((decimal)Math.E);
                case "Pi":
                    return new NumericLiteralExpression((decimal)Math.PI);
                default:
                    // func_call
                    if (_tokens.Peek().Type == TokenType.OpenParenthesis)
                    {
                        List<Expression> arguments = ParseArgumentsList();
                        return new FunctionCallExpression(identifier, arguments);
                    }

                    // получить существующую переменную
                    else
                    {
                        return new VariableExpression(identifier);
                    }
            }
        }

        throw new UnexpectedLexemeException(TokenType.Identifier, t);
    }

    private FunctionCallExpression ParseFunctionCallExpression()
    {
        string functionName = Match(TokenType.Identifier).Value!.ToString();
        List<Expression> arguments = ParseArgumentsList();
        return new FunctionCallExpression(functionName, arguments);
    }

    private List<Expression> ParseArgumentsList()
    {
        Match(TokenType.OpenParenthesis);
        List<Expression> arguments = new();

        if (_tokens.Peek().Type != TokenType.CloseParenthesis)
        {
            arguments.Add(ParseExpression());
            while (_tokens.Peek().Type == TokenType.Comma)
            {
                Match(TokenType.Comma);
                arguments.Add(ParseExpression());
            }
        }

        Match(TokenType.CloseParenthesis);
        return arguments;
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
}