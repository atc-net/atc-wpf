namespace Atc.Wpf.Benchmarks;

/// <summary>
/// Measures one keystroke in the TerminalViewer search box over a full terminal (10,000 lines, the default cap).
/// <c>Filter_*</c> hides the lines that don't match (the list view is refreshed); <c>Count_*</c> only counts matches.
/// Each call switches between two patterns, so every call is a pattern change like a keystroke.
/// </summary>
[MemoryDiagnoser]
public class TerminalSearchBenchmarks
{
    private const int LineCount = 10_000;

    private TerminalLineBuffer buffer = null!;
    private bool toggle;

    [GlobalSetup]
    public void Setup()
    {
        buffer = new TerminalLineBuffer();
        var lines = new List<TerminalLineItem>(LineCount);
        for (var i = 0; i < LineCount; i++)
        {
            var level = (i % 7) switch
            {
                0 => "error",
                1 => "warning",
                _ => "info",
            };

            lines.Add(new TerminalLineItem(
                $"2026-11-13 10:{(i / 60) % 60:00}:{i % 60:00} [{level}] device {i % 40} sent packet {i} with payload 0x{i:X8}",
                Brushes.White)
            {
                LineNumber = i + 1,
            });
        }

        buffer.Add(lines, LineCount);
    }

    [Benchmark(Baseline = true)]
    [STAThread]
    public int Filter_keystroke()
    {
        toggle = !toggle;
        buffer.ApplySearch(toggle ? "device 1" : "device 12", useRegex: false, hideNonMatching: true);
        return buffer.MatchCount;
    }

    [Benchmark]
    [STAThread]
    public int Count_keystroke()
    {
        toggle = !toggle;
        buffer.ApplySearch(toggle ? "device 1" : "device 12", useRegex: false, hideNonMatching: false);
        return buffer.MatchCount;
    }
}