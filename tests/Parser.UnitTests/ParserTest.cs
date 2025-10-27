namespace Parser.UnitTests;

public class ParserTest
{

    [Fact]
    public void Can_parse_If_without_Else()
    {
        Row result = Parser.ExecuteCode("if (true) {}");
        Assert.Equal(1, result.ColumnCount);
        Assert.Equal(RuntimeValue.ValueType.Boolean, result[0].Type);
        Assert.Equal(true, result[0].Value);


        // result пустой
    }

    [Fact]
    public void Can_parse_If_true_condition_body()
    {
        Row result = Parser.ExecuteCode("if (true) { 1 + 5 - 3}");
        Assert.Equal(1, result.ColumnCount);
        Assert.Equal(RuntimeValue.ValueType.Boolean, result[0].Type);
        Assert.Equal(true, result[0].Value);
    }

    [Fact]
    public void Can_parse_If_false_condition_body()
    {
        Row result = Parser.ExecuteCode("if (false) { write(\"hello\") }");
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