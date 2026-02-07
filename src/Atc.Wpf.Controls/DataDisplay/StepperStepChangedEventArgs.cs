namespace Atc.Wpf.Controls.DataDisplay;

/// <summary>
/// Provides data for the <see cref="Stepper.StepChanged"/> event.
/// </summary>
public sealed class StepperStepChangedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StepperStepChangedEventArgs"/> class.
    /// </summary>
    /// <param name="oldIndex">The previous step index.</param>
    /// <param name="newIndex">The new step index.</param>
    public StepperStepChangedEventArgs(
        int oldIndex,
        int newIndex)
    {
        OldIndex = oldIndex;
        NewIndex = newIndex;
    }

    /// <summary>
    /// Gets the previous step index.
    /// </summary>
    public int OldIndex { get; }

    /// <summary>
    /// Gets the new step index.
    /// </summary>
    public int NewIndex { get; }
}