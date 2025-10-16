namespace Lexer.UnitTests;

public class LexerTest
{
    [Theory]
    [MemberData(nameof(GetTokenizeData))]
    public void Can_tokenize(string code, List<Token> expected)
    {
        List<Token> actual = Tokenize(code);
        Assert.Equal(expected, actual);
    }

    public static TheoryData<string, List<Token>> GetTokenizeData()
    {
        return new TheoryData<string, List<Token>>
        {
            {
                """
                if (x == 1)  
                { 
                    x = x + 1; 
                }
                """, [
                    new Token(TokenType.If),
                    new Token(TokenType.OpenParenthesis),
                    new Token(TokenType.Identifier, new TokenValue("x")),
                    new Token(TokenType.LooseEquality),
                    new Token(TokenType.NumericLiteral, new TokenValue(1)),
                    new Token(TokenType.CloseParenthesis),

                    new Token(TokenType.OpenBraces),

                    new Token(TokenType.Identifier, new TokenValue("x")),
                    new Token(TokenType.Assignment),
                    new Token(TokenType.Identifier, new TokenValue("x")),
                    new Token(TokenType.PlusSign),
                    new Token(TokenType.NumericLiteral, new TokenValue(1)),
                    new Token(TokenType.Semicolon),

                    new Token(TokenType.CloseBraces),
                ]
            },
            {
                """
                if (x == 1) 
                { 
                    x = x + 1 
                } 
                else 
                { 
                    x = x + 10;
                }
                """, [
                    new Token(TokenType.If),
                    new Token(TokenType.OpenParenthesis),
                    new Token(TokenType.Identifier, new TokenValue("x")),
                    new Token(TokenType.LooseEquality),
                    new Token(TokenType.NumericLiteral, new TokenValue(1)),
                    new Token(TokenType.CloseParenthesis),

                    new Token(TokenType.OpenBraces),

                    new Token(TokenType.Identifier, new TokenValue("x")),
                    new Token(TokenType.Assignment),
                    new Token(TokenType.Identifier, new TokenValue("x")),
                    new Token(TokenType.PlusSign),
                    new Token(TokenType.NumericLiteral, new TokenValue(1)),

                    new Token(TokenType.CloseBraces),

                    new Token(TokenType.Else),

                    new Token(TokenType.OpenBraces),

                    new Token(TokenType.Identifier, new TokenValue("x")),
                    new Token(TokenType.Assignment),
                    new Token(TokenType.Identifier, new TokenValue("x")),
                    new Token(TokenType.PlusSign),
                    new Token(TokenType.NumericLiteral, new TokenValue(10)),
                    new Token(TokenType.Semicolon),

                    new Token(TokenType.CloseBraces),
                ]
            },
            {
                """
                if (x == 1) 
                { 
                    x = x + 1 
                } 
                else if (x == 10) 
                { 
                    x = x + 10 
                }
                """, [
                    new Token(TokenType.If),
                    new Token(TokenType.OpenParenthesis),
                    new Token(TokenType.Identifier, new TokenValue("x")),
                    new Token(TokenType.LooseEquality),
                    new Token(TokenType.NumericLiteral, new TokenValue(1)),
                    new Token(TokenType.CloseParenthesis),

                    new Token(TokenType.OpenBraces),

                    new Token(TokenType.Identifier, new TokenValue("x")),
                    new Token(TokenType.Assignment),
                    new Token(TokenType.Identifier, new TokenValue("x")),
                    new Token(TokenType.PlusSign),
                    new Token(TokenType.NumericLiteral, new TokenValue(1)),

                    new Token(TokenType.CloseBraces),

                    new Token(TokenType.Else),
                    new Token(TokenType.If),
                    new Token(TokenType.OpenParenthesis),
                    new Token(TokenType.Identifier, new TokenValue("x")),
                    new Token(TokenType.LooseEquality),
                    new Token(TokenType.NumericLiteral, new TokenValue(10)),
                    new Token(TokenType.CloseParenthesis),

                    new Token(TokenType.OpenBraces),

                    new Token(TokenType.Identifier, new TokenValue("x")),
                    new Token(TokenType.Assignment),
                    new Token(TokenType.Identifier, new TokenValue("x")),
                    new Token(TokenType.PlusSign),
                    new Token(TokenType.NumericLiteral, new TokenValue(10)),

                    new Token(TokenType.CloseBraces),
                ]
            },
            {
                """
                while (x < 10) do 
                { 
                    x = x + 1 
                }
                """, [
                    new Token(TokenType.While),
                    new Token(TokenType.OpenParenthesis),
                    new Token(TokenType.Identifier, new TokenValue("x")),
                    new Token(TokenType.LessThan),
                    new Token(TokenType.NumericLiteral, new TokenValue(10)),
                    new Token(TokenType.CloseParenthesis),
                    new Token(TokenType.Do),
                    new Token(TokenType.OpenBraces),
                    new Token(TokenType.Identifier, new TokenValue("x")),
                    new Token(TokenType.Assignment),
                    new Token(TokenType.Identifier, new TokenValue("x")),
                    new Token(TokenType.PlusSign),
                    new Token(TokenType.NumericLiteral, new TokenValue(1)),
                    new Token(TokenType.CloseBraces),
                ]
            },
            {
                """write ("hello world")""",
                [
                    new Token(TokenType.Write),
                    new Token(TokenType.OpenParenthesis),
                    new Token(TokenType.StringLiteral, new TokenValue("hello world")),
                    new Token(TokenType.CloseParenthesis),
                ]
            },
            {
                """writeln ("hello world")""",
                [
                    new Token(TokenType.Writeln),
                    new Token(TokenType.OpenParenthesis),
                    new Token(TokenType.StringLiteral, new TokenValue("hello world")),
                    new Token(TokenType.CloseParenthesis),
                ]
            },
            {
                "read (a)",
                [
                    new Token(TokenType.Read),
                    new Token(TokenType.OpenParenthesis),
                    new Token(TokenType.Identifier, new TokenValue("a")),
                    new Token(TokenType.CloseParenthesis),
                ]
            },
            {
                "if (x = 1 and y = 2)",
                [
                    new Token(TokenType.If),
                    new Token(TokenType.OpenParenthesis),
                    new Token(TokenType.Identifier, new TokenValue("x")),
                    new Token(TokenType.Assignment),
                    new Token(TokenType.NumericLiteral, new TokenValue(1)),
                    new Token(TokenType.And),
                    new Token(TokenType.Identifier, new TokenValue("y")),
                    new Token(TokenType.Assignment),
                    new Token(TokenType.NumericLiteral, new TokenValue(2)),
                    new Token(TokenType.CloseParenthesis),
                ]
            },
            {
                "If (x = 3 oR y = 4)",
                [
                    new Token(TokenType.If),
                    new Token(TokenType.OpenParenthesis),
                    new Token(TokenType.Identifier, new TokenValue("x")),
                    new Token(TokenType.Assignment),
                    new Token(TokenType.NumericLiteral, new TokenValue(3)),
                    new Token(TokenType.Or),
                    new Token(TokenType.Identifier, new TokenValue("y")),
                    new Token(TokenType.Assignment),
                    new Token(TokenType.NumericLiteral, new TokenValue(4)),
                    new Token(TokenType.CloseParenthesis),
                ]
            },

            // Операторы
            {
                "2 + 2",
                [
                    new Token(TokenType.NumericLiteral, new TokenValue(2)),
                    new Token(TokenType.PlusSign),
                    new Token(TokenType.NumericLiteral, new TokenValue(2)),
                ]
            },
            {
                "2 - 2",
                [
                    new Token(TokenType.NumericLiteral, new TokenValue(2)),
                    new Token(TokenType.MinusSign),
                    new Token(TokenType.NumericLiteral, new TokenValue(2)),
                ]
            },
            {
                "2 * 2",
                [
                    new Token(TokenType.NumericLiteral, new TokenValue(2)),
                    new Token(TokenType.MultiplySign),
                    new Token(TokenType.NumericLiteral, new TokenValue(2)),
                ]
            },
            {
                "2 / 2",
                [
                    new Token(TokenType.NumericLiteral, new TokenValue(2)),
                    new Token(TokenType.DivideSign),
                    new Token(TokenType.NumericLiteral, new TokenValue(2)),
                ]
            },
            {
                "2 % 2",
                [
                    new Token(TokenType.NumericLiteral, new TokenValue(2)),
                    new Token(TokenType.ModuloSign),
                    new Token(TokenType.NumericLiteral, new TokenValue(2)),
                ]
            },
            {
                "2 ^ 2",
                [
                    new Token(TokenType.NumericLiteral, new TokenValue(2)),
                    new Token(TokenType.ExponentiationSign),
                    new Token(TokenType.NumericLiteral, new TokenValue(2)),
                ]
            },
            {
                "2 = 2",
                [
                    new Token(TokenType.NumericLiteral, new TokenValue(2)),
                    new Token(TokenType.Assignment),
                    new Token(TokenType.NumericLiteral, new TokenValue(2)),
                ]
            },
            {
                "2 != 2",
                [
                    new Token(TokenType.NumericLiteral, new TokenValue(2)),
                    new Token(TokenType.NotEqual),
                    new Token(TokenType.NumericLiteral, new TokenValue(2)),
                ]
            },
            {
                "3 > 2",
                [
                    new Token(TokenType.NumericLiteral, new TokenValue(3)),
                    new Token(TokenType.GreaterThan),
                    new Token(TokenType.NumericLiteral, new TokenValue(2)),
                ]
            },
            {
                "2 < 3",
                [
                    new Token(TokenType.NumericLiteral, new TokenValue(2)),
                    new Token(TokenType.LessThan),
                    new Token(TokenType.NumericLiteral, new TokenValue(3)),
                ]
            },
            {
                "2 >= 2",
                [
                    new Token(TokenType.NumericLiteral, new TokenValue(2)),
                    new Token(TokenType.GreaterThanOrEqual),
                    new Token(TokenType.NumericLiteral, new TokenValue(2)),
                ]
            },
            {
                "2 <= 2",
                [
                    new Token(TokenType.NumericLiteral, new TokenValue(2)),
                    new Token(TokenType.LessThanOrEqual),
                    new Token(TokenType.NumericLiteral, new TokenValue(2)),
                ]
            },

            // Комментарии
            {
                """
                $Hello World!
                """,
                []
            },
            {
                """
                $$Multi-line!
                This is second string$$
                """,
                []
            },

            // Строковые литералы
            {
                """
                x = "he%^&*()asd"
                """,
                [
                    new Token(TokenType.Identifier, new TokenValue("x")),
                    new Token(TokenType.Assignment),
                    new Token(TokenType.StringLiteral, new TokenValue("he%^&*()asd")),
                ]
            },
            {
                """x = "hello\world" """,
                [
                    new Token(TokenType.Identifier, new TokenValue("x")),
                    new Token(TokenType.Assignment),
                    new Token(TokenType.StringLiteral, new TokenValue("hello\\world")),
                ]
            },
            {
                """x = "" """,
                [
                    new Token(TokenType.Identifier, new TokenValue("x")),
                    new Token(TokenType.Assignment),
                    new Token(TokenType.StringLiteral, new TokenValue("")),
                ]
            },
        };
    }

    private List<Token> Tokenize(string code)
    {
        List<Token> results = [];
        Lexer lexer = new(code);

        for (Token t = lexer.ParseToken(); t.Type != TokenType.EndOfFile; t = lexer.ParseToken())
        {
            results.Add(t);
        }

        return results;
    }
}