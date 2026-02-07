namespace Atc.Wpf.Sample.SamplesWpfControls.DataDisplay;

public partial class StepperView
{
    public StepperView()
    {
        InitializeComponent();
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