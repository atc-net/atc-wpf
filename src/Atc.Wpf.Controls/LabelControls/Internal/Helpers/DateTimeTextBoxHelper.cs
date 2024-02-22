// ReSharper disable InvertIf
// ReSharper disable ConvertIfStatementToSwitchStatement
namespace Atc.Wpf.Controls.LabelControls.Internal.Helpers;

internal static class DateTimeTextBoxHelper
{
    public static bool TryParseUsingCurrentUiCulture(
        DatePickerFormat datePickerFormat,
        string value,
        out DateTime result)
    {
        result = DateTime.MinValue;
        switch (datePickerFormat)
        {
            case DatePickerFormat.Long:
                if (!string.IsNullOrWhiteSpace(value) &&
                    DateTime.TryParse(
                        value,
                        Thread.CurrentThread.CurrentUICulture.DateTimeFormat,
                        DateTimeStyles.None,
                        out var resLong))
                {
                    result = resLong;
                    return true;
                }

                break;
            case DatePickerFormat.Short:
                if (DateTimeHelper.TryParseShortDateUsingCurrentUiCulture(
                        value,
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

    public static bool TryParseUsingCurrentUiCulture(
        string dateValue,
        string timeValue,
        out DateTime result)
    {
        result = DateTime.MinValue;
        if (!string.IsNullOrWhiteSpace(dateValue) &&
            !string.IsNullOrWhiteSpace(timeValue) &&
            DateTime.TryParse(
                $"{dateValue} {timeValue}",
                Thread.CurrentThread.CurrentUICulture.DateTimeFormat,
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
        TextChangedEventArgs e)
    {
        var textChange = e.Changes.First();

        var use24Hours = !(Thread.CurrentThread.CurrentUICulture.DateTimeFormat.ShortTimePattern.StartsWith("h:", StringComparison.Ordinal) ||
                           Thread.CurrentThread.CurrentUICulture.DateTimeFormat.ShortTimePattern.StartsWith("h.", StringComparison.Ordinal));

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
                if (!HandlePrerequisiteForOnTextTimeChangedAddedOneLetter(control, lastChar, use24Hours))
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
            !HandlePrerequisiteForOnTextTimeChangedAddedOneLetterAmPmPart(control, lastChar, ref isHandled))
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
            control.Text = control.Text.ToUpper(Thread.CurrentThread.CurrentUICulture);
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
            control.Text = control.Text.ToUpper(Thread.CurrentThread.CurrentUICulture);
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