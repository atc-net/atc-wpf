namespace Atc.Wpf.Controls.Sample;

/// <summary>
/// A tree view item in the sample tree that refers to a sample by its <see cref="SamplePath"/>.
/// </summary>
public sealed partial class SampleTreeViewItem : TreeViewItem
{
    /// <summary>
    /// Gets or sets the sample path.
    /// </summary>
    /// <value>
    /// The sample path.
    /// </value>
    [DependencyProperty(MetadataType = DependencyPropertyMetadataType.UIPropertyMetadata)]
    private string samplePath;

    /// <summary>
    /// Provides class handling for a MouseLeftButtonDown event.
    /// </summary>
    /// <param name="e">The event data.</param>
    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);

        if (string.IsNullOrEmpty(SamplePath))
        {
            SetCurrentValue(IsExpandedProperty, !IsExpanded);
        }
        else
        {
            SetCurrentValue(IsSelectedProperty, value: true);
        }

        e.Handled = true;
    }
}