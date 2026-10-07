namespace Atc.Wpf.Forms.Tests.Controls;

public sealed class LabelTextBoxTests
{
    [StaFact]
    public void IsValid_MandatoryAndEmpty_ReportsFieldIsRequired()
    {
        UseEnglishUi();
        var sut = new LabelTextBox { IsMandatory = true };

        Assert.False(sut.IsValid());
        Assert.Equal("Field is required", sut.ValidationText);
    }

    [StaFact]
    public void IsValid_OptionalAndEmpty_IsValid()
    {
        UseEnglishUi();
        var sut = new LabelTextBox { IsMandatory = false };

        Assert.True(sut.IsValid());
        Assert.Equal(string.Empty, sut.ValidationText);
    }

    [StaFact]
    public void Text_ShorterThanMinLength_ReportsTheMinimum()
    {
        UseEnglishUi();
        var sut = new LabelTextBox
        {
            MinLength = 3,
            TriggerOnlyOnLostFocus = false,
        };

        sut.Text = "ab";

        Assert.Equal("Min. length: 3", sut.ValidationText);
        Assert.False(sut.IsValid());
    }

    [StaFact]
    public void Text_LongerThanMaxLength_ReportsTheMaximum()
    {
        UseEnglishUi();
        var sut = new LabelTextBox
        {
            MaxLength = 5,
            TriggerOnlyOnLostFocus = false,
        };

        sut.Text = "abcdef";

        Assert.Equal("Max. length: 5", sut.ValidationText);
    }

    [StaFact]
    public void Text_WithNotAllowedCharacters_ListsOnlyTheCharactersUsed()
    {
        UseEnglishUi();
        var sut = new LabelTextBox
        {
            CharactersNotAllowed = "#!$",
            TriggerOnlyOnLostFocus = false,
        };

        sut.Text = "a#b!c";

        Assert.Equal("Not allowed: # !", sut.ValidationText);
    }

    [StaFact]
    public void Text_NotMatchingRegexPattern_ReportsTheMismatch()
    {
        UseEnglishUi();
        var sut = new LabelTextBox
        {
            RegexPattern = "^[0-9]+$",
            TriggerOnlyOnLostFocus = false,
        };

        sut.Text = "12a";

        Assert.Equal("Regular expression don't match", sut.ValidationText);
    }

    [StaFact]
    public void Text_BecomingValidAgain_ClearsTheMessage()
    {
        UseEnglishUi();
        var sut = new LabelTextBox
        {
            MinLength = 3,
            TriggerOnlyOnLostFocus = false,
        };
        sut.Text = "ab";

        sut.Text = "abc";

        Assert.Equal(string.Empty, sut.ValidationText);
    }

    [StaFact]
    public void TriggerOnlyOnLostFocus_TypingInvalidText_DefersTheMessageUntilValidated()
    {
        UseEnglishUi();
        var sut = new LabelTextBox
        {
            MinLength = 3,
            TriggerOnlyOnLostFocus = true,
        };

        sut.Text = "ab";
        var whileTyping = sut.ValidationText;
        var isValid = sut.IsValid();

        Assert.Equal(string.Empty, whileTyping);
        Assert.False(isValid);
        Assert.Equal("Min. length: 3", sut.ValidationText);
    }

    [StaFact]
    public void Text_Changed_RaisesTextChangedWithOldAndNewValue()
    {
        UseEnglishUi();
        var sut = new LabelTextBox { Text = "old" };
        RoutedPropertyChangedEventArgs<string>? raised = null;
        sut.AddHandler(
            LabelTextBox.TextChangedEvent,
            new RoutedPropertyChangedEventHandler<string>((_, e) => raised = e));

        sut.Text = "new";

        Assert.NotNull(raised);
        Assert.Equal("old", raised.OldValue);
        Assert.Equal("new", raised.NewValue);
    }

    private static void UseEnglishUi()
        => Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");
}