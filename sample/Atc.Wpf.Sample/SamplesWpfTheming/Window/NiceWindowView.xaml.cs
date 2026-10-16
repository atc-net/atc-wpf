namespace Atc.Wpf.Sample.SamplesWpfTheming.Window;

public partial class NiceWindowView
{
    public NiceWindowView()
    {
        InitializeComponent();
    }

    private void OnOpenSingleBorderWindowClick(
        object sender,
        RoutedEventArgs e)
    {
        OpenNiceWindow(
            WindowStyle.SingleBorderWindow,
            "SingleBorderWindow");
    }

    private void OnOpenThreeDBorderWindowClick(
        object sender,
        RoutedEventArgs e)
    {
        OpenNiceWindow(
            WindowStyle.ThreeDBorderWindow,
            "ThreeDBorderWindow");
    }

    private void OnOpenToolWindowClick(
        object sender,
        RoutedEventArgs e)
    {
        OpenNiceWindow(
            WindowStyle.ToolWindow,
            "ToolWindow");
    }

    private void OnOpenNoneWindowClick(
        object sender,
        RoutedEventArgs e)
    {
        OpenNiceWindow(
            WindowStyle.None,
            "None (Borderless)");
    }

    private static void OpenNiceWindow(
        WindowStyle windowStyle,
        string title)
    {
        var window = new NiceWindow
        {
            Title = $"NiceWindow - {title}",
            Width = 600,
            Height = 400,
            WindowStyle = windowStyle,
            WindowStartupLocation = WindowStartupLocation.CenterScreen,
        };

        var content = new StackPanel
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
        };

        content.Children.Add(new TextBlock
        {
            Text = $"This is a NiceWindow with WindowStyle = {windowStyle}",
            HorizontalAlignment = HorizontalAlignment.Center,
            FontSize = 16,
        });

        if (windowStyle == WindowStyle.None)
        {
            // A borderless window has no title bar and therefore no close button of its own.
            var closeButton = new Button
            {
                Content = "Close",
                Width = 120,
                Margin = new Thickness(0, 20, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Center,
            };

            closeButton.Click += (_, _) => window.Close();
            content.Children.Add(closeButton);
        }

        window.Content = content;
        window.Show();
    }
}