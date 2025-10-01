using System.Collections;

using Xunit;

namespace ExampleLib.UnitTests;

public class FormatRomanUtilTests
{
    public static TheoryData<int, string> SimpleValidDataTest()
    {
        return new TheoryData<int, string>
        {
            { 1, "I" },
            { 4, "IV" },
            { 5, "V" },
            { 9, "IX" },
            { 10, "X" },
            { 40, "XL" },
            { 50, "L" },
            { 90, "XC" },
            { 100, "C" },
            { 400, "CD" },
            { 500, "D" },
            { 900, "CM" },
            { 1000, "M" },
        };
    }

    public static TheoryData<int, string> BoundaryValidDataTest()
    {
        return new TheoryData<int, string>
        {
            { 1, "I" },
            { 2, "II" },
            { 3, "III" },
            { 6, "VI" },
            { 8, "VIII" },
            { 9, "IX" },
            { 14, "XIV" },
            { 19, "XIX" },
            { 49, "XLIX" },
            { 99, "XCIX" },
            { 1999, "MCMXCIX" },
            { 2023, "MMXXIII" },
            { 3000, "MMM" },
        };
    }

    public static TheoryData<int> InvalidInputDataTest()
    {
        return new TheoryData<int>
        {
            { -100 },
            { 0 },
            { 3001 },
            { 4000 },
        };
    }

    [Theory]
    [MemberData(nameof(SimpleValidDataTest))]
    public void CanFormatAllSimpleValidArabicIntoRoman(int arabic, string expectedRoman)
    {
        // Arrange
        // (данные подготовлены в MemberData)

        // Act
        string actual = FormatRomanUtil.FormatRoman(arabic);

        // Assert
        Assert.Equal(expectedRoman, actual);
    }

    [Theory]
    [MemberData(nameof(BoundaryValidDataTest))]
    public void CanFormatBoundaryValidArabicIntoRoman(int arabic, string expectedRoman)
    {
        string actual = FormatRomanUtil.FormatRoman(arabic);
        Assert.Equal(expectedRoman, actual);
    }

    [Theory]
    [MemberData(nameof(InvalidInputDataTest))]
    public void CanFormatAllInvalidArabicIntoRoman(int arabic)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => FormatRomanUtil.FormatRoman(arabic));
    }
}