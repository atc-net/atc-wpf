namespace Atc.Wpf.Benchmarks;

/// <summary>
/// Pins the post-fix performance of <see cref="SolidColorBrushHelper.GetBrushFromString(string)"/>.
/// The earlier implementation did an O(n) LINQ scan over the localized brush dictionary on every
/// call; the current implementation uses an O(1) reverse lookup. This benchmark establishes a
/// baseline so future regressions are caught.
/// </summary>
[MemoryDiagnoser]
public class SolidColorBrushHelperBenchmarks
{
    [Params("Red", "DodgerBlue", "Coral", "AliceBlue")]
    public string ColorName { get; set; } = "Red";

    [Benchmark]
    public object? GetBrushFromString_invariant_culture()
        => SolidColorBrushHelper.GetBrushFromString(ColorName, CultureInfo.InvariantCulture);

    [Benchmark]
    public object? GetBrushFromString_default()
        => SolidColorBrushHelper.GetBrushFromString(ColorName);
}