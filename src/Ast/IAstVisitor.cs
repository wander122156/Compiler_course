using Blang.Ast.Declarations;
using Blang.Ast.Expressions;
using Blang.Ast.Statement;

namespace Blang.Ast;

public interface IAstVisitor
{
    public void Visit(BinaryOperationExpression e);

    public void Visit(UnaryOperationExpression e);

    public void Visit(NumericLiteralExpression e);

    public void Visit(StringLiteralExpression e);

    public void Visit(BooleanLiteralExpression b);

    public void Visit(VariableDeclaration e);

    public void Visit(VariableDeclarationStatement e);

    public void Visit(AssignmentExpression e);

    public void Visit(VariableExpression e);

    public void Visit(FunctionCallExpression f);

    public void Visit(ConstantDeclaration d);

    public void Visit(WriteStatement s);

    public void Visit(WriteLineStatement s);

    public void Visit(ReadStatement s);

    public void Visit(ReadLineStatement s);

    public void Visit(IfElseStatement s);

    public void Visit(CompoundStatement s);

    public void Visit(ForLoopStatement s);

    public void Visit(WhileLoopStatement s);

    public void Visit(DoWhileLoopStatement s);

    public void Visit(FunctionDeclaration d);

    public void Visit(ReturnStatement s);

    public void Visit(BreakStatement s);

    public void Visit(ContinueStatement s);
}