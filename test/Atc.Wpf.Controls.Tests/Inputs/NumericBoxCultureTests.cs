namespace Atc.Wpf.Controls.Tests.Inputs;

// Changing the UI culture is process-wide, so these tests don't run alongside others.
[Collection(nameof(UiCultureTestGroup))]
public sealed class NumericBoxCultureTests : IDisposable
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

    private static readonly CultureInfo EnUs = CultureInfo.GetCultureInfo("en-US");
    private static readonly CultureInfo DaDk = CultureInfo.GetCultureInfo("da-DK");
    private static readonly CultureInfo DeDe = CultureInfo.GetCultureInfo("de-DE");

    private readonly CultureInfo originalUiCulture = Atc.Wpf.Translation.CultureManager.UiCulture;
    private readonly CultureInfo originalCulture = Thread.CurrentThread.CurrentCulture;

    public NumericBoxCultureTests()
        => Atc.Wpf.Translation.CultureManager.UiCulture = EnUs;

    public void Dispose()
    {
        Atc.Wpf.Translation.CultureManager.UiCulture = originalUiCulture;
        Thread.CurrentThread.CurrentCulture = originalCulture;
        Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();
    }

    [StaFact]
    public void UiCultureChanged_LocalCulture_IsKeptAndStillParses()
    {
        // Arrange
        var (numericBox, textBox) = CreateTemplated();
        numericBox.Culture = EnUs;

        // Act
        Atc.Wpf.Translation.CultureManager.UiCulture = DaDk;
        textBox.Text = string.Empty;
        TypeText(textBox, "1.5");

        // Assert
        Assert.Same(EnUs, numericBox.Culture);
        Assert.Equal(1.5d, numericBox.Value);
    }

    [StaFact]
    public void UiCultureChanged_BoundCulture_KeepsTheBinding()
    {
        // Arrange
        var numericBox = new NumericBox();
        BindingOperations.SetBinding(
            numericBox,
            NumericBox.CultureProperty,
            new Binding { Source = EnUs });

        // Act
        Atc.Wpf.Translation.CultureManager.UiCulture = DaDk;

        // Assert
        Assert.NotNull(BindingOperations.GetBindingExpression(numericBox, NumericBox.CultureProperty));
        Assert.Same(EnUs, numericBox.Culture);
    }

    [StaFact]
    public void UiCultureChanged_StyledCulture_IsKept()
    {
        // Arrange
        var style = new Style(typeof(NumericBox));
        style.Setters.Add(new Setter(NumericBox.CultureProperty, EnUs));
        var numericBox = new NumericBox { Style = style };

        // Act
        Atc.Wpf.Translation.CultureManager.UiCulture = DaDk;

        // Assert
        Assert.Same(EnUs, numericBox.Culture);
    }

    [StaFact]
    public void UiCultureChanged_CultureNotSet_FollowsEverySwitch()
    {
        // Arrange
        var (numericBox, textBox) = CreateTemplated();
        numericBox.Value = 1.5;

        // Act
        Atc.Wpf.Translation.CultureManager.UiCulture = DaDk;
        var afterFirst = numericBox.Culture;
        var textAfterFirst = textBox.Text;
        Atc.Wpf.Translation.CultureManager.UiCulture = DeDe;

        // Assert
        Assert.Equal(DaDk, afterFirst);
        Assert.Equal("1,5", textAfterFirst);
        Assert.Equal(DeDe, numericBox.Culture);
        Assert.Equal(
            BaseValueSource.Default,
            DependencyPropertyHelper.GetValueSource(numericBox, NumericBox.CultureProperty).BaseValueSource);
    }

    [StaTheory]
    [InlineData(typeof(DecimalBox))]
    [InlineData(typeof(CurrencyBox))]
    [InlineData(typeof(IntegerBox))]
    public void TypedBox_StartsInUiCultureAndFollowsSwitch(Type boxType)
    {
        // Arrange
        Atc.Wpf.Translation.CultureManager.UiCulture = DaDk;
        var numericBox = (NumericBox)Activator.CreateInstance(boxType)!;
        var initial = numericBox.Culture;

        // Act
        Atc.Wpf.Translation.CultureManager.UiCulture = DeDe;

        // Assert
        Assert.Equal(DaDk, initial);
        Assert.Equal(DeDe, numericBox.Culture);
    }

    [StaFact]
    public void TypedBox_LocalCulture_BeatsTheSeededUiCulture()
    {
        // Arrange
        Atc.Wpf.Translation.CultureManager.UiCulture = DaDk;
        var currencyBox = new CurrencyBox { Culture = EnUs };

        // Act
        Atc.Wpf.Translation.CultureManager.UiCulture = DeDe;

        // Assert
        Assert.Same(EnUs, currencyBox.Culture);
    }

    [StaFact]
    public void TypedBox_StyledCulture_BeatsTheSeededUiCulture()
    {
        // Arrange
        Atc.Wpf.Translation.CultureManager.UiCulture = DaDk;
        var style = new Style(typeof(CurrencyBox));
        style.Setters.Add(new Setter(NumericBox.CultureProperty, EnUs));
        var currencyBox = new CurrencyBox { Style = style };

        // Act
        Atc.Wpf.Translation.CultureManager.UiCulture = DeDe;

        // Assert
        Assert.Same(EnUs, currencyBox.Culture);
    }

    private static (NumericBox NumericBox, TextBox TextBox) CreateTemplated()
    {
        var numericBox = new NumericBox
        {
            Speedup = false,
            Template = (ControlTemplate)XamlReader.Parse(TemplateXaml),
        };

        numericBox.ApplyTemplate();
        var textBox = (TextBox)numericBox.Template.FindName("PART_TextBox", numericBox);
        return (numericBox, textBox);
    }

    private static void TypeText(
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
        Assert.False(args.Handled, $"PreviewTextInput rejected '{text}'.");
        textBox.Text = text;
    }
}