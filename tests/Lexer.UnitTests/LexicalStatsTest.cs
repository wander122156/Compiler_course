using Lexer.UnitTests.Helpers;

namespace Lexer.UnitTests;
public class LexicalStatsTest
{
    [Theory]
    [MemberData(nameof(GetLexicalStats))]
    public void CanGetStats(string code, string expected)
    {
        using TempFile file = TempFile.Create(code);

        string actual = LexicalStats.CollectFromFile(file.Path);
        Assert.Equal(expected, actual);
    }

    public static TheoryData<string, string> GetLexicalStats()
    {
        return new TheoryData<string, string>
        {
            {
                """
                {
                  string worldName;

                  writeln("Enter your world name ");
                  read(worldName);

                  write("Hello ", worldName, "world.";

                }

                """,
                """
                Keywords: 3
                Identifiers: 4
                Number literals: 0
                String literals: 3
                Operators: 0
                Other lexemes: 13
                """
            },
            {
                """
                {
                    int num1, num2, sum;
                    writeln("Enter the first number: ");
                    read(num1);

                    writeln("Enter the second number: ");
                    read(num2);

                    sum = num1 + num2;

                    write("The sum of ", num1, " and ", num2, " is: ", sum);
                }

                """,
                """
                Keywords: 5
                Identifiers: 12
                Number literals: 0
                String literals: 5
                Operators: 2
                Other lexemes: 26
                """
            },
        };
    }
}