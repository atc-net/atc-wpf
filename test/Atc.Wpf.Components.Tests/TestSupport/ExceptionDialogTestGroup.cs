namespace Atc.Wpf.Components.Tests.TestSupport;

/// <summary>
/// The exception dialog tests replace a static fallback, so they run alone.
/// </summary>
[CollectionDefinition(nameof(ExceptionDialogTestGroup), DisableParallelization = true)]
[SuppressMessage("Naming", "CA1711:Identifiers should not have incorrect suffix", Justification = "xUnit collection definition.")]
public sealed class ExceptionDialogTestGroup;