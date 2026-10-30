namespace Atc.Wpf.Sample.SamplesWpfControls.Selectors;

/// <summary>
/// A user row for the auto-generated columns sample.
/// </summary>
public sealed record SampleUser(
    string UserName,
    string FullName,
    string Department,
    string Email);