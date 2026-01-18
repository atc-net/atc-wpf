// ReSharper disable InvertIf
// ReSharper disable ConvertIfStatementToSwitchStatement
namespace Atc.Wpf.Forms.Internal.Helpers;

internal static class DateTimeTextBoxHelper
{
    [SuppressMessage("Globalization", "CA1305:Specify IFormatProvider", Justification = "OK.")]
    public static string GetSelectedDateAsText(
        DateTime dateTime,
        DatePickerFormat datePickerFormat,
        CultureInfo cultureInfo)
    {
        ArgumentNullException.ThrowIfNull(cultureInfo);

        if (datePickerFormat == DatePickerFormat.Short ||
            (datePickerFormat == DatePickerFormat.Long &&
             Thread.CurrentThread.CurrentUICulture.LCID == GlobalizationConstants.EnglishCultureInfo.LCID))
        {
            return dateTime.ToString(
                datePickerFormat == DatePickerFormat.Short
                    ? cultureInfo.DateTimeFormat.ShortDatePattern
                    : cultureInfo.DateTimeFormat.LongDatePattern);
        }

        var s = dateTime.ToString(cultureInfo.DateTimeFormat.LongDatePattern);

        if (s.StartsWith(nameof(DayOfWeek.Sunday), StringComparison.Ordinal))
        {
            s = s.Replace(nameof(DayOfWeek.Sunday), DayOfWeekHelper.GetDescription(DayOfWeek.Sunday, cultureInfo), StringComparison.Ordinal);
        }
        else if (s.StartsWith(nameof(DayOfWeek.Monday), StringComparison.Ordinal))
        {
            s = s.Replace(nameof(DayOfWeek.Monday), DayOfWeekHelper.GetDescription(DayOfWeek.Monday, cultureInfo), StringComparison.Ordinal);
        }
        else if (s.StartsWith(nameof(DayOfWeek.Tuesday), StringComparison.Ordinal))
        {
            s = s.Replace(nameof(DayOfWeek.Tuesday), DayOfWeekHelper.GetDescription(DayOfWeek.Tuesday, cultureInfo), StringComparison.Ordinal);
        }
        else if (s.StartsWith(nameof(DayOfWeek.Wednesday), StringComparison.Ordinal))
        {
            s = s.Replace(nameof(DayOfWeek.Wednesday), DayOfWeekHelper.GetDescription(DayOfWeek.Wednesday, cultureInfo), StringComparison.Ordinal);
        }
        else if (s.StartsWith(nameof(DayOfWeek.Thursday), StringComparison.Ordinal))
        {
            s = s.Replace(nameof(DayOfWeek.Thursday), DayOfWeekHelper.GetDescription(DayOfWeek.Thursday, cultureInfo), StringComparison.Ordinal);
        }
        else if (s.StartsWith(nameof(DayOfWeek.Friday), StringComparison.Ordinal))
        {
            s = s.Replace(nameof(DayOfWeek.Friday), DayOfWeekHelper.GetDescription(DayOfWeek.Friday, cultureInfo), StringComparison.Ordinal);
        }
        else if (s.StartsWith(nameof(DayOfWeek.Saturday), StringComparison.Ordinal))
        {
            s = s.Replace(nameof(DayOfWeek.Saturday), DayOfWeekHelper.GetDescription(DayOfWeek.Saturday, cultureInfo), StringComparison.Ordinal);
        }

        return s;
    }

    public static bool TryParseUsingSpecificCulture(
        DatePickerFormat datePickerFormat,
        string value,
        CultureInfo cultureInfo,
        out DateTime result)
    {
        result = DateTime.MinValue;
        switch (datePickerFormat)
        {
            case DatePickerFormat.Long:
                if (!string.IsNullOrWhiteSpace(value) &&
                    DateTime.TryParse(
                        value,
                        cultureInfo.DateTimeFormat,
                        DateTimeStyles.None,
                        out var resLong))
                {
                    result = resLong;
                    return true;
                }

                break;
            case DatePickerFormat.Short:
                if (DateTimeHelper.TryParseShortDateUsingSpecificCulture(
                        value,
                        cultureInfo,
                        out var resShort))
                {
                    result = resShort;
                    return true;
                }

                break;
            default:
                throw new SwitchExpressionException(datePickerFormat);
        }

        return false;
    }

    public static bool TryParseUsingSpecificCulture(
        string dateValue,
        string timeValue,
        CultureInfo cultureInfo,
        out DateTime result)
    {
        result = DateTime.MinValue;
        if (!string.IsNullOrWhiteSpace(dateValue) &&
            !string.IsNullOrWhiteSpace(timeValue) &&
            DateTime.TryParse(
                $"{dateValue} {timeValue}",
                cultureInfo.DateTimeFormat,
                DateTimeStyles.None,
                out var resShort))
        {
            result = resShort;
            return true;
        }

        return false;
    }

    public static bool HandlePrerequisiteForOnTextTimeChanged(
        TextBox control,
        CultureInfo cultureInfo,
        TextChangedEventArgs e)
    {
        var textChange = e.Changes.First();

        var use24Hours = !(cultureInfo.DateTimeFormat.ShortTimePattern.StartsWith("h:", StringComparison.Ordinal) ||
                           cultureInfo.DateTimeFormat.ShortTimePattern.StartsWith("h.", StringComparison.Ordinal));

        if (textChange.AddedLength == 1)
        {
            var lastChar = control.Text[^1..];
            if (lastChar.IsDigitOnly())
            {
                if (!HandlePrerequisiteForOnTextTimeChangedAddedOneDigit(control, use24Hours))
                {
                    return false;
                }
            }
            else
            {
                if (!HandlePrerequisiteForOnTextTimeChangedAddedOneLetter(control, cultureInfo, lastChar, use24Hours))
                {
                    return false;
                }
            }
        }

        return true;
    }

    private static bool HandlePrerequisiteForOnTextTimeChangedAddedOneDigit(
        TextBox control,
        bool use24Hours)
    {
        var firstChar = control.Text[..1];

        if (!use24Hours && control.Text.Length == 2 && !firstChar.Equals("1", StringComparison.Ordinal))
        {
            control.Text = control.Text[..^1];
            control.CaretIndex = control.Text.Length;
            return false;
        }

        if (control.Text.Length == 2 ||
            (!use24Hours &&
             !firstChar.Equals("1", StringComparison.Ordinal) &&
             !control.Text.Contains(':', StringComparison.Ordinal)))
        {
            control.Text += ":";
            control.CaretIndex = control.Text.Length;
        }
        else if (control.Text.Length > 2)
        {
            var sa = control.Text.Split(':');
            if (sa.Length == 2)
            {
                if (sa[0].IsDigitOnly() &&
                    !sa[1].IsDigitOnly())
                {
                    control.Text = control.Text[..^1];
                    control.CaretIndex = control.Text.Length;
                    return false;
                }

                if (sa[0].IsDigitOnly() &&
                    sa[1].IsDigitOnly())
                {
                    var minutes = NumberHelper.ParseToInt(sa[1]);
                    if (minutes > 60)
                    {
                        control.Text = control.Text[..^1];
                        control.CaretIndex = control.Text.Length;
                        return false;
                    }

                    if (!use24Hours && minutes > 5)
                    {
                        control.Text += " ";
                        control.CaretIndex = control.Text.Length;
                        return false;
                    }
                }
                else
                {
                    control.Text += " ";
                    control.CaretIndex = control.Text.Length;
                    return false;
                }
            }
        }

        return true;
    }

    private static bool HandlePrerequisiteForOnTextTimeChangedAddedOneLetter(
        TextBox control,
        CultureInfo cultureInfo,
        string lastChar,
        bool use24Hours)
    {
        var isHandled = false;
        if (lastChar.Equals(":", StringComparison.Ordinal) &&
            (control.Text.Length == 1 ||
             !control.Text[..^1].IsDigitOnly()))
        {
            control.Text = control.Text[..^1];
            control.CaretIndex = control.Text.Length;
            return false;
        }

        if (!use24Hours &&
            !HandlePrerequisiteForOnTextTimeChangedAddedOneLetterAmPmPart(control, cultureInfo, lastChar, ref isHandled))
        {
            return false;
        }

        if (!isHandled)
        {
            control.Text = control.Text[..^1];
            control.CaretIndex = control.Text.Length;
            return false;
        }

        return true;
    }

    private static bool HandlePrerequisiteForOnTextTimeChangedAddedOneLetterAmPmPart(
        TextBox control,
        CultureInfo cultureInfo,
        string lastChar,
        ref bool isHandled)
    {
        if (lastChar.Equals(" ", StringComparison.Ordinal))
        {
            var sa = control.Text.Split(':');
            if (sa.Length != 2 ||
                !sa[0].IsDigitOnly() ||
                !sa[1].TrimEnd().IsDigitOnly())
            {
                control.Text = control.Text[..^1];
                control.CaretIndex = control.Text.Length;
                return false;
            }

            isHandled = true;
        }

        if (lastChar.Equals("a", StringComparison.OrdinalIgnoreCase) ||
            lastChar.Equals("p", StringComparison.OrdinalIgnoreCase))
        {
            control.Text = control.Text.ToUpper(cultureInfo);
            var sa = control.Text.Split(' ');
            if (sa.Length != 2 || sa[1].Length != 1)
            {
                control.Text = control.Text[..^1];
                control.CaretIndex = control.Text.Length;
                return false;
            }

            control.Text += "M";
            control.CaretIndex = control.Text.Length;
            isHandled = true;
        }
        else if (lastChar.Equals("m", StringComparison.OrdinalIgnoreCase))
        {
            control.Text = control.Text.ToUpper(cultureInfo);
            var sa = control.Text.Split(' ');
            if (sa.Length != 2 || sa[1].Length != 2)
            {
                control.Text = control.Text[..^1];
                control.CaretIndex = control.Text.Length;
                return false;
            }

            isHandled = true;
        }

        return true;
    }
}