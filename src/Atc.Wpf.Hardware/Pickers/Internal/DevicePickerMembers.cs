namespace Atc.Wpf.Hardware.Pickers.Internal;

/// <summary>
/// The dependency properties and the routed event that every hardware picker declares in the same way.
/// The <c>Label*</c> wrappers use the properties they share with their picker.
/// </summary>
[DependencyPropertyTemplate]
internal abstract class DevicePickerMembers
{
    /// <summary>
    /// Occurs when the picker could not start watching for device changes or could not read its device list.
    /// </summary>
    [RoutedEvent(
        AddOwnerOf = typeof(DevicePickerEvents),
        HandlerType = typeof(EventHandler<DeviceErrorRoutedEventArgs>))]
    private static readonly RoutedEvent deviceError;

    [DependencyProperty(DefaultValue = "")]
    private string watermarkText;

    [DependencyProperty(DefaultValue = true)]
    private bool showRefreshButton;

    [DependencyProperty(DefaultValue = true, GeneratePartialHooks = true)]
    private bool autoRefreshOnDeviceChange;

    [DependencyProperty(DefaultValue = false)]
    private bool clearValueOnDisconnect;

    [DependencyProperty(DefaultValue = true)]
    private bool autoRebindOnReconnect;

    [DependencyProperty(DefaultValue = false)]
    private bool autoSelectFirstAvailable;

    // The two summaries below are copied to the pickers, where ItemTemplate and SelectedStateMessage exist;
    // the references cannot be resolved in this class.
#pragma warning disable CS1574
    /// <summary>
    /// Gets the item template in effect: <see cref="ItemTemplate"/> when set, otherwise the default template.
    /// </summary>
    [DependencyProperty(SetterAccessModifier = PropertyAccessModifier.Private)]
    private DataTemplate? resolvedItemTemplate;

    /// <summary>
    /// Gets a value indicating whether <see cref="SelectedStateMessage"/> is not empty.
    /// </summary>
    [DependencyProperty(DefaultValue = false, SetterAccessModifier = PropertyAccessModifier.Private)]
    private bool hasSelectedStateMessage;
#pragma warning restore CS1574
}