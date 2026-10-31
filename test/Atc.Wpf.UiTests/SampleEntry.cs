namespace Atc.Wpf.UiTests;

/// <summary>
/// One sample in the sample app's tree.
/// </summary>
/// <param name="TreeName">The automation id of the tree view that holds it.</param>
/// <param name="Header">The tree item header.</param>
/// <param name="SamplePath">The sample path, unique across the trees.</param>
internal sealed record SampleEntry(
    string TreeName,
    string Header,
    string SamplePath);