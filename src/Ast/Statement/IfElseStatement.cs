using Blang.Ast.Expressions;

namespace Blang.Ast.Statement;
public sealed class IfElseStatement : Statement
{
    public IfElseStatement(Expression condition, IAstElement thenBranch, IAstElement? elseBranch = null)
    {
        Condition = condition;
        ThenBranch = thenBranch;
        ElseBranch = elseBranch;
    }

    public Expression Condition { get; }

    public IAstElement ThenBranch { get; }

    public IAstElement? ElseBranch { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}
