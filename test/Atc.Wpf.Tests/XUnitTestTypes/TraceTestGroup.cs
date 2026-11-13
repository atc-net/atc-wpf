namespace Atc.Wpf.Tests.XUnitTestTypes;

/// <summary>
/// Tests that read the process-wide Trace output; they run one at a time, apart from all other tests.
/// </summary>
[CollectionDefinition(nameof(TraceTestGroup), DisableParallelization = true)]
public sealed class TraceTestGroup;