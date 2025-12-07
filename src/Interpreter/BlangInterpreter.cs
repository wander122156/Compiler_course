using Blang.Ast.Statement;
using Blang.Common;
using Blang.Execution;
using Blang.Semantic;

namespace Blang.Interpreter;
public class BlangInterpreter
{
    private readonly IEnvironment _environment;

    public BlangInterpreter(IEnvironment environment)
    {
        _environment = environment;
    }

    /// <summary>
    /// Выполняет программу на языке Blang
    /// </summary>
    public void Execute(string sourceCode)
    {
        if (string.IsNullOrEmpty(sourceCode))
        {
            throw new ArgumentException("Source code cannot be null or empty", nameof(sourceCode));
        }

        TypeContext typeContext = new();
        Context context = new();

        Parser.Parser parser = new(sourceCode);
        List<Ast.IAstElement> program = parser.ParseProgram();

        SemanticChecker semanticChecker = new();
        semanticChecker.Check(program);

        AstEvaluator evaluator = new(_environment);
        evaluator.Evaluate(program);
    }
}
