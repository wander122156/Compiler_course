namespace Blang.Ast.Statement;
public sealed class CompoundStatement : Statement
{
    public CompoundStatement(List<IAstElement> statements)
    {
        Statements = statements;
    }

    public List<IAstElement> Statements { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}
