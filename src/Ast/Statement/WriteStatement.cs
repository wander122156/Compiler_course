using Blang.Ast.Expressions;

namespace Blang.Ast.Statement;
public sealed class WriteStatement : Statement
{
    public WriteStatement(List<Expression> expressions)
    {
        Expressions = expressions;
    }

    public List<Expression> Expressions { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}