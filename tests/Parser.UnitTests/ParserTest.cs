namespace Parser.UnitTests;

public class ParserTest
{
    [Fact]
    public void Can_parse_arithmetic_with_priority()
    {
        Row result = Parser.ExecuteCode("1 + 2 * 3");
        Assert.Equal(1, result.ColumnCount);
        Assert.Equal(RuntimeValue.ValueType.Number, result[0].Type);
        Assert.Equal(7m, result[0].Value);
    }

    [Fact]
    public void Can_parse_operators_with_different_priority()
    {
        Row result = Parser.ExecuteCode("2 + 3 * 4 % 2 - 2.5");
        Assert.Equal(1, result.ColumnCount);
        Assert.Equal(RuntimeValue.ValueType.Number, result[0].Type);
        Assert.Equal(-0.5m, result[0].Value);
    }

    [Fact]
    public void Check_left_associativity_subtraction()
    {
        Row result = Parser.ExecuteCode("10 - 3 - 2");
        Assert.Equal(1, result.ColumnCount);
        Assert.Equal(RuntimeValue.ValueType.Number, result[0].Type);
        Assert.Equal(5m, result[0].Value);
    }

    [Fact]
    public void Can_parse_unary_plus_minus()
    {
        Row result = Parser.ExecuteCode("+5 + -4");
        Assert.Equal(1, result.ColumnCount);
        Assert.Equal(RuntimeValue.ValueType.Number, result[0].Type);
        Assert.Equal(1m, result[0].Value);
    }

    [Fact]
    public void Can_parse_multiple_unary_operators()
    {
        Row result = Parser.ExecuteCode("--5");
        Assert.Equal(1, result.ColumnCount);
        Assert.Equal(RuntimeValue.ValueType.Number, result[0].Type);
        Assert.Equal(5m, result[0].Value);
    }

    [Fact]
    public void Can_parse_unary_and_binary_operators()
    {
        Row result = Parser.ExecuteCode("-5 + 3 * -2");
        Assert.Equal(1, result.ColumnCount);
        Assert.Equal(RuntimeValue.ValueType.Number, result[0].Type);
        Assert.Equal(-11m, result[0].Value);
    }

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
        Row result = Parser.ExecuteCode("if (true) { 1 + 5 - 3 }");
        Assert.Equal(1, result.ColumnCount);
        Assert.Equal(RuntimeValue.ValueType.Boolean, result[0].Type);
        Assert.Equal(true, result[0].Value);
    }

    [Fact]
    public void Can_parse_If_false_condition_body()
    {
        Row result = Parser.ExecuteCode("if (false) { 1 + 5 - 3 }");
        Assert.Equal(1, result.ColumnCount);
        Assert.Equal(RuntimeValue.ValueType.Boolean, result[0].Type);
        Assert.Equal(false, result[0].Value);
    }

    [Fact]
    public void Can_parse_less_than_operator()
    {
        Row result = Parser.ExecuteCode("if (3 < 5) {}");
        Assert.Equal(1, result.ColumnCount);
        Assert.Equal(RuntimeValue.ValueType.Boolean, result[0].Type);
        Assert.Equal(true, result[0].Value);
    }

    [Fact]
    public void Can_parse_comparison_with_arithmetic()
    {
        Row result = Parser.ExecuteCode("if (1 + 2 < 5) {}");
        Assert.Equal(1, result.ColumnCount);
        Assert.Equal(RuntimeValue.ValueType.Boolean, result[0].Type);
        Assert.Equal(true, result[0].Value);
    }

    [Fact]
    public void Can_parse_all_priority_operators()
    {
        Row result = Parser.ExecuteCode("if (1 + 2 * 3 < 7) {}");
        Assert.Equal(1, result.ColumnCount);
        Assert.Equal(RuntimeValue.ValueType.Boolean, result[0].Type);
        Assert.Equal(false, result[0].Value);
    }

    [Fact]
    public void Check_left_associativity_comparisons()
    {
        Row result = Parser.ExecuteCode("if (1 < 2 < 3) {}");
        Assert.Equal(1, result.ColumnCount);
        Assert.Equal(RuntimeValue.ValueType.Boolean, result[0].Type);
        Assert.Equal(true, result[0].Value);
    }

    [Fact]
    public void Can_parse_single_parentheses()
    {
        Row result = Parser.ExecuteCode("(1 + 2) * 3");
        Assert.Equal(1, result.ColumnCount);
        Assert.Equal(RuntimeValue.ValueType.Number, result[0].Type);
        Assert.Equal(4m, result[0].Value);
    }

    [Fact]
    public void Can_parse_multiple_parentheses()
    {
        Row result = Parser.ExecuteCode("((1 + 2) * (3 - 1)) - 2");
        Assert.Equal(1, result.ColumnCount);
        Assert.Equal(RuntimeValue.ValueType.Number, result[0].Type);
        Assert.Equal(4m, result[0].Value);
    }

    [Fact]
    public void Can_parse_BuiltinConstant_Pi()
    {
        Row result = Parser.ExecuteCode("Pi");
        Assert.Equal(1, result.ColumnCount);
        Assert.Equal(RuntimeValue.ValueType.Number, result[0].Type);
        Assert.Equal((decimal)Math.PI, result[0].Value);
    }

    [Fact]
    public void Can_parse_BuiltinConstant_MathE()
    {
        Row result = Parser.ExecuteCode("MathE * 1");
        Assert.Equal(1, result.ColumnCount);
        Assert.Equal(RuntimeValue.ValueType.Number, result[0].Type);
        Assert.Equal((decimal)Math.E, result[0].Value);
    }

    [Fact]
    public void Can_parse_BuiltinFunction_min()
    {
        Row result = Parser.ExecuteCode("min(7, 10 - 4, 8)");
        Assert.Equal(1, result.ColumnCount);
        Assert.Equal(RuntimeValue.ValueType.Number, result[0].Type);
        Assert.Equal(6m, result[0].Value);
    }

    [Fact]
    public void Can_parse_BuiltinFunction_max()
    {
        Row result = Parser.ExecuteCode("max(1, 3)");
        Assert.Equal(1, result.ColumnCount);
        Assert.Equal(RuntimeValue.ValueType.Number, result[0].Type);
        Assert.Equal(3m, result[0].Value);
    }

    [Fact]
    public void Can_parse_BuiltinFunction_abs()
    {
        Row result = Parser.ExecuteCode("abs(-4)");
        Assert.Equal(1, result.ColumnCount);
        Assert.Equal(RuntimeValue.ValueType.Number, result[0].Type);
        Assert.Equal(4m, result[0].Value);
    }

    [Fact]
    public void Can_parse_BuiltinFunction_pow()
    {
        Row result = Parser.ExecuteCode("pow(5, 3)");
        Assert.Equal(1, result.ColumnCount);
        Assert.Equal(RuntimeValue.ValueType.Number, result[0].Type);
        Assert.Equal(125m, result[0].Value);
    }
}