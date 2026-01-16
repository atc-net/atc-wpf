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
            Content = new TextBlock
            {
                Text = $"This is a NiceWindow with WindowStyle = {windowStyle}",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                FontSize = 16,
            },
        };

        window.Show();
    }
}