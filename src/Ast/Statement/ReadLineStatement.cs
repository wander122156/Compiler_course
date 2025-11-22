namespace Blang.Ast.Statement;
public sealed class ReadLineStatement : Statement
{
    public List<string> VariableNames { get; }

    public ReadLineStatement(List<string> variableNames)
    {
        VariableNames = variableNames;
    }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}
