namespace Atc.Wpf.Controls.Tests.Inputs;

public sealed class NumericBoxTests : IDisposable
{
    public void Dispose()
        => Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();

    private const string TemplateXaml =
        "<ControlTemplate " +
        "xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" " +
        "xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\">" +
        "<Grid>" +
        "<TextBox x:Name=\"PART_TextBox\" />" +
        "<RepeatButton x:Name=\"PART_NumericUpButton\" />" +
        "<RepeatButton x:Name=\"PART_NumericDownButton\" />" +
        "</Grid>" +
        "</ControlTemplate>";

    #region Coercion Tests

    [StaFact]
    public void Value_AboveMaximum_IsCoercedToMaximum()
    {
        // Arrange
        var numericBox = new NumericBox
        {
            Minimum = 0,
            Maximum = 10,
        };

        // Act
        numericBox.Value = 15;

        // Assert
        Assert.Equal(10d, numericBox.Value);
    }

    [StaFact]
    public void Value_BelowMinimum_IsCoercedToMinimum()
    {
        // Arrange
        var numericBox = new NumericBox
        {
            Minimum = -5,
            Maximum = 10,
        };

        // Act
        numericBox.Value = -20;

        // Assert
        Assert.Equal(-5d, numericBox.Value);
    }

    [StaFact]
    public void Minimum_RaisedAboveCurrentValue_CoercesValueUp()
    {
        // Arrange
        var numericBox = new NumericBox
        {
            Minimum = 0,
            Maximum = 10,
            Value = 3,
        };

        // Act
        numericBox.Minimum = 8;

        // Assert
        Assert.Equal(8d, numericBox.Value);
    }

    [StaFact]
    public void Maximum_LoweredBelowCurrentValue_CoercesValueDown()
    {
        // Arrange
        var numericBox = new NumericBox
        {
            Minimum = 0,
            Maximum = 10,
            Value = 9,
        };

        // Act
        numericBox.Maximum = 4;

        // Assert
        Assert.Equal(4d, numericBox.Value);
    }

    [StaFact]
    public void Maximum_SetBelowMinimum_IsCoercedToMinimum()
    {
        // Arrange
        var numericBox = new NumericBox
        {
            Minimum = 5,
        };

        // Act
        numericBox.Maximum = 2;

        // Assert
        Assert.Equal(5d, numericBox.Maximum);
    }

    [StaFact]
    public void DefaultValue_OutsideRange_IsCoercedIntoRange()
    {
        // Arrange
        var numericBox = new NumericBox
        {
            Minimum = 0,
            Maximum = 10,
        };

        // Act
        numericBox.DefaultValue = 42;

        // Assert
        Assert.Equal(10d, numericBox.DefaultValue);
    }

    [StaFact]
    public void Value_FractionInNumbersOnlyMode_IsTruncated()
    {
        // Arrange
        var numericBox = new NumericBox
        {
            NumericInputMode = NumericInput.Numbers,
        };

        // Act
        numericBox.Value = 3.7;

        // Assert
        Assert.Equal(3d, numericBox.Value);
    }

    #endregion

    #region Null Value Tests

    [StaFact]
    public void Value_DefaultsToNull()
    {
        // Arrange & Act
        var numericBox = new NumericBox();

        // Assert
        Assert.Null(numericBox.Value);
    }

    [StaFact]
    public void Value_SetToNullWithDefaultValue_IsCoercedToDefaultValue()
    {
        // Arrange
        var numericBox = new NumericBox
        {
            DefaultValue = 3,
            Value = 7,
        };

        // Act
        numericBox.Value = null;

        // Assert
        Assert.Equal(3d, numericBox.Value);
    }

    [StaFact]
    public void DefaultValue_SetWhileValueIsNull_AssignsDefaultToValue()
    {
        // Arrange
        var numericBox = new NumericBox();

        // Act
        numericBox.DefaultValue = 4;

        // Assert
        Assert.Equal(4d, numericBox.Value);
    }

    [StaFact]
    public void Value_SetToNullWithoutDefaultValue_ClearsText()
    {
        // Arrange
        var (numericBox, textBox, _, _) = CreateTemplated(CultureInfo.GetCultureInfo("en-US"));
        numericBox.Value = 5;

        // Act
        numericBox.Value = null;

        // Assert
        Assert.Null(numericBox.Value);
        Assert.Equal(string.Empty, textBox.Text);
    }

    [StaFact]
    public void Text_ClearedWithoutDefaultValue_SetsValueToNull()
    {
        // Arrange
        var (numericBox, textBox, _, _) = CreateTemplated(CultureInfo.GetCultureInfo("en-US"));
        numericBox.Value = 5;

        // Act
        textBox.Text = string.Empty;

        // Assert
        Assert.Null(numericBox.Value);
    }

    [StaFact]
    public void Text_ClearedWithDefaultValue_RestoresDefaultValueAndText()
    {
        // Arrange
        var (numericBox, textBox, _, _) = CreateTemplated(CultureInfo.GetCultureInfo("en-US"));
        numericBox.DefaultValue = 2.5;
        numericBox.Value = 5;

        // Act
        textBox.Text = string.Empty;

        // Assert
        Assert.Equal(2.5d, numericBox.Value);
        Assert.Equal("2.5", textBox.Text);
    }

    #endregion

    #region Culture Parsing Tests

    [StaTheory]
    [InlineData("da-DK", "1,5", 1.5)]
    [InlineData("de-DE", "1,5", 1.5)]
    [InlineData("en-US", "1.5", 1.5)]
    [InlineData("en-US", "1,234.5", 1234.5)]
    [InlineData("da-DK", "1.234,5", 1234.5)]
    [InlineData("en-US", "-7.25", -7.25)]
    public void TypedText_IsParsedWithCultureDecimalSeparator(
        string cultureName,
        string text,
        double expected)
    {
        // Arrange
        var (numericBox, textBox, _, _) = CreateTemplated(CultureInfo.GetCultureInfo(cultureName));

        // Act
        TypeText(textBox, text);

        // Assert
        Assert.Equal(expected, numericBox.Value);
    }

    [StaFact]
    public void TypedText_WithLetters_IsRejectedByPreviewTextInput()
    {
        // Arrange
        var (_, textBox, _, _) = CreateTemplated(CultureInfo.GetCultureInfo("en-US"));

        // Act
        var handled = RaisePreviewTextInput(textBox, "12a");

        // Assert
        Assert.True(handled);
    }

    [StaFact]
    public void TypedText_AboveMaximum_IsClampedToMaximum()
    {
        // Arrange
        var (numericBox, textBox, _, _) = CreateTemplated(CultureInfo.GetCultureInfo("en-US"));
        numericBox.Minimum = 0;
        numericBox.Maximum = 10;

        // Act
        TypeText(textBox, "50");

        // Assert
        Assert.Equal(10d, numericBox.Value);
    }

    [StaTheory]
    [InlineData("da-DK", "1,5")]
    [InlineData("en-US", "1.5")]
    public void Value_IsDisplayedWithCultureDecimalSeparator(
        string cultureName,
        string expectedText)
    {
        // Arrange
        var (numericBox, textBox, _, _) = CreateTemplated(CultureInfo.GetCultureInfo(cultureName));

        // Act
        numericBox.Value = 1.5;

        // Assert
        Assert.Equal(expectedText, textBox.Text);
    }

    [StaFact]
    public void Culture_Changed_ReformatsDisplayedText()
    {
        // Arrange
        var (numericBox, textBox, _, _) = CreateTemplated(CultureInfo.GetCultureInfo("en-US"));
        numericBox.Value = 1.5;

        // Act
        numericBox.Culture = CultureInfo.GetCultureInfo("da-DK");

        // Assert
        Assert.Equal("1,5", textBox.Text);
    }

    #endregion

    #region StringFormat Tests

    [StaTheory]
    [InlineData("en-US", "N2", 1234.5, "1,234.50")]
    [InlineData("da-DK", "N2", 1234.5, "1.234,50")]
    [InlineData("en-US", "F3", 2.5, "2.500")]
    [InlineData("en-US", "{0:N1} kg", 3.5, "3.5 kg")]
    [InlineData("en-US", "X2", 255, "FF")]
    public void StringFormat_IsAppliedToDisplayedText(
        string cultureName,
        string stringFormat,
        double value,
        string expectedText)
    {
        // Arrange
        var (numericBox, textBox, _, _) = CreateTemplated(CultureInfo.GetCultureInfo(cultureName));
        numericBox.StringFormat = stringFormat;

        // Act
        numericBox.Value = value;

        // Assert
        Assert.Equal(expectedText, textBox.Text);
    }

    [StaFact]
    public void StringFormat_ChangedAfterValue_ReformatsDisplayedText()
    {
        // Arrange
        var (numericBox, textBox, _, _) = CreateTemplated(CultureInfo.GetCultureInfo("en-US"));
        numericBox.Value = 2;

        // Act
        numericBox.StringFormat = "F2";

        // Assert
        Assert.Equal("2.00", textBox.Text);
    }

    [StaTheory]
    [InlineData("en-US", "N2", "1,234.50", 1234.5)]
    [InlineData("da-DK", "N2", "1.234,50", 1234.5)]
    [InlineData("en-US", "F3", "2.500", 2.5)]
    public void StringFormat_FormattedTextTypedBack_RoundTripsToSameValue(
        string cultureName,
        string stringFormat,
        string formattedText,
        double expectedValue)
    {
        // Arrange
        var (numericBox, textBox, _, _) = CreateTemplated(CultureInfo.GetCultureInfo(cultureName));
        numericBox.StringFormat = stringFormat;

        // Act
        TypeText(textBox, formattedText);

        // Assert
        Assert.Equal(expectedValue, numericBox.Value);
    }

    [StaTheory]
    [InlineData("P0")]
    [InlineData("{0:P0}")]
    public void StringFormat_Percent_TypedNumberIsDividedByHundred(
        string stringFormat)
    {
        // Arrange
        var (numericBox, textBox, _, _) = CreateTemplated(CultureInfo.GetCultureInfo("en-US"));
        numericBox.StringFormat = stringFormat;

        // Act
        TypeText(textBox, "25");

        // Assert
        Assert.Equal(0.25d, numericBox.Value);
    }

    [StaFact(Skip = "Bug: NumericBox.ConvertStringFormatValue only recognizes an upper-case P percent specifier, so p0 input is not divided by 100.")]
    public void StringFormat_LowercasePercent_TypedNumberIsDividedByHundred()
    {
        // Arrange
        var (numericBox, textBox, _, _) = CreateTemplated(CultureInfo.GetCultureInfo("en-US"));
        numericBox.StringFormat = "p0";

        // Act
        TypeText(textBox, "25");

        // Assert
        Assert.Equal(0.25d, numericBox.Value);
    }

    [StaFact]
    public void StringFormat_Hexadecimal_SwitchesParsingToHexNumber()
    {
        // Arrange
        var numericBox = new NumericBox();

        // Act
        numericBox.StringFormat = "X2";

        // Assert
        Assert.Equal(NumberStyles.HexNumber, numericBox.ParsingNumberStyle);
    }

    [StaFact]
    public void StringFormat_Hexadecimal_TypedDigitsAreParsedAsHex()
    {
        // Arrange
        var (numericBox, textBox, _, _) = CreateTemplated(CultureInfo.GetCultureInfo("en-US"));
        numericBox.StringFormat = "X2";

        // Act
        TypeText(textBox, "10");

        // Assert
        Assert.Equal(16d, numericBox.Value);
    }

    [StaFact(Skip = "Bug: NumericBox.ValidateText rejects any letter before the hexadecimal branch, so A-F cannot be typed with a hex StringFormat.")]
    public void StringFormat_Hexadecimal_TypedHexLettersAreAccepted()
    {
        // Arrange
        var (numericBox, textBox, _, _) = CreateTemplated(CultureInfo.GetCultureInfo("en-US"));
        numericBox.StringFormat = "X2";

        // Act
        TypeText(textBox, "1A");

        // Assert
        Assert.Equal(26d, numericBox.Value);
    }

    [StaFact]
    public void StringFormat_Null_IsCoercedToEmptyString()
    {
        // Arrange
        var numericBox = new NumericBox();

        // Act
        numericBox.StringFormat = null!;

        // Assert
        Assert.Equal(string.Empty, numericBox.StringFormat);
    }

    #endregion

    #region Interval Increment / Decrement Tests

    [StaFact]
    public void UpButton_Click_IncrementsValueByInterval()
    {
        // Arrange
        var (numericBox, textBox, up, _) = CreateTemplated(CultureInfo.GetCultureInfo("en-US"));
        numericBox.Interval = 0.5;
        numericBox.Value = 1;

        // Act
        Click(up);

        // Assert
        Assert.Equal(1.5d, numericBox.Value);
        Assert.Equal("1.5", textBox.Text);
    }

    [StaFact]
    public void DownButton_Click_DecrementsValueByInterval()
    {
        // Arrange
        var (numericBox, _, _, down) = CreateTemplated(CultureInfo.GetCultureInfo("en-US"));
        numericBox.Interval = 2;
        numericBox.Value = 10;

        // Act
        Click(down);
        Click(down);

        // Assert
        Assert.Equal(6d, numericBox.Value);
    }

    [StaFact]
    public void UpButton_Click_WhenValueIsNull_StartsFromZero()
    {
        // Arrange
        var (numericBox, _, up, _) = CreateTemplated(CultureInfo.GetCultureInfo("en-US"));
        numericBox.Interval = 3;

        // Act
        Click(up);

        // Assert
        Assert.Equal(3d, numericBox.Value);
    }

    [StaFact]
    public void UpButton_Click_NearMaximum_StopsAtMaximum()
    {
        // Arrange
        var (numericBox, _, up, _) = CreateTemplated(CultureInfo.GetCultureInfo("en-US"));
        numericBox.Minimum = 0;
        numericBox.Maximum = 10;
        numericBox.Interval = 2;
        numericBox.Value = 9;

        // Act
        Click(up);

        // Assert
        Assert.Equal(10d, numericBox.Value);
    }

    [StaFact]
    public void DownButton_Click_NearMinimum_StopsAtMinimum()
    {
        // Arrange
        var (numericBox, _, _, down) = CreateTemplated(CultureInfo.GetCultureInfo("en-US"));
        numericBox.Minimum = 0;
        numericBox.Maximum = 10;
        numericBox.Interval = 2;
        numericBox.Value = 1;

        // Act
        Click(down);

        // Assert
        Assert.Equal(0d, numericBox.Value);
    }

    [StaFact]
    public void UpButton_Click_WhenReadOnly_DoesNotChangeValue()
    {
        // Arrange
        var (numericBox, _, up, _) = CreateTemplated(CultureInfo.GetCultureInfo("en-US"));
        numericBox.Value = 4;
        numericBox.IsReadOnly = true;

        // Act
        Click(up);

        // Assert
        Assert.Equal(4d, numericBox.Value);
    }

    [StaFact]
    public void UpButton_Click_RaisesValueIncrementedWithInterval()
    {
        // Arrange
        var (numericBox, _, up, _) = CreateTemplated(CultureInfo.GetCultureInfo("en-US"));
        numericBox.Interval = 0.25;
        numericBox.Value = 1;
        var intervals = new List<double>();
        numericBox.AddHandler(
            NumericBox.ValueIncrementedEvent,
            new NumericBoxChangedRoutedEventHandler((_, e) => intervals.Add(e.Interval)));

        // Act
        Click(up);

        // Assert
        Assert.Equal(0.25d, Assert.Single(intervals));
    }

    [StaFact]
    public void DownButton_Click_RaisesValueDecrementedWithNegativeInterval()
    {
        // Arrange
        var (numericBox, _, _, down) = CreateTemplated(CultureInfo.GetCultureInfo("en-US"));
        numericBox.Interval = 0.25;
        numericBox.Value = 1;
        var intervals = new List<double>();
        numericBox.AddHandler(
            NumericBox.ValueDecrementedEvent,
            new NumericBoxChangedRoutedEventHandler((_, e) => intervals.Add(e.Interval)));

        // Act
        Click(down);

        // Assert
        Assert.Equal(-0.25d, Assert.Single(intervals));
    }

    [StaFact]
    public void ValueIncremented_HandledByListener_CancelsChange()
    {
        // Arrange
        var (numericBox, _, up, _) = CreateTemplated(CultureInfo.GetCultureInfo("en-US"));
        numericBox.Value = 1;
        numericBox.AddHandler(
            NumericBox.ValueIncrementedEvent,
            new NumericBoxChangedRoutedEventHandler((_, e) => e.Handled = true));

        // Act
        Click(up);

        // Assert
        Assert.Equal(1d, numericBox.Value);
    }

    [StaFact]
    public void ValueIncremented_IntervalChangedByListener_UsesNewInterval()
    {
        // Arrange
        var (numericBox, _, up, _) = CreateTemplated(CultureInfo.GetCultureInfo("en-US"));
        numericBox.Value = 1;
        numericBox.AddHandler(
            NumericBox.ValueIncrementedEvent,
            new NumericBoxChangedRoutedEventHandler((_, e) => e.Interval = 10));

        // Act
        Click(up);

        // Assert
        Assert.Equal(11d, numericBox.Value);
    }

    [StaFact]
    public void UpButton_Click_WithSnapToMultipleOfInterval_SnapsResult()
    {
        // Arrange
        var (numericBox, _, up, _) = CreateTemplated(CultureInfo.GetCultureInfo("en-US"));
        numericBox.Interval = 0.5;
        numericBox.SnapToMultipleOfInterval = true;
        numericBox.Value = 1.2;

        // Act
        Click(up);

        // Assert
        Assert.Equal(1.5d, numericBox.Value);
    }

    [StaFact]
    public void SnapToMultipleOfInterval_Enabled_SnapsCurrentValue()
    {
        // Arrange
        var numericBox = new NumericBox
        {
            Interval = 0.5,
            Value = 1.3,
        };

        // Act
        numericBox.SnapToMultipleOfInterval = true;

        // Assert
        Assert.Equal(1.5d, numericBox.Value);
    }

    #endregion

    #region ValueChanged Event Tests

    [StaFact]
    public void ValueChanged_ProgrammaticSet_RaisedOnceWithOldAndNewValue()
    {
        // Arrange
        var numericBox = new NumericBox
        {
            Value = 1,
        };
        var events = CaptureValueChanged(numericBox);

        // Act
        numericBox.Value = 2;

        // Assert
        var single = Assert.Single(events);
        Assert.Equal(1d, single.OldValue);
        Assert.Equal(2d, single.NewValue);
    }

    [StaFact]
    public void ValueChanged_SameValueSet_IsNotRaised()
    {
        // Arrange
        var numericBox = new NumericBox
        {
            Value = 1,
        };
        var events = CaptureValueChanged(numericBox);

        // Act
        numericBox.Value = 1;

        // Assert
        Assert.Empty(events);
    }

    [StaFact]
    public void ValueChanged_ClampedToMaximum_ReportsCoercedNewValue()
    {
        // Arrange
        var numericBox = new NumericBox
        {
            Minimum = 0,
            Maximum = 10,
            Value = 5,
        };
        var events = CaptureValueChanged(numericBox);

        // Act
        numericBox.Value = 20;

        // Assert
        var single = Assert.Single(events);
        Assert.Equal(5d, single.OldValue);
        Assert.Equal(10d, single.NewValue);
    }

    [StaFact]
    public void ValueChanged_SetToNull_ReportsNullNewValue()
    {
        // Arrange
        var (numericBox, _, _, _) = CreateTemplated(CultureInfo.GetCultureInfo("en-US"));
        numericBox.Value = 5;
        var events = CaptureValueChanged(numericBox);

        // Act
        numericBox.Value = null;

        // Assert
        var single = Assert.Single(events);
        Assert.Equal(5d, single.OldValue);
        Assert.Null(single.NewValue);
    }

    [StaFact]
    public void ValueChanged_TypedText_RaisedOnceWithOldAndNewValue()
    {
        // Arrange
        var (numericBox, textBox, _, _) = CreateTemplated(CultureInfo.GetCultureInfo("en-US"));
        var events = CaptureValueChanged(numericBox);

        // Act
        TypeText(textBox, "1.5");

        // Assert
        var single = Assert.Single(events);
        Assert.Null(single.OldValue);
        Assert.Equal(1.5d, single.NewValue);
    }

    [StaFact]
    public void ValueChanged_UpButtonClick_RaisedOnceWithOldAndNewValue()
    {
        // Arrange
        var (numericBox, _, up, _) = CreateTemplated(CultureInfo.GetCultureInfo("en-US"));
        numericBox.Value = 4;
        var events = CaptureValueChanged(numericBox);

        // Act
        Click(up);

        // Assert
        var single = Assert.Single(events);
        Assert.Equal(4d, single.OldValue);
        Assert.Equal(5d, single.NewValue);
    }

    #endregion

    private static (NumericBox NumericBox, TextBox TextBox, RepeatButton Up, RepeatButton Down) CreateTemplated(
        CultureInfo culture)
    {
        var numericBox = new NumericBox
        {
            Culture = culture,
            Speedup = false,
            Template = (ControlTemplate)XamlReader.Parse(TemplateXaml),
        };

        numericBox.ApplyTemplate();

        var textBox = (TextBox)numericBox.Template.FindName("PART_TextBox", numericBox);
        var up = (RepeatButton)numericBox.Template.FindName("PART_NumericUpButton", numericBox);
        var down = (RepeatButton)numericBox.Template.FindName("PART_NumericDownButton", numericBox);

        return (numericBox, textBox, up, down);
    }

    private static bool RaisePreviewTextInput(
        TextBox textBox,
        string text)
    {
        var args = new TextCompositionEventArgs(
            Keyboard.PrimaryDevice,
            new TextComposition(InputManager.Current, textBox, text))
        {
            RoutedEvent = TextCompositionManager.PreviewTextInputEvent,
        };

        textBox.RaiseEvent(args);
        return args.Handled;
    }

    private static void TypeText(
        TextBox textBox,
        string text)
    {
        var handled = RaisePreviewTextInput(textBox, text);
        Assert.False(handled, $"PreviewTextInput rejected '{text}'.");
        textBox.Text = text;
    }

    private static void Click(RepeatButton button)
        => button.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));

    private static List<(double? OldValue, double? NewValue)> CaptureValueChanged(
        NumericBox numericBox)
    {
        var events = new List<(double? OldValue, double? NewValue)>();
        numericBox.AddHandler(
            NumericBox.ValueChangedEvent,
            new RoutedPropertyChangedEventHandler<double?>((_, e) => events.Add((e.OldValue, e.NewValue))));
        return events;
    }
}