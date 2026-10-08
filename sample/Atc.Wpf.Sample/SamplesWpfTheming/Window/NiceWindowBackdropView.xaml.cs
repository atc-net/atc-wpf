namespace Atc.Wpf.Sample.SamplesWpfTheming.Window;

public partial class NiceWindowBackdropView
{
    public NiceWindowBackdropView()
    {
        InitializeComponent();
    }

    private void OnOpenWindowClick(
        object sender,
        RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { Tag: WindowBackdropType backdropType })
        {
            return;
        }

        var status = new TextBlock();
        var window = new NiceWindow
        {
            Title = $"NiceWindow backdrop - {backdropType}",
            Width = 640,
            Height = 420,
            WindowStartupLocation = WindowStartupLocation.CenterScreen,
            BackdropType = backdropType,
            Content = CreateDemoContent(status),
        };

        status.SetBinding(
            TextBlock.TextProperty,
            new Binding(nameof(NiceWindow.IsBackdropActive))
            {
                Source = window,
                StringFormat = "IsBackdropActive: {0}",
            });

        window.Show();
    }

    // The content has no background of its own, so the backdrop shows through around the controls.
    private static UniformSpacingPanel CreateDemoContent(TextBlock status)
    {
        var panel = new UniformSpacingPanel
        {
            Margin = new Thickness(24),
            Orientation = Orientation.Vertical,
            Spacing = 12,
        };

        panel.Children.Add(new TextBlock { Text = "Backdrop demo", FontSize = 20 });
        panel.Children.Add(status);
        panel.Children.Add(new TextBox { Text = "A text box on the backdrop", Width = 260, HorizontalAlignment = HorizontalAlignment.Left });
        panel.Children.Add(new CheckBox { Content = "A check box", IsChecked = true });
        panel.Children.Add(new Button { Content = "A button", Width = 120, HorizontalAlignment = HorizontalAlignment.Left });

        return panel;
    }
}