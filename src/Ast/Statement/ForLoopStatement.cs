using Blang.Ast.Expressions;

namespace Blang.Ast.Statement;
public sealed class ForLoopStatement : Statement
{
    public ForLoopStatement(
        IAstElement initialization,
        Expression condition,
        AssignmentExpression increment,
        CompoundStatement body)
    {
        Initialization = initialization;
        Condition = condition;
        Increment = increment;
        Body = body;
    }

    public IAstElement Initialization { get; }

    public Expression Condition { get; }

    public AssignmentExpression Increment { get; }

    public CompoundStatement Body { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}
