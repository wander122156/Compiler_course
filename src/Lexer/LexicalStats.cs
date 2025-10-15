using System.Text;

namespace Lexer;
public struct Stats
{
    public int Keywords;
    public int Identifiers;
    public int NumberLiterals;
    public int StringLiterals;
    public int Operators;
    public int OtherLexemes;
}

public static class LexicalStats
{
    public static Stats CountLexicalStats(string code)
    {
        Stats results = new();
        Lexer lexer = new(code);
        for (Token t = lexer.ParseToken(); t.Type != TokenType.EndOfFile; t = lexer.ParseToken())
        {
            switch (t.Type)
            {
                // Ключевые слова
                case TokenType.While:
                case TokenType.Do:
                case TokenType.Write:
                case TokenType.Writeln:
                case TokenType.Read:
                case TokenType.If:
                case TokenType.Else:
                case TokenType.And:
                case TokenType.Or:
                    results.Keywords++;
                    break;

                // Идентификаторы и литералы
                case TokenType.Identifier:
                    results.Identifiers++;
                    break;
                case TokenType.NumericLiteral:
                    results.NumberLiterals++;
                    break;
                case TokenType.StringLiteral:
                    results.StringLiterals++;
                    break;

                // Операторы
                case TokenType.Assignment:
                case TokenType.LooseEquality:
                case TokenType.NotEqual:
                case TokenType.Not:
                case TokenType.PlusSign:
                case TokenType.MinusSign:
                case TokenType.MultiplySign:
                case TokenType.DivideSign:
                case TokenType.ModuloSign:
                case TokenType.ExponentiationSign:
                case TokenType.LessThan:
                case TokenType.LessThanOrEqual:
                case TokenType.GreaterThan:
                case TokenType.GreaterThanOrEqual:
                    results.Operators++;
                    break;

                // Специальные токены
                case TokenType.EndOfFile:
                case TokenType.Error:
                    break;

                default:
                    results.OtherLexemes++;
                    break;
            }
        }

        return results;
    }

    public static string CollectFromFile(string path)
    {
        string code = File.ReadAllText(path, Encoding.UTF8);

        Stats stats = new();
        stats = CountLexicalStats(code);

        string statsInfo = $"""
            Keywords: {stats.Keywords}
            Identifiers: {stats.Identifiers}
            Number literals: {stats.NumberLiterals}
            String literals: {stats.StringLiterals}
            Operators: {stats.Operators}
            Other lexemes: {stats.OtherLexemes}
            """;

        return statsInfo;
    }
}
