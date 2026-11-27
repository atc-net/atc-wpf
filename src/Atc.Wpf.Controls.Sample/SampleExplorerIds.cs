namespace Atc.Wpf.Controls.Sample;

/// <summary>
/// The automation IDs of a <see cref="SampleExplorer"/>, for UI tests that drive a sample app.
/// </summary>
public static class SampleExplorerIds
{
    /// <summary>
    /// The automation ID of the filter text box.
    /// </summary>
    public const string FilterTextBox = "TbSampleFilter";

    /// <summary>
    /// The automation ID of the tab control that holds the category tabs.
    /// </summary>
    public const string TabControl = "SamplesTabControl";

    /// <summary>
    /// The automation ID of the check box that shows the sample right to left.
    /// </summary>
    public const string RightToLeftCheckBox = "SampleRightToLeft";

    /// <summary>
    /// Gets the automation ID of a category's tab: <c>Tab{Key}</c>.
    /// </summary>
    /// <param name="key">The category key.</param>
    /// <returns>The automation ID.</returns>
    public static string Tab(string key)
        => "Tab" + key;

    /// <summary>
    /// Gets the automation ID of a category's sample tree: <c>StvSamples{Key}</c>.
    /// </summary>
    /// <param name="key">The category key.</param>
    /// <returns>The automation ID.</returns>
    public static string Tree(string key)
        => "StvSamples" + key;
}