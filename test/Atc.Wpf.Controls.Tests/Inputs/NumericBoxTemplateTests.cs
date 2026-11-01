namespace Atc.Wpf.Controls.Tests.Inputs;

[Collection(nameof(PackResourceTestGroup))]
public sealed class NumericBoxTemplateTests : IDisposable
{
    public NumericBoxTemplateTests()
    {
        // pack://application URIs need the Application type initialized.
        RuntimeHelpers.RunClassConstructor(typeof(Application).TypeHandle);
    }

    public void Dispose()
        => Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();

    [StaFact]
    public void ReadOnly_KeepsTheTextBoxEnabledButReadOnly()
    {
        // Arrange
        var numericBox = Realize(new NumericBox { Value = 5 });

        // Act
        numericBox.IsReadOnly = true;
        numericBox.Value = 6;

        // Assert
        var textBox = (TextBox)numericBox.Template.FindName("PART_TextBox", numericBox);
        var up = (RepeatButton)numericBox.Template.FindName("PART_NumericUpButton", numericBox);
        var down = (RepeatButton)numericBox.Template.FindName("PART_NumericDownButton", numericBox);
        Assert.True(textBox.IsEnabled);
        Assert.True(textBox.IsReadOnly);
        Assert.False(up.IsEnabled);
        Assert.False(down.IsEnabled);
    }

    [StaTheory]
    [InlineData("PART_NumericUpButton", "PART_NumericUpButtonIcon")]
    [InlineData("PART_NumericDownButton", "PART_NumericDownButtonIcon")]
    public void PressedSpinButton_RecoloursItsOwnIcon(
        string buttonName,
        string expectedIconName)
    {
        // Arrange
        var numericBox = Realize(new NumericBox());

        // Act
        var setters = numericBox.Template.Triggers
            .OfType<Trigger>()
            .Where(t => t.SourceName == buttonName && t.Property == ButtonBase.IsPressedProperty)
            .SelectMany(t => t.Setters.OfType<Setter>())
            .ToList();

        // Assert
        Assert.NotEmpty(setters);
        Assert.All(setters, s => Assert.Equal(expectedIconName, s.TargetName));
    }

    private static NumericBox Realize(NumericBox numericBox)
    {
        var dictionary = new ResourceDictionary
        {
            Source = new Uri("pack://application:,,,/Atc.Wpf.Controls;component/Inputs/NumericBox.xaml", UriKind.Absolute),
        };
        numericBox.Style = (Style)dictionary["AtcApps.Styles.NumericBox"];
        numericBox.ApplyTemplate();
        numericBox.Measure(new Size(200, 30));
        numericBox.Arrange(new Rect(0, 0, 200, 30));
        numericBox.UpdateLayout();
        return numericBox;
    }
}