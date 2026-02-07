// ReSharper disable CheckNamespace
namespace Atc.Wpf.Controls;

/// <summary>
/// Specifies the status of a step in a <see cref="DataDisplay.Stepper"/> control.
/// </summary>
public enum StepperStepStatus
{
    /// <summary>
    /// The step has not yet been reached.
    /// </summary>
    Pending,

    /// <summary>
    /// The step is currently active.
    /// </summary>
    Active,

    /// <summary>
    /// The step has been completed successfully.
    /// </summary>
    Completed,

    /// <summary>
    /// The step has an error that needs attention.
    /// </summary>
    Error,
}