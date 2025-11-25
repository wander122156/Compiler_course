namespace Blang.Ast.Statement;
public sealed class ContinueStatement : Statement
{
    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}
