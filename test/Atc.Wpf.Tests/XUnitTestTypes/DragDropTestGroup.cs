namespace Atc.Wpf.Tests.XUnitTestTypes;

/// <summary>
/// Tests that touch the static drag-and-drop state of <see cref="DragDropManager"/>; they run one at a time.
/// </summary>
[CollectionDefinition(nameof(DragDropTestGroup))]
public sealed class DragDropTestGroup;