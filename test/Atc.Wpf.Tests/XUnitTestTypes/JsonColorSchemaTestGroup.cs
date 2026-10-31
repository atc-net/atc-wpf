namespace Atc.Wpf.Tests.XUnitTestTypes;

/// <summary>
/// Tests that switch the process-wide <see cref="JsonColorSchema"/> theme; they run one at a time.
/// </summary>
[CollectionDefinition(nameof(JsonColorSchemaTestGroup))]
public sealed class JsonColorSchemaTestGroup;