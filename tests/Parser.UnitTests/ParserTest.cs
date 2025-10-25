namespace Parser.UnitTests;

public class ParserTest
{
    [Fact]
    public void Can_parse_Write_without_Semicolon()
    {
        Row result = Parser.ExecuteCode("write (\"hello\")");
        Assert.Equal(1, result.ColumnCount);
        Assert.Equal(RuntimeValue.ValueType.String, result[0].Type);
        Assert.Equal("hello", result[0].Value);
    }

    [Fact]
    public void Can_parse_Write_with_Semicolon()
    {
        Row result = Parser.ExecuteCode("write (\"hello\");");
        Assert.Equal(1, result.ColumnCount);
        Assert.Equal(RuntimeValue.ValueType.String, result[0].Type);
        Assert.Equal("hello", result[0].Value);
    }

    [Fact]
    public void Can_parse_Write_with_multiple_expressions()
    {
        Row result = Parser.ExecuteCode("write(\"hello\", \"asd\")");
        Assert.Equal(2, result.ColumnCount);
        Assert.Equal(RuntimeValue.ValueType.String, result[0].Type);
        Assert.Equal("hello", result[0].Value);
        Assert.Equal(RuntimeValue.ValueType.String, result[1].Type);
        Assert.Equal("asd", result[1].Value);
    }

    [Fact]
    public void Can_parse_If_without_Else()
    {
        Row result = Parser.ExecuteCode("if (True) {}");
        Assert.Equal(0, result.ColumnCount);

        // result пустой
    }

    [Fact]
    public void Can_parse_If_true_condition_body()
    {
        Row result = Parser.ExecuteCode("if (True) { write(\"hello\") }");
        Assert.Equal(1, result.ColumnCount);
        Assert.Equal(RuntimeValue.ValueType.String, result[0].Type);
        Assert.Equal("hello", result[0].Value);
    }

    [Fact]
    public void Can_parse_If_false_condition_body()
    {
        Row result = Parser.ExecuteCode("if (False) { write(\"hello\") }");
        Assert.Equal(0, result.ColumnCount);
    }

    [Fact]
    public void Can_parse_If_condition_with_multiple_expressions()
    {
        Row result = Parser.ExecuteCode("if (1 + 4 - 2 == 3) {}");
        Assert.Equal(0, result.ColumnCount);

        // result пустой
    }
}