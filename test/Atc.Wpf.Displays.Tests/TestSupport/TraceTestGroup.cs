namespace Atc.Wpf.Displays.Tests.TestSupport;

/// <summary>
/// Tests that count the messages written to the process-wide <see cref="Trace"/>; they run one at a time, apart from all other tests.
/// </summary>
[CollectionDefinition(nameof(TraceTestGroup), DisableParallelization = true)]
public sealed class TraceTestGroup;