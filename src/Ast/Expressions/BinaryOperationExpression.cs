namespace Ast.Expressions;
public sealed class BinaryOperationExpression : Expression
{
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
        /// Операция сравнения "меньше".
        /// </summary>
        LessThan,
    }

    public Expression Left { get; }

    public BinaryOperation Operation { get; }

    public Expression Right { get; }

    public BinaryOperationExpression(Expression left, BinaryOperation operation, Expression right)
    {
        Left = left;
        Operation = operation;
        Right = right;
    }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}
