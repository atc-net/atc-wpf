namespace Atc.Wpf.Controls.DataDisplay;

/// <summary>
/// Provides the routed command used by the Stepper control for click-to-navigate.
/// </summary>
public static class StepperCommands
{
    /// <summary>
    /// Command raised when a step indicator is clicked.
    /// The command parameter is the <see cref="StepperItem"/> that was clicked.
    /// </summary>
    public static readonly RoutedCommand StepClickedCommand = new(
        nameof(StepClickedCommand),
        typeof(StepperCommands));
}