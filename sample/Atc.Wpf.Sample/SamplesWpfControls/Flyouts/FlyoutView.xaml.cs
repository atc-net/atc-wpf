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

    private void OpenCenterFlyoutButton_Click(
        object sender,
        RoutedEventArgs e)
        => CenterFlyout.IsOpen = true;

    private void OpenPinnableFlyoutButton_Click(
        object sender,
        RoutedEventArgs e)
        => PinnableFlyout.IsOpen = true;

    private void OpenResizableFlyoutButton_Click(
        object sender,
        RoutedEventArgs e)
        => ResizableFlyout.IsOpen = true;

    private void OpenBouncyFlyoutButton_Click(
        object sender,
        RoutedEventArgs e)
        => BouncyFlyout.IsOpen = true;

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

    private void OpenPresenterFlyoutButton_Click(
        object sender,
        RoutedEventArgs e)
        => PresenterFlyout.IsOpen = true;

    private void ClosePresenterFlyout_Click(
        object sender,
        RoutedEventArgs e)
        => PresenterFlyout.IsOpen = false;

    private void SavePresenterFlyout_Click(
        object sender,
        RoutedEventArgs e)
    {
        // In a real app, you would save the form data here
        PresenterFlyout.IsOpen = false;
    }

    private void OpenFocusTrappedFlyoutButton_Click(
        object sender,
        RoutedEventArgs e)
        => FocusTrappedFlyout.IsOpen = true;

    private void OpenFocusFreeFlyoutButton_Click(
        object sender,
        RoutedEventArgs e)
        => FocusFreeFlyout.IsOpen = true;

    private void OpenBrandedFlyoutButton_Click(
        object sender,
        RoutedEventArgs e)
        => BrandedFlyout.IsOpen = true;

    private void OpenSuccessFlyoutButton_Click(
        object sender,
        RoutedEventArgs e)
        => SuccessFlyout.IsOpen = true;

    private void OpenWarningFlyoutButton_Click(
        object sender,
        RoutedEventArgs e)
        => WarningFlyout.IsOpen = true;

    private void OpenErrorFlyoutButton_Click(
        object sender,
        RoutedEventArgs e)
        => ErrorFlyout.IsOpen = true;
}