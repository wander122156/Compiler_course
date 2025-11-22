namespace Blang.Ast.Statement;
public sealed class ReadStatement : Statement
{
    public List<string> VariableNames { get; }

    public ReadStatement(List<string> variableNames)
    {
        VariableNames = variableNames;
    }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}
