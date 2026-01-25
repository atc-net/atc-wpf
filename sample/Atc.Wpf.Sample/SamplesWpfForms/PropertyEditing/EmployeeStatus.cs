namespace Atc.Wpf.Sample.SamplesWpfForms.PropertyEditing;

/// <summary>
/// Sample enum for demonstrating enum property editing.
/// </summary>
public enum EmployeeStatus
{
    [Description("Currently active and working")]
    Active,

    [Description("On temporary leave")]
    OnLeave,

    [Description("Contract has ended")]
    Terminated,

    [Description("Working from home")]
    Remote,
}