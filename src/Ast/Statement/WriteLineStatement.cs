using Blang.Ast.Expressions;

namespace Blang.Ast.Statement;
public sealed class WriteLineStatement : Statement
{
    public WriteLineStatement(List<Expression> expressions)
    {
        Expressions = expressions;
    }

    public List<Expression> Expressions { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}