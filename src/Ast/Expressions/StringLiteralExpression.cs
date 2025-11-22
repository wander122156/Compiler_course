namespace Blang.Ast.Expressions;
public sealed class StringLiteralExpression : Expression
{
    public StringLiteralExpression(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}
