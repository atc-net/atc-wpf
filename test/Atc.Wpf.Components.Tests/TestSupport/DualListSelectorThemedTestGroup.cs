namespace Atc.Wpf.Components.Tests.TestSupport;

/// <summary>
/// Loading the theme from pack URIs in parallel with other tests can break them, so these tests run alone.
/// </summary>
[CollectionDefinition(nameof(DualListSelectorThemedTestGroup), DisableParallelization = true)]
[SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix", Justification = "xUnit collection definition.")]
public sealed class DualListSelectorThemedTestGroup;