namespace Atc.Wpf.Benchmarks;

/// <summary>
/// BenchmarkDotNet entry point. Run with `dotnet run -c Release --project benchmark/Atc.Wpf.Benchmarks`.
/// </summary>
public static class Program
{
    public static void Main(string[] args)
        => BenchmarkSwitcher
            .FromAssembly(typeof(Program).Assembly)
            .Run(args, BenchmarkConfig.Create());
}