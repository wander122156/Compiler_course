using System.Linq;

using Blang.Ast;
using Blang.Ast.Declarations;
using Blang.Ast.Expressions;
using Blang.Ast.Statement;
using Blang.Common;
using Blang.Execution;

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

    private ValueType _functionReturnType;
    private bool _functionHasReturn;

    public SemanticChecker()
    {
        _typeContext = new TypeContext();
    }

    public void Check(List<IAstElement> root)
    {
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

    public static ValueType ParseStringToType(string typeName)
    {
        return typeName.ToLower() switch
        {
            "num" => ValueType.Number,
            "string" => ValueType.String,
            "bool" => ValueType.Boolean,
            "void" => ValueType.Void,
            "undefined" => ValueType.Undefined,
            "null" => ValueType.Null,
            "newline" => ValueType.NewLine,
            _ => throw new TypeException($"Unknown type: '{typeName}'")
        };
    }

    public static string TypeToString(ValueType type)
    {
        return type switch
        {
            ValueType.Number => "number",
            ValueType.String => "string",
            ValueType.Boolean => "boolean",
            ValueType.Void => "void",
            ValueType.Undefined => "undefined",
            ValueType.Null => "null",
            ValueType.NewLine => "newline",
            _ => "unknown"
        };
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

        _typeContext.DefineVariableType(d.Name, declaredType); // ?
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

            _typeContext.DefineVariableType(declaration.Name, declaredType); // ?
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

        switch (e.Operation)
        {
            case BinaryOperation.Plus:
            case BinaryOperation.Minus:
            case BinaryOperation.Multiply:
            case BinaryOperation.Divide:
            case BinaryOperation.Modulo:
            case BinaryOperation.Exponentiation:
                if (leftType != ValueType.Number || rightType != ValueType.Number)
                    throw new TypeException($"Arithmetic op requires num but got {leftType} and {leftType}");
                break;

            case BinaryOperation.LessThan:
            case BinaryOperation.GreaterThan:
            case BinaryOperation.LessThanOrEqual:
            case BinaryOperation.GreaterThanOrEqual:
                if (leftType != ValueType.Number || rightType != ValueType.Number )
                    throw new TypeException($"Comparison requires num but got {leftType} and {rightType}");
                break;

            case BinaryOperation.LooseEquality:
            case BinaryOperation.NotEqual:
                if (leftType != rightType)
                    throw new TypeException($"Cannot compare {leftType} and {rightType}");
                break;

            default:
                throw new TypeException($"Unknown binary operation {e.Operation}");
        }
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
                        $"Unary operator requires number, got {operandType}"
                    );
                }

                _types.Push(ValueType.Number);
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
            conditionType != ValueType.Number )
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
        // Сохраняем текущий контекст функции
        ValueType oldReturnType = _functionReturnType;
        bool oldHasReturn = _functionHasReturn;

        ValueType returnType = ParseStringToType(d.ReturnType);
        _functionReturnType = returnType;
        _functionHasReturn = false;

        // Создаем область видимости для параметров
        _typeContext.PushScope();

        // Объявляем параметры
        List<(string, ValueType)> parameters = [];
        foreach ((string name, string type) param in d.Parameters)
        {
            ValueType paramType = ParseStringToType(param.type);
            parameters.Add((param.name, paramType));

            _typeContext.DefineVariableType(param.name, paramType);
        }

        // Регистрируем функцию в TypeContext
        _typeContext.DefineFunction(d.FuncName, returnType, parameters);

        d.Body.Accept(this);

        // Проверяем, что не-void функция имеет return
        if (_functionReturnType != ValueType.Void && !_functionHasReturn)
        {
            throw new TypeException(
                $"Function '{d.FuncName}' must return a value of type {TypeToString(_functionReturnType)}"
            );
        }

        _typeContext.PopScope();

        // Восстанавливаем предыдущий контекст
        _functionReturnType = oldReturnType;
        _functionHasReturn = oldHasReturn;
    }

    public void Visit(ReturnStatement s)
    {
        _functionHasReturn = true;

        if (_functionReturnType == ValueType.Void)
        {
            // Void функция не должна возвращать значение
            if (s.ReturnValue != null)
            {
                throw new TypeException("Void function cannot return a value");
            }
        }
        else
        {
            // Не-void функция должна возвращать значение
            if (s.ReturnValue == null)
            {
                throw new TypeException("Function must return a value");
            }

            s.ReturnValue.Accept(this);
            ValueType returnType = _types.Pop();

            if (_functionReturnType != returnType)
            {
                throw new TypeException(
                    $"Function returns {TypeToString(returnType)}, expected {TypeToString(_functionReturnType)}"
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
        else if (_typeContext.HasFunction(e.FunctionName))
        {
            TypeContext.FunctionInfo funcInfo = _typeContext.GetFunctionInfo(e.FunctionName);
            CheckFunctionCall(e, funcInfo);
            return;
        }

        throw new TypeException($"Function '{e.FunctionName}' is not defined");
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

        // Проверяем количество аргументов
        if (e.Arguments.Count != typeInfo.ParameterTypes.Count)
        {
            throw new TypeException(
                $"Builtin function '{e.FunctionName}' expects {typeInfo.ParameterTypes.Count} arguments, got {e.Arguments.Count}"
            );
        }

        // Проверяем типы аргументов (все должны быть Number)
        for (int i = 0; i < e.Arguments.Count; i++)
        {
            e.Arguments[i].Accept(this);
            ValueType argType = _types.Pop();

            if (argType != ValueType.Number)
            {
                throw new TypeException(
                    $"Argument {i + 1} of '{e.FunctionName}' expects number, got {TypeToString(argType)}"
                );
            }
        }

        _types.Push(ValueType.Number);
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
}