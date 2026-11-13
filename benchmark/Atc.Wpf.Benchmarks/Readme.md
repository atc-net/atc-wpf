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
| `SvgImageBenchmarks` | Pins the cost of loading an `SvgImage`. `Load_cold` always parses and renders; `Load_like_xaml` sets the source before initialization as XAML does, so a repeated icon comes from the frozen-drawing cache. Before the cache (and with a second render on every XAML load) this path cost about 576 µs and 53 KB per image; after, about 100 µs and 7 KB. |

| `DataGridFilterDistinctValuesBenchmarks` | Opening a DataGridFilter column pop-up over 100,000 rows with two other columns filtered. Measured at about 7–11 ms for a list of objects and about 60 ms for a `DataTable` (its rows are read through a filtered `DataView` copy); both judged acceptable for a pop-up open. |
| `TerminalSearchBenchmarks` | One keystroke in the TerminalViewer search over a full terminal (10,000 lines): about 0.36 ms when filtering, 0.14 ms when only counting, so the search is not debounced. |
| `NetworkScannerBenchmarks` | The scanner's work per progress report (`Update_same_result`, the common case), per new host, and the once-per-scan offline-host pass. Skipping unchanged ping, discovery and port data took a repeated report from about 1.2 µs and 2.4 KB to 0.4 µs and 1.25 KB. |

The benchmarks reach internal types in `Atc.Wpf.Controls`, `Atc.Wpf.Components` and
`Atc.Wpf.Network` through `InternalsVisibleTo`.

Add a benchmark whenever a measurable hot-path fix lands so the regression
surface is captured before the next change.

## In CI

`.github/workflows/benchmarks.yml` runs every benchmark on demand (*Run workflow*) and
weekly on `windows-latest`, and uploads the `BenchmarkDotNet.Artifacts/results` folder as
the `benchmark-results` artifact. There is no pull-request gate: shared runners vary too
much for a stable threshold, so compare the results between runs by hand.

## Conventions

- One class per hot path; group related parameter shapes with `[Params]`.
- `[MemoryDiagnoser]` on every benchmark — allocation regressions matter as
  much as time regressions.
- Method names use `snake_case` (BenchmarkDotNet convention; CA1707/CA1304 are
  suppressed in `benchmark/Directory.Build.props`).