using Blang.Ast.Expressions;

namespace Blang.Ast.Statement;
public sealed class WhileLoopStatement : Statement
{
    public WhileLoopStatement(
        Expression condition,
        CompoundStatement body)
    {
        Condition = condition;
        Body = body;
    }

    public Expression Condition { get; }

    public CompoundStatement Body { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}
