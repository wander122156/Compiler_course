namespace Blang.Ast.Expressions;
public sealed class FunctionCallExpression : Expression
{
    public FunctionCallExpression(string functionName, List<Expression> arguments)
    {
        FunctionName = functionName;
        Arguments = arguments;
    }

    public string FunctionName { get; }

    public List<Expression> Arguments { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}
