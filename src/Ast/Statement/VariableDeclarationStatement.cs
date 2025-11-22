using Blang.Ast.Declarations;

namespace Blang.Ast.Statement;
public sealed class VariableDeclarationStatement : Statement
{
    public List<VariableDeclaration> Declarations { get; }

    public VariableDeclarationStatement(List<VariableDeclaration> declarations)
    {
        Declarations = declarations;
    }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}