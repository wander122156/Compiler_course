using Xunit;

namespace ExampleLib.UnitTests;

public class FormatRomanUtilTests
{
    public static TheoryData<int, string> SimpleValidDataTest()
    {
        return new TheoryData<int, string>
        {
            { 1, "I" },
            { 5, "V" },
            { 10, "X" },
            { 50, "L" },
            { 100, "C" },
            { 500, "D" },
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
            { 4, "IV" },
            { 6, "VI" },
            { 8, "VIII" },
            { 9, "IX" },
            { 14, "XIV" },
            { 19, "XIX" },
            { 40, "XL" },
            { 49, "XLIX" },
            { 90, "XC" },
            { 99, "XCIX" },
            { 400, "CD" },
            { 900, "CM" },
            { 1999, "MCMXCIX" },
            { 2023, "MMXXIII" },
            { 2999, "MMCMXCIX" },
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
    public void CanFormatSimpleValidArabicIntoRoman(int arabic, string expectedRoman)
    {
        // Arrange

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
    public void CanFormatInvalidArabicIntoRoman(int arabic)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => FormatRomanUtil.FormatRoman(arabic));
    }
}