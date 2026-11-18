namespace Atc.Wpf.Hardware.Pickers.Internal;

/// <summary>
/// The dependency properties and the routed event that every <c>Label*</c> picker wrapper declares in the same way
/// and that differ from the declaration in the picker itself.
/// </summary>
/// <typeparam name="TInfo">The type of the selected device, with the nullability the <c>Value</c> property has.</typeparam>
[DependencyPropertyTemplate]
internal abstract class LabelDevicePickerMembers<TInfo>
{
    /// <summary>
    /// Occurs when the picker inside this control could not start watching for device changes or could not read its device list.
    /// </summary>
    [RoutedEvent(
        AddOwnerOf = typeof(DevicePickerEvents),
        HandlerType = typeof(EventHandler<DeviceErrorRoutedEventArgs>))]
    [SuppressMessage("Major Code Smell", "S2743:Static fields should not be used in generic types", Justification = "Template field: read by the source generator only, never used at run time.")]
    private static readonly RoutedEvent deviceError;

    [DependencyProperty]
    private TInfo value;

    [DependencyProperty(DefaultValue = true)]
    private bool autoRefreshOnDeviceChange;
}