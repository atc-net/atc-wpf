namespace Atc.Wpf.Benchmarks;

/// <summary>
/// A row for the DataGridFilter benchmarks.
/// </summary>
public sealed record FilterOrder(
    string Customer,
    string Country,
    string Status,
    decimal Amount,
    DateTime Date);