using Blang.Ast.Expressions;

namespace Blang.Ast.Statement;
public sealed class DoWhileLoopStatement : Statement
{
    public DoWhileLoopStatement(
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
