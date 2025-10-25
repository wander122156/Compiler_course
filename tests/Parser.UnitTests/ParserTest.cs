namespace Parser.UnitTests;

public class ParserTest
{
    [Fact]
    public void Can_parse_Write_without_Semicolon()
    {
        string result = Parser.ExecuteCode("write (\"hello\")");
        Assert.Equal("hello", result);
    }

    [Fact]
    public void Can_parse_Write_with_Semicolon()
    {
        string result = Parser.ExecuteCode("write (\"hello\");");
        Assert.Equal("hello", result);
    }
}