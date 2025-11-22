namespace Blang.Ast;

public abstract class IAstElement
{
    public abstract void Accept(IAstVisitor visitor);
}
