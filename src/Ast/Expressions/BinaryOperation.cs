namespace Blang.Ast.Expressions;

public enum BinaryOperation
{
    /// <summary>
    /// Операция сложения.
    /// </summary>
    Plus,

    /// <summary>
    /// Операция вычитания.
    /// </summary>
    Minus,

    /// <summary>
    /// Операция умножения.
    /// </summary>
    Multiply,

    /// <summary>
    /// Операция деления.
    /// </summary>
    Divide,

    /// <summary>
    /// Операция деления по модулю.
    /// </summary>
    Modulo,

    /// <summary>
    /// Операция возведения в степень.
    /// </summary>
    Exponentiation,

    /// <summary>
    /// Операция сравнения "меньше".
    /// </summary>
    LessThan,

    /// <summary>
    /// Операция сравнения "больше".
    /// </summary>
    GreaterThan,

    /// <summary>
    /// Операция сравнения "больше или равно".
    /// </summary>
    GreaterThanOrEqual,

    /// <summary>
    /// Операция сравнения "меньше или равно".
    /// </summary>
    LessThanOrEqual,

    /// <summary>
    /// Операция сравнения "не равно".
    /// </summary>
    NotEqual,

    /// <summary>
    /// Операция сравнения "равно".
    /// </summary>
    LooseEquality,

    /// <summary>
    /// Логическая операция "и".
    /// </summary>
    And,

    /// <summary>
    /// Логическая операция "или".
    /// </summary>
    Or,
}