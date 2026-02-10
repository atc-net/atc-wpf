namespace Atc.Wpf.Sample.SamplesWpfControls.DataDisplay;

public partial class StepperView
{
    public StepperView()
    {
        InitializeComponent();
    }

    private void OnUsagePreviousClick(
        object sender,
        RoutedEventArgs e)
    {
        UsageStepper.Previous();
    }

    private void OnUsageNextClick(
        object sender,
        RoutedEventArgs e)
    {
        UsageStepper.Next();
    }

    private void OnUsageResetClick(
        object sender,
        RoutedEventArgs e)
    {
        // Reset all step statuses and go to step 0
        foreach (var item in UsageStepper.Items)
        {
            item.Status = StepperStepStatus.Pending;
        }

        UsageStepper.ActiveStepIndex = 0;
        UsageStepStatus.Text = "Stepper reset to step 1.";
    }

    private void OnSetErrorClick(
        object sender,
        RoutedEventArgs e)
    {
        if (UsageStepper.Items.Count > 1)
        {
            UsageStepper.Items[1].Status = StepperStepStatus.Error;
            UsageStepStatus.Text = "Step 2 set to Error state.";
        }
    }

    private void OnOrientationChanged(
        object sender,
        SelectionChangedEventArgs e)
    {
        if (UsageStepper is null)
        {
            return;
        }

        UsageStepper.Orientation = OrientationComboBox.SelectedIndex == 0
            ? Orientation.Horizontal
            : Orientation.Vertical;
    }

    private void OnIsClickableChanged(
        object sender,
        RoutedEventArgs e)
    {
        if (UsageStepper is null)
        {
            return;
        }

        UsageStepper.IsClickable = IsClickableCheckBox.IsChecked == true;
    }

    private void OnUsageStepChanged(
        object? sender,
        StepperStepChangedEventArgs e)
    {
        UsageStepStatus.Text = $"Navigated from step {e.OldIndex + 1} to step {e.NewIndex + 1}.";
    }

    private void OnPreviousClick(
        object sender,
        RoutedEventArgs e)
    {
        InteractiveStepper.Previous();
    }

    private void OnNextClick(
        object sender,
        RoutedEventArgs e)
    {
        InteractiveStepper.Next();
    }

    private void OnStepChanged(
        object? sender,
        StepperStepChangedEventArgs e)
    {
        StepStatus.Text = $"Navigated from step {e.OldIndex + 1} to step {e.NewIndex + 1}.";
    }
}