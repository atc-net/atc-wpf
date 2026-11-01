namespace Atc.Wpf.Hardware.Tests.TestSupport;

/// <summary>
/// Tests that switch the shared resource culture (for example to de-DE); they run one at a time, apart from all
/// other tests, so a picker test running in parallel never reads a translated text.
/// </summary>
[CollectionDefinition("Localization", DisableParallelization = true)]
public sealed class LocalizationTestGroup;