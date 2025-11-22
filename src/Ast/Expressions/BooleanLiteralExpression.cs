namespace Blang.Ast.Expressions;

public sealed class BooleanLiteralExpression : Expression
{
    public BooleanLiteralExpression(bool value)
    {
          Value = value;
    }

    public bool Value { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}
