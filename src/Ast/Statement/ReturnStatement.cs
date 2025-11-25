using Blang.Ast.Expressions;

namespace Blang.Ast.Statement;
public sealed class ReturnStatement : Statement
{
    public ReturnStatement(Expression returnValue)
    {
        ReturnValue = returnValue;
    }

    public Expression ReturnValue { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}