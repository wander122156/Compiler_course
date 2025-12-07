using Blang.Ast.Statement;

namespace Blang.Ast.Declarations;
public sealed class FunctionDeclaration : Declaration
{
    public FunctionDeclaration(
        string funcName,
        string returnType,
        List<(string name, string type)> parameters,
        CompoundStatement body
        )
    {
        FuncName = funcName;
        ReturnType = returnType;
        Parameters = parameters;
        Body = body;
    }

    public string FuncName { get; }

    public string ReturnType { get; }

    public List<(string name, string type)> Parameters { get; }

    public CompoundStatement Body { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}
