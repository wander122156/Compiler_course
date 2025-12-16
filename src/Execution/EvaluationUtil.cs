using Blang.Ast;
using Blang.Ast.Declarations;
using Blang.Ast.Expressions;
using Blang.Ast.Statement;
using Blang.Common;

namespace Blang.Execution;
public static class EvaluationUtil
{
    public static RuntimeValue ApplyBinaryOperation(
        BinaryOperation operation,
        Func<RuntimeValue> evaluateLeft,
        Func<RuntimeValue> evaluateRight
    )
    {
        return operation switch
        {
            BinaryOperation.Plus => ApplyPlusOperation(evaluateLeft, evaluateRight),
            BinaryOperation.Minus => ApplyArithmeticOperation(
                evaluateLeft,
                evaluateRight,
                (i1, i2) => i1 - i2
            ),
            BinaryOperation.Multiply => ApplyArithmeticOperation(
                evaluateLeft,
                evaluateRight,
                (i1, i2) => i1 * i2
            ),
            BinaryOperation.Divide => ApplyArithmeticOperation(
                evaluateLeft,
                evaluateRight,
                (i1, i2) => i2 != 0 ? i1 / i2 : throw new DivideByZeroException("Division by zero")
            ),
            BinaryOperation.Modulo => ApplyArithmeticOperation(
                evaluateLeft,
                evaluateRight,
                (i1, i2) => i2 != 0 ? i1 % i2 : throw new DivideByZeroException("Modulo by zero")
            ),
            BinaryOperation.Exponentiation => ApplyArithmeticOperation(
                evaluateLeft,
                evaluateRight,
                (i1, i2) =>
                {
                    if (i1 == 0 && i2 <= 0)
                        throw new InvalidOperationException("Zero cannot be raised to a non-positive power");
                    return (decimal)Math.Pow((double)i1, (double)i2);
                }
            ),
            BinaryOperation.LooseEquality => ApplyEqualityOperation(
                evaluateLeft,
                evaluateRight,
                (i1, i2) => i1 == i2,
                (s1, s2) => s1 == s2,
                (b1, b2) => b1 == b2
            ),
            BinaryOperation.NotEqual => ApplyEqualityOperation(
                evaluateLeft,
                evaluateRight,
                (i1, i2) => i1 != i2,
                (s1, s2) => s1 != s2,
                (b1, b2) => b1 != b2
            ),
            BinaryOperation.LessThan => ApplyComparisonOperation(
                evaluateLeft,
                evaluateRight,
                (i1, i2) => i1 < i2,
                (s1, s2) => string.CompareOrdinal(s1, s2) < 0
            ),
            BinaryOperation.GreaterThan => ApplyComparisonOperation(
                evaluateLeft,
                evaluateRight,
                (i1, i2) => i1 > i2,
                (s1, s2) => string.CompareOrdinal(s1, s2) > 0
            ),
            BinaryOperation.LessThanOrEqual => ApplyComparisonOperation(
                evaluateLeft,
                evaluateRight,
                (i1, i2) => i1 <= i2,
                (s1, s2) => string.CompareOrdinal(s1, s2) <= 0
            ),
            BinaryOperation.GreaterThanOrEqual => ApplyComparisonOperation(
                evaluateLeft,
                evaluateRight,
                (i1, i2) => i1 >= i2,
                (s1, s2) => string.CompareOrdinal(s1, s2) >= 0
            ),
            _ => throw new NotImplementedException($"Unknown binary operation {operation}"),
        };
    }

    /// <summary>
    /// Выполняет операцию сложения чисел или конкатенации строк)
    /// </summary>
    private static RuntimeValue ApplyPlusOperation(
        Func<RuntimeValue> evaluateLeft,
        Func<RuntimeValue> evaluateRight)
    {
        RuntimeValue left = evaluateLeft();
        RuntimeValue right = evaluateRight();

        // Проверяем типы и выполняем соответствующую операцию
        return (left.Type, right.Type) switch
        {
            (RuntimeValue.ValueType.Number, RuntimeValue.ValueType.Number) =>
                RuntimeValue.Number((decimal)left + (decimal)right),

            (RuntimeValue.ValueType.String, RuntimeValue.ValueType.String) =>
                RuntimeValue.String((string)left + (string)right),

            _ => throw new InvalidOperationException(
                $"Operator '+' cannot be applied to types {left.Type} and {right.Type}. " +
                "Use with numbers or strings only.")
        };
    }

    /// <summary>
    /// Выполняет арифметическую операцию, если оба операнда являются числами.
    /// Иначе бросает исключение.
    /// </summary>
    private static RuntimeValue ApplyArithmeticOperation(
        Func<RuntimeValue> evaluateLeft,
        Func<RuntimeValue> evaluateRight,
        Func<decimal, decimal, decimal> operation
    )
    {
        RuntimeValue left = evaluateLeft();
        RuntimeValue right = evaluateRight();

        if (left.Type != RuntimeValue.ValueType.Number || right.Type != RuntimeValue.ValueType.Number)
        {
            throw new InvalidOperationException(
                $"Arithmetic operation requires numbers, got {left.Type} and {right.Type}");
        }

        decimal leftNum = (decimal)left;
        decimal rightNum = (decimal)right;

        return RuntimeValue.Number(operation(leftNum, rightNum));
    }

    /// <summary>
    /// Проверяет равенство/неравенство для чисел, строк и булевых значений
    /// </summary>
    private static RuntimeValue ApplyEqualityOperation(
        Func<RuntimeValue> evaluateLeft,
        Func<RuntimeValue> evaluateRight,
        Func<decimal, decimal, bool> compareDecimals,
        Func<string, string, bool> compareStrings,
        Func<bool, bool, bool> compareBooleans
    )
    {
        RuntimeValue left = evaluateLeft();
        RuntimeValue right = evaluateRight();

        if (left.Type != right.Type)
        {
            throw new InvalidOperationException(
                $"Cannot compare values of different types: {left.Type} and {right.Type}");
        }

        return (left.Type, right.Type) switch
        {
            (RuntimeValue.ValueType.Number, RuntimeValue.ValueType.Number) =>
                RuntimeValue.Boolean(compareDecimals((decimal)left, (decimal)right)),

            (RuntimeValue.ValueType.String, RuntimeValue.ValueType.String) =>
                RuntimeValue.Boolean(compareStrings((string)left, (string)right)),

            (RuntimeValue.ValueType.Boolean, RuntimeValue.ValueType.Boolean) =>
                RuntimeValue.Boolean(compareBooleans((bool)left, (bool)right)),

            _ => throw new InvalidOperationException(
                $"Cannot check equality for types {left.Type} and {right.Type}")
        };
    }

    /// <summary>
    /// Сравнивает два операнда (<, >, <=, >=) для чисел и строк
    /// </summary>
    private static RuntimeValue ApplyComparisonOperation(
        Func<RuntimeValue> evaluateLeft,
        Func<RuntimeValue> evaluateRight,
        Func<decimal, decimal, bool> compareDecimals,
        Func<string, string, bool> compareStrings
    )
    {
        RuntimeValue left = evaluateLeft();
        RuntimeValue right = evaluateRight();

        if (left.Type != right.Type)
        {
            throw new InvalidOperationException(
                $"Cannot compare values of different types: {left.Type} and {right.Type}");
        }

        return (left.Type, right.Type) switch
        {
            (RuntimeValue.ValueType.Number, RuntimeValue.ValueType.Number) =>
                RuntimeValue.Boolean(compareDecimals((decimal)left, (decimal)right)),

            (RuntimeValue.ValueType.String, RuntimeValue.ValueType.String) =>
                RuntimeValue.Boolean(compareStrings((string)left, (string)right)),

            (RuntimeValue.ValueType.Boolean, RuntimeValue.ValueType.Boolean) =>
                throw new InvalidOperationException(
                    $"Comparison operators (<, >, <=, >=) are not supported for boolean values"),

            _ => throw new InvalidOperationException(
                $"Cannot compare values of type {left.Type}")
        };
    }
}