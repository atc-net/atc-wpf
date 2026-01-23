namespace Atc.Wpf.Sample.SamplesWpfControls.Flyouts;

public partial class FlyoutView
{
    public FlyoutView()
    {
        InitializeComponent();
    }

    private void OpenRightFlyoutButton_Click(
        object sender,
        RoutedEventArgs e)
        => RightFlyout.IsOpen = true;

    private void OpenLeftFlyoutButton_Click(
        object sender,
        RoutedEventArgs e)
        => LeftFlyout.IsOpen = true;

    private void OpenTopFlyoutButton_Click(
        object sender,
        RoutedEventArgs e)
        => TopFlyout.IsOpen = true;

    private void OpenBottomFlyoutButton_Click(
        object sender,
        RoutedEventArgs e)
        => BottomFlyout.IsOpen = true;

    private void OpenConfiguredFlyoutButton_Click(
        object sender,
        RoutedEventArgs e)
        => ConfiguredFlyout.IsOpen = true;

    private void OpenFormFlyoutButton_Click(
        object sender,
        RoutedEventArgs e)
        => FormFlyout.IsOpen = true;

    private void CloseFlyout_Click(
        object sender,
        RoutedEventArgs e)
    {
        if (sender is Button { Tag: Flyout flyout })
        {
            flyout.IsOpen = false;
        }
    }

    private void SaveAndCloseFlyout_Click(
        object sender,
        RoutedEventArgs e)
    {
        // In a real app, you would save settings here
        if (sender is Button { Tag: Flyout flyout })
        {
            flyout.IsOpen = false;
        }
    }

    private void OpenNestedFlyoutButton_Click(
        object sender,
        RoutedEventArgs e)
        => NestedFlyout1.IsOpen = true;

    private void OpenNestedFlyout2_Click(
        object sender,
        MouseButtonEventArgs e)
        => NestedFlyout2.IsOpen = true;

    private void OpenNestedFlyout3_Click(
        object sender,
        RoutedEventArgs e)
        => NestedFlyout3.IsOpen = true;
}