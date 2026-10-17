namespace Atc.Wpf.Theming.Tests.Styles;

/// <summary>
/// The themed TextBox and ComboBox templates show TextBoxHelper.Watermark in PART_Message while there is
/// no text (or no selection), and hide it once there is. Visibility is checked rather than Opacity, which
/// the focus and has-text storyboards animate.
/// </summary>
public sealed class WatermarkStyleTests
{
    private const string WatermarkText = "Type here";

    [StaFact]
    public void TextBox_ShowsTheWatermarkWhileEmpty_AndHidesItOnceTextIsEntered()
    {
        var resources = ThemedWindow.CreateResources();
        var textBox = new TextBox { Style = (Style)resources["AtcApps.Styles.TextBox"] };
        Atc.Wpf.Helpers.TextBoxHelper.SetWatermark(textBox, WatermarkText);

        var window = ThemedWindow.Show(resources, textBox);
        try
        {
            var message = Part<TextBlock>(textBox, "PART_Message");
            Assert.Equal(WatermarkText, message.Text);
            Assert.Equal(Visibility.Visible, message.Visibility);

            textBox.Text = "hello";
            ThemedWindow.Pump();
            Assert.Equal(Visibility.Collapsed, message.Visibility);

            textBox.Text = string.Empty;
            ThemedWindow.Pump();
            Assert.Equal(Visibility.Visible, message.Visibility);
        }
        finally
        {
            window.Close();
        }
    }

    [StaFact]
    public void ComboBox_ShowsTheWatermarkWithoutASelection_AndHidesItOnceAnItemIsSelected()
    {
        var resources = ThemedWindow.CreateResources();
        var comboBox = new ComboBox
        {
            Style = (Style)resources["AtcApps.Styles.ComboBox"],
            ItemsSource = new[] { "One", "Two" },
        };
        Atc.Wpf.Helpers.TextBoxHelper.SetWatermark(comboBox, WatermarkText);

        var window = ThemedWindow.Show(resources, comboBox);
        try
        {
            var message = Part<TextBlock>(comboBox, "PART_Message");
            Assert.Equal(WatermarkText, message.Text);
            Assert.Equal(Visibility.Visible, message.Visibility);

            comboBox.SelectedIndex = 1;
            ThemedWindow.Pump();
            Assert.Equal(Visibility.Collapsed, message.Visibility);

            comboBox.SelectedIndex = -1;
            ThemedWindow.Pump();
            Assert.Equal(Visibility.Visible, message.Visibility);
        }
        finally
        {
            window.Close();
        }
    }

    [StaFact]
    public void EditableComboBox_ShowsTheWatermarkWhileEmpty_AndHidesItOnceTextIsEntered()
    {
        var resources = ThemedWindow.CreateResources();
        var comboBox = new ComboBox
        {
            Style = (Style)resources["AtcApps.Styles.ComboBox"],
            IsEditable = true,
            ItemsSource = new[] { "One", "Two" },
        };
        Atc.Wpf.Helpers.TextBoxHelper.SetWatermark(comboBox, WatermarkText);

        var window = ThemedWindow.Show(resources, comboBox);
        try
        {
            // The editable combo box hosts a themed TextBox; the watermark lives in that TextBox's template.
            var editableTextBox = Part<TextBox>(comboBox, "PART_EditableTextBox");
            var message = Part<TextBlock>(editableTextBox, "PART_Message");
            Assert.Equal(WatermarkText, message.Text);
            Assert.Equal(Visibility.Visible, message.Visibility);

            comboBox.Text = "typed";
            ThemedWindow.Pump();
            Assert.Equal("typed", editableTextBox.Text);
            Assert.Equal(Visibility.Collapsed, message.Visibility);

            comboBox.Text = string.Empty;
            ThemedWindow.Pump();
            Assert.Equal(Visibility.Visible, message.Visibility);
        }
        finally
        {
            window.Close();
        }
    }

    private static T Part<T>(
        Control control,
        string name)
        where T : FrameworkElement
    {
        control.ApplyTemplate();
        var part = control.Template.FindName(name, control);
        return Assert.IsType<T>(part, exactMatch: false);
    }
}