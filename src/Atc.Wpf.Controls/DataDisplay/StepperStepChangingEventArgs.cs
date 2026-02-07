namespace Atc.Wpf.Controls.DataDisplay;

/// <summary>
/// Provides data for the <see cref="Stepper.StepChanging"/> event.
/// </summary>
public sealed class StepperStepChangingEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StepperStepChangingEventArgs"/> class.
    /// </summary>
    /// <param name="currentIndex">The current step index.</param>
    /// <param name="targetIndex">The target step index.</param>
    public StepperStepChangingEventArgs(
        int currentIndex,
        int targetIndex)
    {
        CurrentIndex = currentIndex;
        TargetIndex = targetIndex;
    }

    /// <summary>
    /// Gets the current step index.
    /// </summary>
    public int CurrentIndex { get; }

    /// <summary>
    /// Gets the target step index.
    /// </summary>
    public int TargetIndex { get; }

    /// <summary>
    /// Gets or sets a value indicating whether the step change should be canceled.
    /// </summary>
    public bool Cancel { get; set; }
}