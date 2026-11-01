namespace Atc.Wpf.Controls.Tests.Inputs;

[Collection(nameof(UiCultureTestGroup))]
public sealed class NumericBoxLetteredFormatTests : IDisposable
{
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

    private readonly CultureInfo originalCulture = Thread.CurrentThread.CurrentCulture;
    private readonly CultureInfo originalUiCulture = Thread.CurrentThread.CurrentUICulture;

    public void Dispose()
    {
        Thread.CurrentThread.CurrentCulture = originalCulture;
        Thread.CurrentThread.CurrentUICulture = originalUiCulture;
        Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();
    }

    [StaTheory]
    [InlineData("da-DK", "C2")]
    [InlineData("sv-SE", "C2")]
    [InlineData("nb-NO", "C2")]
    [InlineData("pl-PL", "C2")]
    [InlineData("de-CH", "C2")]
    [InlineData("da-DK", "{0:N2} kg")]
    [InlineData("en-US", "{0:N2} kg")]
    public void Backspace_OnLastDigit_KeepsTheEditedValue(
        string cultureName,
        string stringFormat)
    {
        // Arrange
        var (numericBox, textBox) = CreateCurrencyLikeBox(cultureName, stringFormat);
        var lastDigit = LastDigitIndex(textBox.Text);

        // Act
        PressBackspace(textBox, lastDigit + 1);

        // Assert
        Assert.Equal(1234.2d, numericBox.Value);
    }

    [StaTheory]
    [InlineData("da-DK", "C2")]
    [InlineData("sv-SE", "C2")]
    [InlineData("nb-NO", "C2")]
    [InlineData("pl-PL", "C2")]
    [InlineData("de-CH", "C2")]
    [InlineData("da-DK", "{0:N2} kg")]
    [InlineData("en-US", "{0:N2} kg")]
    public void TypedDigit_InFormattedText_IsAccepted(
        string cultureName,
        string stringFormat)
    {
        // Arrange
        var (numericBox, textBox) = CreateCurrencyLikeBox(cultureName, stringFormat);
        var lastDigit = LastDigitIndex(textBox.Text);
        PressBackspace(textBox, lastDigit + 1);

        // Act
        textBox.CaretIndex = lastDigit;
        var handled = RaisePreviewTextInput(textBox, "7");
        if (!handled)
        {
            textBox.Text = textBox.Text.Insert(lastDigit, "7");
        }

        // Assert
        Assert.False(handled);
        Assert.Equal(1234.27d, numericBox.Value);
    }

    [StaTheory]
    [InlineData("da-DK")]
    [InlineData("en-US")]
    public void OneDecimalUnitFormat_BackspaceThenDigit_KeepsAndUpdatesTheValue(
        string cultureName)
    {
        // Arrange
        var (numericBox, textBox) = CreateCurrencyLikeBox(cultureName, "{0:N1} kg", 12.5);
        var lastDigit = LastDigitIndex(textBox.Text);

        // Act
        PressBackspace(textBox, lastDigit + 1);
        var afterBackspace = numericBox.Value;
        textBox.CaretIndex = lastDigit;
        var handled = RaisePreviewTextInput(textBox, "7");
        if (!handled)
        {
            textBox.Text = textBox.Text.Insert(lastDigit, "7");
        }

        // Assert
        Assert.True(afterBackspace is 12d or 12.5d, $"Value after Backspace was {afterBackspace}.");
        Assert.False(handled);
        Assert.Equal(12.7d, numericBox.Value);
    }

    [StaFact]
    public void PartlyDeletedSuffix_DoesNotResetTheValueToDefault()
    {
        // Arrange
        var (numericBox, textBox) = CreateCurrencyLikeBox("da-DK", "C2");
        var text = textBox.Text;

        // Act
        PressBackspace(textBox, text.Length);

        // Assert
        Assert.Equal(1234.25d, numericBox.Value);
        Assert.Equal(text[..^1], textBox.Text);
    }

    [StaFact]
    public void LetterNotInFormat_IsStillRejected()
    {
        // Arrange
        var (_, textBox) = CreateCurrencyLikeBox("da-DK", "C2");
        textBox.CaretIndex = 0;

        // Act
        var handled = RaisePreviewTextInput(textBox, "q");

        // Assert
        Assert.True(handled);
    }

    private static (NumericBox NumericBox, TextBox TextBox) CreateCurrencyLikeBox(
        string cultureName,
        string stringFormat,
        double value = 1234.25)
    {
        var culture = CultureInfo.GetCultureInfo(cultureName);
        Thread.CurrentThread.CurrentCulture = culture;
        Thread.CurrentThread.CurrentUICulture = culture;

        var numericBox = new NumericBox
        {
            Culture = culture,
            Speedup = false,
            DefaultValue = 0,
            NumericInputMode = NumericInput.Decimal,
            StringFormat = stringFormat,
            Template = (ControlTemplate)XamlReader.Parse(TemplateXaml),
        };

        numericBox.ApplyTemplate();
        numericBox.Value = value;

        var textBox = (TextBox)numericBox.Template.FindName("PART_TextBox", numericBox);
        Assert.Contains(culture.NumberFormat.NumberDecimalSeparator, textBox.Text, StringComparison.Ordinal);
        return (numericBox, textBox);
    }

    private static int LastDigitIndex(string text)
        => text.Length - 1 - text.Reverse().TakeWhile(c => !char.IsAsciiDigit(c)).Count();

    private static void PressBackspace(
        TextBox textBox,
        int caretIndex)
    {
        var args = new KeyEventArgs(Keyboard.PrimaryDevice, new TestPresentationSource(), 0, Key.Back)
        {
            RoutedEvent = Keyboard.PreviewKeyDownEvent,
        };

        textBox.RaiseEvent(args);
        textBox.Text = textBox.Text.Remove(caretIndex - 1, 1);
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

    private sealed class TestPresentationSource : PresentationSource
    {
        public override Visual? RootVisual { get; set; }

        public override bool IsDisposed => false;

        protected override CompositionTarget? GetCompositionTargetCore()
            => null;
    }
}