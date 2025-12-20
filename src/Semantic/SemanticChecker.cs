using Blang.Ast;
using Blang.Ast.Declarations;
using Blang.Ast.Expressions;
using Blang.Ast.Statement;

using ValueType = Blang.Common.RuntimeValue.ValueType;

namespace Blang.Semantic;

/// <summary>
/// Семантический анализатор, проверяющий типы и контекстные правила.
/// Использует отдельный TypeContext, не связанный с выполнением.
/// </summary>
public class SemanticChecker : IAstVisitor
{
    private readonly TypeContext _typeContext;
    private readonly Stack<ValueType> _types = new();
    private readonly Stack<FunctionContext> _functionStack = new();

    public SemanticChecker()
    {
        _typeContext = new TypeContext();
    }

    public void Check(List<IAstElement> root)
    {
        // Объявляем все функции (только сигнатуры)
        foreach (IAstElement node in root)
        {
            if (node is FunctionDeclaration funcDecl)
            {
                DeclareFunction(funcDecl);
            }
        }

        foreach (IAstElement node in root)
        {
            try
            {
                node.Accept(this);
            }
            catch (TypeException)
            {
                throw;
            }
        }
    }

    public void Visit(NumericLiteralExpression e) => _types.Push(ValueType.Number);

    public void Visit(StringLiteralExpression e) => _types.Push(ValueType.String);

    public void Visit(BooleanLiteralExpression e) => _types.Push(ValueType.Boolean);

    public void Visit(ConstantDeclaration d)
    {
        d.Value.Accept(this);
        ValueType valueType = _types.Pop();

        _typeContext.DefineVariableType(d.Name, valueType);
    }

    public void Visit(VariableExpression e)
    {
        ValueType type = _typeContext.GetVariableType(e.Name);
        _types.Push(type);
    }

    public void Visit(VariableDeclaration d)
    {
        ValueType declaredType = ParseStringToType(d.DeclaredTypeName); // "num", "string", "bool"
        ValueType actualType = ValueType.Undefined;

        if (d.Value != null)
        {
            d.Value.Accept(this);
            actualType = _types.Pop(); // declaredType identifier = actualType
            if (actualType != declaredType)
            {
                throw new TypeException($"Cannot assign value of type {actualType} to variable '{d.Name}' of type {declaredType}");
            }
        }

        _typeContext.DefineVariableType(d.Name, declaredType);
    }

    public void Visit(VariableDeclarationStatement s)
    {
        _typeContext.PushScope();

        foreach (VariableDeclaration declaration in s.Declarations)
        {
            ValueType declaredType = ParseStringToType(declaration.DeclaredTypeName);
            ValueType actualType = ValueType.Undefined;

            if (declaration.Value != null)
            {
                declaration.Value.Accept(this);
                actualType = _types.Pop();

                if (actualType != declaredType)
                {
                    throw new TypeException(
                        $"Cannot assign value of type {actualType} to variable '{declaration.Name}' of type {declaration.DeclaredTypeName}"
                    );
                }
            }

            _typeContext.DefineVariableType(declaration.Name, declaredType);
        }
    }

    public void Visit(AssignmentExpression e)
    {
        e.Value.Accept(this);
        ValueType valueType = _types.Pop();

        ValueType variableType = _typeContext.GetVariableType(e.Name);
        if (valueType != variableType)
        {
            throw new TypeException(
                $"Cannot assign {valueType} to variable '{e.Name}' of type {variableType}"
            );
        }

        _types.Push(variableType);
    }

    public void Visit(BinaryOperationExpression e)
    {
        e.Left.Accept(this);
        ValueType leftType = _types.Pop();

        e.Right.Accept(this);
        ValueType rightType = _types.Pop();

        ValueType resultType = CheckBinaryOperation(e.Operation, leftType, rightType);
        _types.Push(resultType);
    }

    public void Visit(UnaryOperationExpression e)
    {
        e.Operand.Accept(this);
        ValueType operandType = _types.Pop();

        switch (e.Operation)
        {
            case UnaryOperation.Minus:
            case UnaryOperation.Plus:
                if (operandType != ValueType.Number)
                {
                    throw new TypeException(
                        $"Unary operator + or - requires number, got {operandType}"
                    );
                }

                _types.Push(ValueType.Number);
                break;
            case UnaryOperation.Not:
                if (operandType != ValueType.Number &&
                    operandType != ValueType.String &&
                    operandType != ValueType.Boolean )
                {
                    throw new TypeException(
                        $"Unary operator !(not) requires number, boolean or string got {operandType}"
                    );
                }

                _types.Push(ValueType.Boolean);
                break;

            default:
                throw new TypeException($"Unknown unary operation: {e.Operation}");
        }
    }

    public void Visit(IfElseStatement s)
    {
        s.Condition.Accept(this);
        ValueType conditionType = _types.Pop();

        if (conditionType != ValueType.Boolean &&
            conditionType != ValueType.Number &&
            conditionType != ValueType.String)
        {
            throw new TypeException(
                $"If condition must be boolean or number, got {conditionType}"
            );
        }

        s.ThenBranch.Accept(this);
        s.ElseBranch?.Accept(this);
    }

    public void Visit(ForLoopStatement s)
    {
        _typeContext.PushScope();

        s.Initialization.Accept(this);

        s.Condition.Accept(this);
        ValueType conditionType = _types.Pop();

        if (conditionType != ValueType.Boolean &&
            conditionType != ValueType.Number)
        {
            throw new TypeException(
                $"Loop condition must be boolean or number, got {conditionType}"
            );
        }

        s.Body.Accept(this);
        s.Increment.Accept(this);

        _typeContext.PopScope();
    }

    public void Visit(WhileLoopStatement s)
    {
        _typeContext.PushScope();

        s.Condition.Accept(this);
        ValueType conditionType = _types.Pop();

        if (conditionType != ValueType.Boolean &&
            conditionType != ValueType.Number)
        {
            throw new TypeException(
                $"Loop condition must be boolean or number, got {conditionType}"
            );
        }

        s.Body.Accept(this);

        _typeContext.PopScope();
    }

    public void Visit(DoWhileLoopStatement s)
    {
        _typeContext.PushScope();
        s.Body.Accept(this);
        _typeContext.PopScope();

        s.Condition.Accept(this);
        ValueType conditionType = _types.Pop();

        if (conditionType != ValueType.Boolean &&
            conditionType != ValueType.Number)
        {
            throw new TypeException(
                $"Loop condition must be boolean or number, got {conditionType}"
            );
        }
    }

    public void Visit(BreakStatement s)
    {
    }

    public void Visit(ContinueStatement s)
    {
    }

    public void Visit(FunctionDeclaration d)
    {
        _functionStack.Push(new FunctionContext(
            d.FuncName,
            ParseStringToType(d.ReturnType)
        ));

        // Создаем область видимости для параметров
        _typeContext.PushScope();

        foreach ((string name, string type) param in d.Parameters)
        {
            ValueType paramType = ParseStringToType(param.type);
            _typeContext.DefineVariableType(param.name, paramType);
        }

        d.Body.Accept(this);

        FunctionContext currentFunc = _functionStack.Peek();

        // Проверяем, что не-void функция имеет return
        if (currentFunc.ReturnType != ValueType.Void && !currentFunc.HasReturn)
        {
            throw new TypeException(
                $"Function '{currentFunc.Name}' must return a value of type {TypeToString(currentFunc.ReturnType)}"
            );
        }

        _typeContext.PopScope();

        _functionStack.Pop();
    }

    public void Visit(ReturnStatement s)
    {
        if (_functionStack.Count == 0)
        {
            throw new TypeException("'return' statement outside of function");
        }

        FunctionContext currentFunc = _functionStack.Peek();
        currentFunc.HasReturn = true;

        if (currentFunc.ReturnType == ValueType.Void)
        {
            // Void функция не должна возвращать значение
            if (s.ReturnValue != null)
            {
                throw new TypeException($"Void function '{currentFunc.Name}' cannot return a value");
            }
        }
        else
        {
            // Не-void функция должна возвращать значение
            if (s.ReturnValue == null)
            {
                throw new TypeException($"Function '{currentFunc.Name}' must return a value");
            }

            s.ReturnValue.Accept(this);
            ValueType returnType = _types.Pop();

            if (currentFunc.ReturnType != returnType)
            {
                throw new TypeException(
                    $"Function '{currentFunc.Name}' returns {TypeToString(returnType)}, expected {TypeToString(currentFunc.ReturnType)}"
                );
            }
        }
    }

    public void Visit(FunctionCallExpression e)
    {
        if (BuiltinTypeChecker.IsBuiltin(e.FunctionName))
        {
            CheckBuiltinFunctionCall(e);
            return;
        }

        if (!_typeContext.HasFunction(e.FunctionName))
        {
            throw new TypeException($"Function '{e.FunctionName}' is not defined");
        }

        TypeContext.FunctionInfo funcInfo = _typeContext.GetFunctionInfo(e.FunctionName);
        CheckFunctionCall(e, funcInfo);
    }

    public void Visit(WriteStatement s)
    {
        foreach (Expression expr in s.Expressions)
        {
            expr.Accept(this);
            _types.Pop();
        }
    }

    public void Visit(WriteLineStatement s)
    {
        foreach (Expression expr in s.Expressions)
        {
            expr.Accept(this);
            _types.Pop();
        }
    }

    public void Visit(ReadStatement s)
    {
        // Read считывает в существующие переменные
        foreach (string varName in s.VariableNames)
        {
            ValueType type = _typeContext.GetVariableType(varName);

            // Read может считывать только в строки или числа
            if (type != ValueType.String && type != ValueType.Number)
            {
                throw new TypeException(
                    $"Cannot read into variable '{varName}' of type {TypeToString(type)}"
                );
            }
        }
    }

    public void Visit(ReadLineStatement s)
    {
        // Readln считывает в существующие переменные
        foreach (string varName in s.VariableNames)
        {
            ValueType type = _typeContext.GetVariableType(varName);

            // Readln может считывать только в строки или числа
            if (type != ValueType.String && type != ValueType.Number)
            {
                throw new TypeException(
                    $"Cannot readln into variable '{varName}' of type {TypeToString(type)}"
                );
            }
        }
    }

    public void Visit(CompoundStatement s)
    {
        _typeContext.PushScope();

        foreach (IAstElement stmt in s.Statements)
        {
            stmt.Accept(this);
        }

        _typeContext.PopScope();
    }

    private void CheckBuiltinFunctionCall(FunctionCallExpression e)
    {
        BuiltinTypeChecker.FunctionTypeInfo typeInfo = BuiltinTypeChecker.GetTypeInfo(e.FunctionName);

        if (!BuiltinTypeChecker.CheckArgumentCount(e.FunctionName, e.Arguments.Count))
        {
            string expected;
            if (typeInfo.minArguments > 0)
                expected = $"at least {typeInfo.minArguments}";
            else
                expected = $"{typeInfo.parameterTypes.Count}";

            throw new TypeException(
                    $"Builtin function '{e.FunctionName}' expects {expected} arguments, got {e.Arguments.Count}"
                );
        }

        for (int i = 0; i < e.Arguments.Count; i++)
        {
            e.Arguments[i].Accept(this);
            ValueType argType = _types.Pop();

            ValueType expectedType;

            if (typeInfo.minArguments > 0 && i >= typeInfo.parameterTypes.Count)
            {
                // Если передано больше аргументов чем в parameterTypes,
                // используем тип последнего параметра
                expectedType = typeInfo.parameterTypes[^1];
            }
            else
            {
                expectedType = typeInfo.parameterTypes[i];
            }

            if (argType != expectedType)
            {
                throw new TypeException(
                    $"Argument {i + 1} of '{e.FunctionName}' expects {TypeToString(expectedType)}, got {TypeToString(argType)}"
                );
            }
        }

        // Тип return
        _types.Push(typeInfo.returnType);
    }

    private void CheckFunctionCall(FunctionCallExpression e, TypeContext.FunctionInfo funcInfo)
    {
        // 1. Проверяем количество аргументов
        if (e.Arguments.Count != funcInfo.parameters.Count)
        {
            throw new TypeException(
                $"Function '{e.FunctionName}' expects {funcInfo.parameters.Count} arguments, got {e.Arguments.Count}"
            );
        }

        // 2. Проверяем типы аргументов
        for (int i = 0; i < e.Arguments.Count; i++)
        {
            e.Arguments[i].Accept(this);
            ValueType argType = _types.Pop();

            ValueType expectedType = funcInfo.parameters[i].Type;

            if (expectedType != argType)
            {
                throw new TypeException(
                    $"Argument {i + 1} of '{e.FunctionName}' expects {TypeToString(expectedType)}, got {TypeToString(argType)}"
                );
            }
        }

        // 3. Возвращаем тип, который возвращает функция
        _types.Push(funcInfo.returnType);
    }

    private void DeclareFunction(FunctionDeclaration d)
    {
        ValueType returnType = ParseStringToType(d.ReturnType);

        List<(string, ValueType)> parameters = [];
        foreach ((string name, string type) param in d.Parameters)
        {
            ValueType paramType = ParseStringToType(param.type);
            parameters.Add((param.name, paramType));
        }

        _typeContext.DefineFunction(d.FuncName, returnType, parameters);
    }

    private static ValueType ParseStringToType(string typeName)
    {
        return typeName.ToLower() switch
        {
            "num" => ValueType.Number,
            "string" => ValueType.String,
            "bool" => ValueType.Boolean,
            "void" => ValueType.Void,
            "undefined" => ValueType.Undefined,
            "newline" => ValueType.NewLine,
            _ => throw new TypeException($"Unknown type: '{typeName}'")
        };
    }

    private static string TypeToString(ValueType type)
    {
        return type switch
        {
            ValueType.Number => "num",
            ValueType.String => "string",
            ValueType.Boolean => "bool",
            ValueType.Void => "void",
            ValueType.Undefined => "undefined",
            ValueType.NewLine => "newline",
            _ => "unknown"
        };
    }

    private ValueType CheckBinaryOperation(BinaryOperation op, ValueType left, ValueType right)
    {
        return (op, left, right) switch
        {
            (BinaryOperation.Plus, ValueType.Number, ValueType.Number) => ValueType.Number,
            (BinaryOperation.Plus, ValueType.String, ValueType.String) => ValueType.String,

            (BinaryOperation.Minus, ValueType.Number, ValueType.Number) => ValueType.Number,
            (BinaryOperation.Multiply, ValueType.Number, ValueType.Number) => ValueType.Number,
            (BinaryOperation.Divide, ValueType.Number, ValueType.Number) => ValueType.Number,
            (BinaryOperation.Modulo, ValueType.Number, ValueType.Number) => ValueType.Number,
            (BinaryOperation.Exponentiation, ValueType.Number, ValueType.Number) => ValueType.Number,

            (BinaryOperation.LooseEquality, ValueType l, ValueType r) when l == r => ValueType.Boolean,
            (BinaryOperation.NotEqual, ValueType l, ValueType r) when l == r => ValueType.Boolean,

            (BinaryOperation.LessThan, ValueType.Number, ValueType.Number) => ValueType.Boolean,
            (BinaryOperation.LessThan, ValueType.String, ValueType.String) => ValueType.Boolean,
            (BinaryOperation.GreaterThan, ValueType.Number, ValueType.Number) => ValueType.Boolean,
            (BinaryOperation.GreaterThan, ValueType.String, ValueType.String) => ValueType.Boolean,
            (BinaryOperation.LessThanOrEqual, ValueType.Number, ValueType.Number) => ValueType.Boolean,
            (BinaryOperation.LessThanOrEqual, ValueType.String, ValueType.String) => ValueType.Boolean,
            (BinaryOperation.GreaterThanOrEqual, ValueType.Number, ValueType.Number) => ValueType.Boolean,
            (BinaryOperation.GreaterThanOrEqual, ValueType.String, ValueType.String) => ValueType.Boolean,

            (BinaryOperation.And, ValueType.Boolean, ValueType.Boolean) => ValueType.Boolean,
            (BinaryOperation.And, ValueType.Number, ValueType.Number) => ValueType.Boolean,
            (BinaryOperation.And, ValueType.String, ValueType.String) => ValueType.Boolean,

            (BinaryOperation.Or, ValueType.Boolean, ValueType.Boolean) => ValueType.Boolean,
            (BinaryOperation.Or, ValueType.Number, ValueType.Number) => ValueType.Boolean,
            (BinaryOperation.Or, ValueType.String, ValueType.String) => ValueType.Boolean,

            _ => throw new TypeException(
                $"Operator {op} cannot be applied to types {left} and {right}"
            )
        };
    }
}