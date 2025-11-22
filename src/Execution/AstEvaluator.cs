using Blang.Ast;
using Blang.Ast.Declarations;
using Blang.Ast.Expressions;
using Blang.Ast.Statement;
using Blang.Common;

namespace Blang.Execution;
public class AstEvaluator : IAstVisitor
{
    private readonly Context _context;
    private readonly IEnvironment _environment;
    private readonly Stack<RuntimeValue> _values = [];

    public AstEvaluator(Context context, IEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    public void Evaluate(IAstElement node)
    {
        if (_values.Count > 0)
        {
            throw new InvalidOperationException(
                $"Evaluation stack must be empty, but contains {_values.Count} values: {string.Join(", ", _values)}"
            );
        }

        node.Accept(this);
    }

    public void Visit(BinaryOperationExpression e)
    {
        e.Left.Accept(this);
        e.Right.Accept(this);
        decimal right = (decimal)_values.Pop().Value;
        decimal left = (decimal)_values.Pop().Value;

        switch (e.Operation)
        {
            case BinaryOperation.Plus:
                _values.Push(RuntimeValue.Number(left + right));
                break;
            case BinaryOperation.Minus:
                _values.Push(RuntimeValue.Number(left - right));
                break;
            case BinaryOperation.Multiply:
                _values.Push(RuntimeValue.Number(left * right));
                break;
            case BinaryOperation.Divide:
                _values.Push(RuntimeValue.Number(right != 0 ? left / right : throw new DivideByZeroException("Division by zero")));
                break;
            case BinaryOperation.Modulo:
                _values.Push(RuntimeValue.Number(right != 0 ? left % right : throw new DivideByZeroException("Modulo by zero")));
                break;
            case BinaryOperation.Exponentiation:
                if (left == 0 && right <= 0)
                {
                    throw new DivideByZeroException("Zero cannot be raised to a non-positive power");
                }

                _values.Push(RuntimeValue.Number((decimal)Math.Pow((double)left, (double)right)));
                break;
            case BinaryOperation.LessThan:
                _values.Push(RuntimeValue.Boolean(left < right));
                break;
            case BinaryOperation.GreaterThan:
                _values.Push(RuntimeValue.Boolean(left > right));
                break;
            case BinaryOperation.LessThanOrEqual:
                _values.Push(RuntimeValue.Boolean(left <= right));
                break;
            case BinaryOperation.GreaterThanOrEqual:
                _values.Push(RuntimeValue.Boolean(left >= right));
                break;
            case BinaryOperation.LooseEquality:
                _values.Push(RuntimeValue.Boolean(left == right));
                break;
            case BinaryOperation.NotEqual:
                _values.Push(RuntimeValue.Boolean(left != right));
                break;
            default:
                throw new NotImplementedException($"Unknown binary operation {e.Operation}");
        }
    }

    public void Visit(UnaryOperationExpression e)
    {
        e.Operand.Accept(this);
        decimal value = (decimal)_values.Pop().Value;

        switch (e.Operation)
        {
            case UnaryOperation.Minus:
                _values.Push(RuntimeValue.Number(-value));
                break;
            case UnaryOperation.Plus:
                _values.Push(RuntimeValue.Number(value));
                break;
            default:
                throw new NotImplementedException($"Unknown unary operation {e.Operation}");
        }
    }

    public void Visit(NumericLiteralExpression e)
    {
        _values.Push(RuntimeValue.Number(e.Value));
    }

    public void Visit(StringLiteralExpression e)
    {
        _values.Push(RuntimeValue.String(e.Value));
    }

    public void Visit(BooleanLiteralExpression e)
    {
        _values.Push(RuntimeValue.Boolean(e.Value));
    }

    public void Visit(VariableDeclaration d)
    {
        // NOTE: Вычисляем инициализирующее выражение, и затем присваиваем его значение переменной,
        //  сохраняя результат в стеке.
        decimal value = 0m;
        if (d.Value != null)
        {
            d.Value.Accept(this);
            value = (decimal)_values.Peek().Value;
        }

        _context.DefineVariable(d.Name, value);
    }

    public void Visit(VariableDeclarationStatement statement)
    {
        // Создаем ОДНУ область видимости для всех переменных в этом объявлении
        _context.PushScope(new Scope());

        // Объявляем все переменные в этой области видимости
        foreach (VariableDeclaration declaration in statement.Declarations)
        {
            decimal value = 0m;
            if (declaration.Value != null)
            {
                declaration.Value.Accept(this);
                value = (decimal)_values.Pop().Value;
            }

            _context.DefineVariable(declaration.Name, value);
        }
    }

    public void Visit(ConstantDeclaration d)
    {
        d.Value.Accept(this);
        decimal value = (decimal)_values.Pop().Value;
        _context.DefineConstant(d.Name, value);
    }

    public void Visit(AssignmentExpression e)
    {
        e.Value.Accept(this);
        RuntimeValue value = _values.Pop();
        _context.AssignVariable(e.Name, (decimal)value.Value);
    }

    // Это получение значения существующей переменной
    public void Visit(VariableExpression e)
    {
        _values.Push(RuntimeValue.Number(_context.GetValue(e.Name)));
    }

    public void Visit(FunctionCallExpression e)
    {
        List<decimal> arguments = new();
        foreach (Expression arg in e.Arguments)
        {
            arg.Accept(this);
            arguments.Add((decimal)_values.Pop().Value);
        }

        // Вызываем встроенную функцию
        decimal result = BuiltinFunctions.Invoke(e.FunctionName, arguments);
        _values.Push(RuntimeValue.Number(result));
    }

    public void Visit(WriteStatement s)
    {
        foreach (Expression expr in s.Expressions)
        {
            expr.Accept(this);
            RuntimeValue value = _values.Pop();
            _environment.Write(value);
        }
    }

    public void Visit(WriteLineStatement s)
    {
        foreach (Expression expr in s.Expressions)
        {
            expr.Accept(this);
            RuntimeValue value = _values.Pop();
            _environment.Write(value);
        }

        _environment.Writeln(RuntimeValue.NewLine());
    }

    public void Visit(ReadStatement s)
    {
        foreach (string variableName in s.VariableNames)
        {
            RuntimeValue value = _environment.Read();

            if (value.Type == RuntimeValue.ValueType.Number)
            {
                _context.AssignVariable(variableName, (decimal)value.Value);
            }
            else
            {
                throw new InvalidOperationException($"Read expected number but got {value.Type}");
            }
        }
    }

    public void Visit(ReadLineStatement s)
    {
        foreach (string variableName in s.VariableNames)
        {
            RuntimeValue value = _environment.Readln();

            if (value.Type == RuntimeValue.ValueType.Number)
            {
                _context.AssignVariable(variableName, (decimal)value.Value);
            }
            else
            {
                throw new InvalidOperationException($"Readln expected number but got {value.Type}");
            }
        }
    }

    public void Visit(IfElseStatement s)
    {
        s.Condition.Accept(this);
        RuntimeValue conditionValue = _values.Pop();

        bool isTrue = ConvertToBoolean(conditionValue);

        if (isTrue)
        {
            s.ThenBranch.Accept(this);
        }
        else if (s.ElseBranch != null)
        {
            s.ElseBranch.Accept(this);
        }

        // Если условие false и нет else - ничего не делаем
    }

    public void Visit(CompoundStatement s)
    {
        _context.PushScope(new Scope());

        try
        {
            foreach (IAstElement statement in s.Statements)
            {
                statement.Accept(this);
                if (_values.Count > 0)
                {
                    _values.Pop();
                }
            }
        }
        finally
        {
            _context.PopScope();
        }
    }

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
