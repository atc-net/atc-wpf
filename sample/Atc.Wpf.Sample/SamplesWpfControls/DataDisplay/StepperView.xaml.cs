namespace Atc.Wpf.Sample.SamplesWpfControls.DataDisplay;

public partial class StepperView
{
    public StepperView()
    {
        InitializeComponent();
        DataContext = new StepperDemoViewModel();
    }

    private void OnUsagePreviousClick(
        object sender,
        RoutedEventArgs e)
    {
        InteractiveStepper.Previous();
    }

    private void OnUsageNextClick(
        object sender,
        RoutedEventArgs e)
    {
        InteractiveStepper.Next();
    }

    private void OnUsageResetClick(
        object sender,
        RoutedEventArgs e)
    {
        foreach (var item in InteractiveStepper.Items)
        {
            item.Status = StepperStepStatus.Pending;
        }

        InteractiveStepper.ActiveStepIndex = 0;
        UsageStepStatus.Text = "Stepper reset to step 1.";
    }

    private void OnSetErrorClick(
        object sender,
        RoutedEventArgs e)
    {
        if (InteractiveStepper.Items.Count > 1)
        {
            InteractiveStepper.Items[1].Status = StepperStepStatus.Error;
            UsageStepStatus.Text = "Step 2 set to Error state.";
        }
    }

    private void OnUsageStepChanged(
        object? sender,
        StepperStepChangedEventArgs e)
    {
        UsageStepStatus.Text = $"Navigated from step {e.OldIndex + 1} to step {e.NewIndex + 1}.";
    }
}