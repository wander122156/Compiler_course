using Blang.Ast.Statement;

namespace Blang.Ast.Declarations;
public sealed class FunctionDeclaration : Declaration
{
    public FunctionDeclaration(
        string funcName,
        List<(string name, string type)> parameters,
        CompoundStatement body
        )
    {
        FuncName = funcName;
        Parameters = parameters;
        Body = body;
    }

    public string FuncName { get; }

    public List<(string name, string type)> Parameters { get; }

    public CompoundStatement Body { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}
