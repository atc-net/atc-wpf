namespace Atc.Wpf.Controls.Tests.TestSupport;

/// <summary>
/// Tests that load a resource dictionary from a pack URI; they run one at a time, apart from all other tests, because
/// reading the assembly resource package while other threads load XAML from it is not thread-safe.
/// </summary>
[CollectionDefinition(nameof(PackResourceTestGroup), DisableParallelization = true)]
public sealed class PackResourceTestGroup;