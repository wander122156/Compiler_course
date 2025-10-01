using System.Text;

namespace ExampleLib;

public static class FormatRomanUtil
{
    public static string FormatRoman(int value)
    {
        if (value < 1 || value > 3000)
        {
            throw new ArgumentOutOfRangeException(nameof(value));
        }

        StringBuilder result = new ();
        int remaining = value;
        while (remaining > 0)
        {
            switch (remaining)
            {
                case >= 1000:
                    result.Append('M');
                    remaining -= 1000;
                    break;
                case >= 900:
                    result.Append("CM");
                    remaining -= 900;
                    break;
                case >= 500:
                    result.Append('D');
                    remaining -= 500;
                    break;
                case >= 400:
                    result.Append("CD");
                    remaining -= 400;
                    break;
                case >= 100:
                    result.Append('C');
                    remaining -= 100;
                    break;
                case >= 90:
                    result.Append("XC");
                    remaining -= 90;
                    break;
                case >= 50:
                    result.Append('L');
                    remaining -= 50;
                    break;
                case >= 40:
                    result.Append("XL");
                    remaining -= 40;
                    break;
                case >= 10:
                    result.Append('X');
                    remaining -= 10;
                    break;
                case >= 9:
                    result.Append("IX");
                    remaining -= 9;
                    break;
                case >= 5:
                    result.Append('V');
                    remaining -= 5;
                    break;
                case >= 4:
                    result.Append("IV");
                    remaining -= 4;
                    break;
                case >= 1:
                    result.Append('I');
                    remaining--;
                    break;
            }
        }

        return result.ToString();
    }
}