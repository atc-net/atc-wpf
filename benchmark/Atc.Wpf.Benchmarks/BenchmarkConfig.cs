namespace Atc.Wpf.Benchmarks;

/// <summary>
/// Forces BenchmarkDotNet's auto-generated boilerplate project to target
/// <c>net10.0-windows</c> instead of plain <c>net10.0</c>. Without this the
/// generated project fails to restore with NU1201, because <c>Atc.Wpf</c>
/// itself targets <c>net10.0-windows7.0</c> (it's a WPF library) and is not
/// compatible with the non-windows TFM that BDN picks by default.
/// Builds on top of <see cref="DefaultConfig.Instance"/> so the standard
/// loggers, exporters, columns and validators stay in place.
/// </summary>
public static class BenchmarkConfig
{
    public static IConfig Create()
        => ManualConfig.Create(DefaultConfig.Instance)
            .AddJob(Job.Default.WithToolchain(
                CsProjCoreToolchain.From(
                    new NetCoreAppSettings(
                        targetFrameworkMoniker: "net10.0-windows",
                        runtimeFrameworkVersion: null!,
                        name: ".NET 10.0 (Windows)"))));
}