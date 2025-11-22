namespace Blang.Ast.Expressions;
public sealed class NumericLiteralExpression : Expression
{
    public NumericLiteralExpression(decimal value)
    {
        Value = value;
    }

    public decimal Value { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}
