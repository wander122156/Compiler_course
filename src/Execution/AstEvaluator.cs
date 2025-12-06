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
        RuntimeValue value = RuntimeValue.Undefined();
        if (d.Value != null)
        {
            d.Value.Accept(this);
            value = _values.Pop();
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
            RuntimeValue value = RuntimeValue.Undefined();
            if (declaration.Value != null)
            {
                declaration.Value.Accept(this);
                value = _values.Pop();
            }

            _context.DefineVariable(declaration.Name, value);
        }
    }

    public void Visit(ConstantDeclaration d)
    {
        d.Value.Accept(this);
        RuntimeValue value = _values.Pop();
        _context.DefineConstant(d.Name, value);
    }

    public void Visit(AssignmentExpression e)
    {
        e.Value.Accept(this);
        RuntimeValue value = _values.Pop();
        _context.AssignVariable(e.Name, value);
    }

    // Это получение значения существующей переменной
    public void Visit(VariableExpression e)
    {
        _values.Push(_context.GetValue(e.Name));
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
            _context.AssignVariable(variableName, value);
        }
    }

    public void Visit(ReadLineStatement s)
    {
        foreach (string variableName in s.VariableNames)
        {
            RuntimeValue value = _environment.Readln();
            _context.AssignVariable(variableName, value);
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

    public void Visit(ForLoopStatement s)
    {
        _context.PushScope(new Scope());

        try
        {
            s.Initialization.Accept(this);

            while (true)
            {
                s.Condition.Accept(this);
                bool condition = ConvertToBoolean(_values.Pop());
                if (!condition)
                {
                    break;
                }

                s.Body.Accept(this);

                if (_context.ShouldBreak) break;
                if (_context.ShouldContinue)
                {
                    _context.ResetFlowControl();
                    s.Increment.Accept(this);
                    continue;
                }

                s.Increment.Accept(this);
            }
        }
        finally
        {
            _context.PopScope();
        }
    }

    public void Visit(WhileLoopStatement s)
    {
        _context.PushScope(new Scope());

        try
        {
            while (true)
            {
                _context.ResetFlowControl();

                s.Condition.Accept(this);
                bool condition = ConvertToBoolean(_values.Pop());
                if (!condition) break;

                s.Body.Accept(this);

                if (_context.ShouldBreak) break;
            }
        }
        finally
        {
            _context.PopScope();
        }
    }

    public void Visit(DoWhileLoopStatement s)
    {
        _context.PushScope(new Scope());

        try
        {
            while (true)
            {
                _context.ResetFlowControl();

                s.Body.Accept(this);

                if (_context.ShouldBreak) break;

                s.Condition.Accept(this);

                bool condition = ConvertToBoolean(_values.Pop());
                if (!condition) break;
            }
        }
        finally
        {
            _context.PopScope();
        }
    }

    public void Visit(FunctionCallExpression e)
    {
        // TODO: Добавить проверку типов параметров и разный тип аргументов
        List<decimal> arguments = new();

        foreach (Expression arg in e.Arguments)
        {
            arg.Accept(this);
            arguments.Add((decimal)_values.Pop().Value);
        }

        if (BuiltinFunctions.IsBuiltin(e.FunctionName))
        {
            decimal result = BuiltinFunctions.Invoke(e.FunctionName, arguments);
            _values.Push(RuntimeValue.Number(result));
        }
        else if(_context.HasFunction(e.FunctionName))
        {
            FunctionDeclaration function = _context.GetFunction(e.FunctionName);
            decimal? result = ExecuteUserFunction(function, arguments);

            if (result != null)
            {
                _values.Push(RuntimeValue.Number(result.Value));
            }

            // иначе функция ничего не возвращает
        }
        else
        {
            throw new InvalidOperationException($"Function '{e.FunctionName}' is not defined");
        }
    }

    public void Visit(ReturnStatement s)
    {
        s.ReturnValue.Accept(this);
        _context.ShouldReturn = true;
    }

    public void Visit(BreakStatement s)
    {
        _context.ShouldBreak = true;
    }

    public void Visit(ContinueStatement s)
    {
        _context.ShouldContinue = true;
    }

    public void Visit(FunctionDeclaration d)
    {
        _context.DefineFunction(d.FuncName, d);
    }

    public void Visit(CompoundStatement s)
    {
        _context.PushScope(new Scope());

        try
        {
            foreach (IAstElement statement in s.Statements)
            {
                if (_context.ShouldBreak || _context.ShouldContinue || _context.ShouldReturn)
                {
                    break;
                }

                statement.Accept(this);
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

    private decimal? ExecuteUserFunction(FunctionDeclaration function, List<decimal> arguments)
    {
        if (arguments.Count != function.Parameters.Count)
        {
            throw new InvalidOperationException(
                $"Function '{function.FuncName}' expects {function.Parameters.Count} arguments but got {arguments.Count}");
        }

        _context.PushScope(new Scope());

        try
        {
            for (int i = 0; i < function.Parameters.Count; i++)
            {
                (string paramName, string paramType) = function.Parameters[i];

                // TODO: Добавить проверку типов параметров
                _context.DefineVariable(paramName, RuntimeValue.Number(arguments[i]));
            }

            function.Body.Accept(this);

            if (_values.Count > 0 && _context.ShouldReturn)
            {
                return (decimal)_values.Pop();
            }

            return null;
        }
        finally
        {
            _context.PopScope();
        }
    }
}