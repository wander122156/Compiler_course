using Blang.Common;
using Blang.Execution;

namespace Blang.Interpreter;
public class BlangInterpreter
{
    private readonly Context _context;
    private readonly IEnvironment _environment;

    public BlangInterpreter()
    {
        _context = new Context();
        _environment = new ConsoleEnvironment();
    }

    /// <summary>
    /// Выполняет программу на языке Kaleidoscope
    /// </summary>
    public void Execute(string sourceCode)
    {
        if (string.IsNullOrEmpty(sourceCode))
        {
            throw new ArgumentException("Source code cannot be null or empty", nameof(sourceCode));
        }

        // Создаем парсер и выполняем программу
        Parser.Parser parser = new(_context, _environment, sourceCode);
        parser.ParseProgram();
    }
}
