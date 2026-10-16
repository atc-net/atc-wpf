namespace Atc.Wpf.Tests.Helpers;

public sealed class TextBoxHelperMonitoringTests
{
    [StaFact]
    public void TextBox_WithMonitoring_TracksHasTextAndTextLengthAsTextChanges()
    {
        // Arrange
        var textBox = new TextBox { Text = "abc" };

        // Act
        TextBoxHelper.SetIsMonitoring(textBox, value: true);

        // Assert - computed immediately, without waiting for Loaded
        Assert.True(TextBoxHelper.GetHasText(textBox));
        Assert.Equal(3, (int)textBox.GetValue(TextBoxHelper.TextLengthProperty));

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
        // Arrange
        var textBox = new TextBox();
        TextBoxHelper.SetIsMonitoring(textBox, value: true);
        textBox.Text = "typed";
        Assert.True(TextBoxHelper.GetHasText(textBox));

        // Act
        TextBoxHelper.SetIsMonitoring(textBox, value: false);
        textBox.Text = string.Empty;

        // Assert - the last monitored state is kept
        Assert.True(TextBoxHelper.GetHasText(textBox));
        Assert.Equal(5, (int)textBox.GetValue(TextBoxHelper.TextLengthProperty));
    }

    [StaFact]
    public void EditableComboBox_WithMonitoring_SetsHasTextWhenAnItemIsSelected()
    {
        // Arrange
        var comboBox = new ComboBox { IsEditable = true };
        comboBox.Items.Add("first");
        comboBox.Items.Add("second");

        // Act
        TextBoxHelper.SetIsMonitoring(comboBox, value: true);

        // Assert
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
        // Arrange
        var comboBox = new ComboBox();
        comboBox.Items.Add("first");
        TextBoxHelper.SetIsMonitoring(comboBox, value: true);
        comboBox.SelectedIndex = 0;
        Assert.True(TextBoxHelper.GetHasText(comboBox));

        // Act
        TextBoxHelper.SetIsMonitoring(comboBox, value: false);
        comboBox.SelectedIndex = -1;

        // Assert
        Assert.True(TextBoxHelper.GetHasText(comboBox));
    }

    [StaFact]
    public void RichTextBox_WithMonitoring_TracksHasTextAsContentChanges()
    {
        // Arrange
        var richTextBox = new RichTextBox();

        // Act
        TextBoxHelper.SetIsMonitoring(richTextBox, value: true);

        // Assert
        Assert.False(TextBoxHelper.GetHasText(richTextBox));

        richTextBox.Document.Blocks.Clear();
        richTextBox.Document.Blocks.Add(new Paragraph(new Run("content")));
        Assert.True(TextBoxHelper.GetHasText(richTextBox));
        Assert.Equal(7, (int)richTextBox.GetValue(TextBoxHelper.TextLengthProperty));
    }
}