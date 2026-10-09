namespace Atc.Wpf.Tests.XUnitTestTypes;

/// <summary>
/// Tests that change the process-wide UI culture; they run one at a time, apart from all other tests.
/// </summary>
[CollectionDefinition(nameof(UiCultureTestGroup), DisableParallelization = true)]
public sealed class UiCultureTestGroup;