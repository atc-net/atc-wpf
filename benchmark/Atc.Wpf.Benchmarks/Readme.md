# Atc.Wpf.Benchmarks

Micro-benchmarks for hot paths in `Atc.Wpf` using
[BenchmarkDotNet](https://benchmarkdotnet.org/). The project lives outside
`test/` so it is never picked up by `dotnet test`.

## Running

Always run **Release** — Debug builds disable inlining and produce misleading
numbers, and BenchmarkDotNet refuses to run in Debug.

```bash
# All benchmarks
dotnet run -c Release --project benchmark/Atc.Wpf.Benchmarks -- --filter '*'

# A single class
dotnet run -c Release --project benchmark/Atc.Wpf.Benchmarks -- --filter '*SolidColorBrushHelper*'

# A single method
dotnet run -c Release --project benchmark/Atc.Wpf.Benchmarks -- --filter '*GetBrushFromString_default*'

# List benchmarks without running
dotnet run -c Release --project benchmark/Atc.Wpf.Benchmarks -- --list flat
```

Results land in `BenchmarkDotNet.Artifacts/` next to the binary.

### Gotcha: WPF target framework

`Atc.Wpf` targets `net10.0-windows`, but BenchmarkDotNet's auto-generated
boilerplate project defaults to plain `net10.0`, which can't reference our
windows-targeted assembly (NU1201). `BenchmarkConfig.Create()` (wired into
`Program.Main`) overrides the toolchain to `net10.0-windows` so the generated
project restores correctly.

This means the standard CLI job-modifier flags (`--job short`, `--job dry`,
etc.) **don't work** as you'd expect: they *add* a second job that re-uses the
default `net10.0` toolchain and fails. To shorten a run, write a custom
`[ShortRunJob]` attribute on the benchmark class instead, or pass
`--launchCount`, `--warmupCount`, `--iterationCount` directly — those mutate
the existing job rather than adding a new one.

## What is benchmarked, and why

| Benchmark | Why it exists |
|---|---|
| `SolidColorBrushHelperBenchmarks` | Pins the post-fix `O(1)` reverse lookup in `SolidColorBrushHelper.GetBrushFromString`. The earlier implementation did an `O(n)` LINQ scan over the localized brush dictionary on every call. |

Add a benchmark whenever a measurable hot-path fix lands so the regression
surface is captured before the next change.

## Conventions

- One class per hot path; group related parameter shapes with `[Params]`.
- `[MemoryDiagnoser]` on every benchmark — allocation regressions matter as
  much as time regressions.
- Method names use `snake_case` (BenchmarkDotNet convention; CA1707/CA1304 are
  suppressed in `benchmark/Directory.Build.props`).