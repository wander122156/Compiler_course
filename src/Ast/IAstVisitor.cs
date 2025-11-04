using Ast.Expressions;

namespace Ast;

public interface IAstVisitor
{
    public void Visit(BinaryOperationExpression e);

    // public void Visit(другие операции);
}
