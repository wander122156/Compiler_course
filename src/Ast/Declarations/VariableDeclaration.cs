using Blang.Ast.Expressions;

namespace Blang.Ast.Declarations;
public sealed class VariableDeclaration : Declaration
{
    public VariableDeclaration(
        string name,
        string declaredTypeName,
        Expression? value)
    {
        Name = name;
        DeclaredTypeName = declaredTypeName;
        Value = value;
    }

    public string Name { get; }

    public string DeclaredTypeName { get; }

    public Expression? Value { get; }

    public override void Accept(IAstVisitor visitor)
    {
        visitor.Visit(this);
    }
}
