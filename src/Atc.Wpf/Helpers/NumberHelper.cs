namespace Atc.Wpf.Helpers;

public static class NumberHelper
{
    public static int ParseToInt(
        string value)
    {
        if (!value.IsDigitOnly())
        {
            return -1;
        }

        return int.Parse(value, NumberStyles.Any, GlobalizationConstants.EnglishCultureInfo);
    }

    public static bool TryParseToInt(
        string value,
        out int result)
    {
        result = 0;

        if (!value.IsDigitOnly())
        {
            return false;
        }

        if (int.TryParse(value, NumberStyles.Any, GlobalizationConstants.EnglishCultureInfo, out var res))
        {
            result = res;
        }

        return false;
    }
}