namespace Atc.Wpf.Components.Dialogs.Helpers;

internal static class DialogBoxHelper
{
    public static ContentControl CreateHeaderControl(string headerText)
        => new()
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            Content = new TextBlock
            {
                Text = TextHelper.NormalizeLineBreaks(headerText),
                TextWrapping = TextWrapping.Wrap,
                FontSize = 24,
            },
        };

    public static ContentControl CreateContentControl(
        string contentText,
        FrameworkElement? contentImage)
    {
        var stackPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal,
        };

        if (contentImage is not null)
        {
            stackPanel.Children.Add(contentImage);
        }

        var textPanel = new StackPanel
        {
            Orientation = Orientation.Vertical,
            VerticalAlignment = VerticalAlignment.Center,
        };

        textPanel.Children.Add(
            new TextBlock
            {
                Text = TextHelper.NormalizeLineBreaks(contentText),
                TextWrapping = TextWrapping.Wrap,
            });

        stackPanel.Children.Add(textPanel);

        return new ContentControl
        {
            Content = stackPanel,
        };
    }
}