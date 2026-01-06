namespace Atc.Wpf.Controls.Sample;

public sealed class SampleTreeViewItem : TreeViewItem
{
    /// <summary>
    /// The sample path property.
    /// </summary>
    public static readonly DependencyProperty SamplePathProperty = DependencyProperty.Register(
        nameof(SamplePath),
        typeof(string),
        typeof(SampleTreeViewItem),
        new UIPropertyMetadata(propertyChangedCallback: null));

    /// <summary>
    /// Gets or sets the sample path.
    /// </summary>
    /// <value>
    /// The sample path.
    /// </value>
    public string SamplePath
    {
        get => (string)GetValue(SamplePathProperty);
        set => SetValue(SamplePathProperty, value);
    }

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