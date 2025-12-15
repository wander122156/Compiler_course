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
            BinaryOperation.Plus => ApplyArithmeticOperation(
                    evaluateLeft,
                    evaluateRight,
                    (i1, i2) => i1 + i2
                ),
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
            BinaryOperation.LooseEquality => ApplyComparisonOperation(
                evaluateLeft,
                evaluateRight,
                (i1, i2) => i1 == i2,
                (s1, s2) => s1 == s2,
                (b1, b2) => b1 == b2
            ),
            BinaryOperation.NotEqual => ApplyComparisonOperation(
                evaluateLeft,
                evaluateRight,
                (i1, i2) => i1 != i2,
                (s1, s2) => s1 != s2,
                (b1, b2) => b1 == b2
            ),
            BinaryOperation.LessThan => ApplyComparisonOperation(
                evaluateLeft,
                evaluateRight,
                (i1, i2) => i1 < i2,
                (s1, s2) => string.CompareOrdinal(s1, s2) < 0,
                (b1, b2) => b1 == b2
            ),
            BinaryOperation.GreaterThan => ApplyComparisonOperation(
                evaluateLeft,
                evaluateRight,
                (i1, i2) => i1 > i2,
                (s1, s2) => string.CompareOrdinal(s1, s2) > 0,
                (b1, b2) => b1 == b2
            ),
            BinaryOperation.LessThanOrEqual => ApplyComparisonOperation(
                evaluateLeft,
                evaluateRight,
                (i1, i2) => i1 <= i2,
                (s1, s2) => string.CompareOrdinal(s1, s2) <= 0,
                (b1, b2) => b1 == b2
            ),
            BinaryOperation.GreaterThanOrEqual => ApplyComparisonOperation(
                evaluateLeft,
                evaluateRight,
                (i1, i2) => i1 >= i2,
                (s1, s2) => string.CompareOrdinal(s1, s2) >= 0,
                (b1, b2) => b1 == b2
            ),
            _ => throw new NotImplementedException($"Unknown binary operation {operation}"),
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
        decimal left = (decimal)evaluateLeft();
        decimal right = (decimal)evaluateRight();
        return RuntimeValue.Number(operation(left, right));
    }

    /// <summary>
    /// Сравнивает два операнда, если они оба являются числами, строками или bool.
    /// Иначе бросает исключение.
    /// </summary>
    private static RuntimeValue ApplyComparisonOperation(
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
            throw new InvalidOperationException($"Cannot compare values of different types: {left.Type} and {right.Type}");
        }

        return (left.Type, right.Type) switch
        {
            (RuntimeValue.ValueType.Number, RuntimeValue.ValueType.Number) =>
                RuntimeValue.Boolean(compareDecimals((decimal)left, (decimal)right)),

            (RuntimeValue.ValueType.String, RuntimeValue.ValueType.String) =>
                RuntimeValue.Boolean(compareStrings((string)left, (string)right)),

            (RuntimeValue.ValueType.Boolean, RuntimeValue.ValueType.Boolean) =>
                RuntimeValue.Boolean(compareBooleans((bool)left, (bool)right)),

            _ => throw new InvalidOperationException($"Values are not comparable: {left.Type} and {right.Type}")
        };
    }
}
