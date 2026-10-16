namespace Atc.Wpf.Tests.Helpers;

/// <summary>
/// Monitoring hooks up when the element is loaded (styles turn it on while the element is still being built), and
/// follows the text live from then on.
/// </summary>
public sealed class TextBoxHelperMonitoringTests
{
    [StaFact]
    public void TextBox_NotLoaded_WaitsForLoadedBeforeComputing()
    {
        var textBox = new TextBox { Text = "abc" };

        TextBoxHelper.SetIsMonitoring(textBox, value: true);

        Assert.False(TextBoxHelper.GetHasText(textBox));

        Load(textBox);

        Assert.True(TextBoxHelper.GetHasText(textBox));
        Assert.Equal(3, (int)textBox.GetValue(TextBoxHelper.TextLengthProperty));
    }

    [StaFact]
    public void TextBox_WithMonitoring_TracksHasTextAndTextLengthAsTextChanges()
    {
        var textBox = new TextBox { Text = "abc" };
        TextBoxHelper.SetIsMonitoring(textBox, value: true);
        Load(textBox);

        textBox.Text = "hello";
        Assert.True(TextBoxHelper.GetHasText(textBox));
        Assert.Equal(5, (int)textBox.GetValue(TextBoxHelper.TextLengthProperty));

        textBox.Text = string.Empty;
        Assert.False(TextBoxHelper.GetHasText(textBox));
        Assert.Equal(0, (int)textBox.GetValue(TextBoxHelper.TextLengthProperty));
    }

    [StaFact]
    public void TextBox_WhenMonitoringIsTurnedOff_StopsUpdating()
    {
        var textBox = new TextBox();
        TextBoxHelper.SetIsMonitoring(textBox, value: true);
        Load(textBox);
        textBox.Text = "typed";
        Assert.True(TextBoxHelper.GetHasText(textBox));

        TextBoxHelper.SetIsMonitoring(textBox, value: false);
        textBox.Text = string.Empty;

        // The last monitored state is kept.
        Assert.True(TextBoxHelper.GetHasText(textBox));
        Assert.Equal(5, (int)textBox.GetValue(TextBoxHelper.TextLengthProperty));
    }

    [StaFact]
    public void TextBox_MonitoringTurnedOffBeforeLoaded_NeverHooksUp()
    {
        var textBox = new TextBox { Text = "abc" };
        TextBoxHelper.SetIsMonitoring(textBox, value: true);
        TextBoxHelper.SetIsMonitoring(textBox, value: false);

        Load(textBox);
        textBox.Text = "hello";

        Assert.False(TextBoxHelper.GetHasText(textBox));
    }

    [StaFact]
    public void EditableComboBox_WithMonitoring_SetsHasTextWhenAnItemIsSelected()
    {
        var comboBox = new ComboBox { IsEditable = true };
        comboBox.Items.Add("first");
        comboBox.Items.Add("second");

        TextBoxHelper.SetIsMonitoring(comboBox, value: true);
        Load(comboBox);

        Assert.False(TextBoxHelper.GetHasText(comboBox));

        comboBox.SelectedIndex = 1;
        Assert.True(TextBoxHelper.GetHasText(comboBox));

        comboBox.SelectedIndex = -1;
        comboBox.Text = string.Empty;
        Assert.False(TextBoxHelper.GetHasText(comboBox));
    }

    [StaFact]
    public void ComboBox_WhenMonitoringIsTurnedOff_StopsUpdating()
    {
        var comboBox = new ComboBox();
        comboBox.Items.Add("first");
        TextBoxHelper.SetIsMonitoring(comboBox, value: true);
        Load(comboBox);
        comboBox.SelectedIndex = 0;
        Assert.True(TextBoxHelper.GetHasText(comboBox));

        TextBoxHelper.SetIsMonitoring(comboBox, value: false);
        comboBox.SelectedIndex = -1;

        Assert.True(TextBoxHelper.GetHasText(comboBox));
    }

    [StaFact]
    public void RichTextBox_WithMonitoring_TracksHasTextAsContentChanges()
    {
        var richTextBox = new RichTextBox();

        TextBoxHelper.SetIsMonitoring(richTextBox, value: true);
        Load(richTextBox);

        Assert.False(TextBoxHelper.GetHasText(richTextBox));

        richTextBox.Document.Blocks.Clear();
        richTextBox.Document.Blocks.Add(new Paragraph(new Run("content")));
        Assert.True(TextBoxHelper.GetHasText(richTextBox));
        Assert.Equal(7, (int)richTextBox.GetValue(TextBoxHelper.TextLengthProperty));
    }

    // The elements are not in a window, so Loaded is raised by hand, which is what the helper listens for.
    private static void Load(FrameworkElement element)
        => element.RaiseEvent(new RoutedEventArgs(FrameworkElement.LoadedEvent));
}