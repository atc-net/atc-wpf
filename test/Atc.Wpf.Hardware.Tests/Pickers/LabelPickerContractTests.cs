namespace Atc.Wpf.Hardware.Tests.Pickers;

/// <summary>
/// The behaviour every <c>Label*</c> picker wrapper shares, run against each wrapper: it hosts its picker,
/// forwards its settings and value, validates (required, disconnected, in use), raises the lost-focus
/// events, and lets the inner picker's <c>DeviceError</c> reach it.
/// </summary>
/// <typeparam name="TLabel">The wrapper type.</typeparam>
/// <typeparam name="TPicker">The picker type it hosts.</typeparam>
/// <typeparam name="TInfo">The device type.</typeparam>
public abstract class LabelPickerContractTests<TLabel, TPicker, TInfo>
    where TLabel : Atc.Wpf.Forms.LabelControl, new()
    where TPicker : FrameworkElement
    where TInfo : class, IDeviceInfo, INotifyPropertyChanged
{
    private readonly string[] forwardedFlags =
    [
        "ShowRefreshButton",
        "AutoRefreshOnDeviceChange",
        "ClearValueOnDisconnect",
        "AutoRebindOnReconnect",
        "AutoSelectFirstAvailable",
    ];

    /// <summary>
    /// Gets the extra boolean settings this wrapper forwards (e.g. <c>DetectInUseState</c>, <c>ShowLivePreview</c>).
    /// </summary>
    protected virtual IReadOnlyList<string> ExtraForwardedFlags => [];

    [WpfFact]
    public void HostsItsPicker()
    {
        var label = new TLabel();

        Assert.NotNull(InnerPicker(label));
    }

    [WpfFact]
    public void ForwardsItsSettingsToThePicker()
    {
        var label = new TLabel();
        var picker = InnerPicker(label);
        label.SetValue(Property<TLabel>("WatermarkText"), "Pick one");
        foreach (var name in forwardedFlags.Concat(ExtraForwardedFlags))
        {
            var current = (bool)label.GetValue(Property<TLabel>(name));
            label.SetValue(Property<TLabel>(name), !current);
        }

        Flush();

        Assert.Equal("Pick one", picker.GetValue(Property<TPicker>("WatermarkText")));
        foreach (var name in forwardedFlags.Concat(ExtraForwardedFlags))
        {
            Assert.True(
                Equals(label.GetValue(Property<TLabel>(name)), picker.GetValue(Property<TPicker>(name))),
                $"{name} is not forwarded to the picker.");
        }
    }

    [WpfFact]
    public void Value_FlowsToThePickerAndBack()
    {
        var label = new TLabel();
        var picker = InnerPicker(label);
        var first = Available(1);
        var second = Available(2);
        Flush();

        label.SetValue(Property<TLabel>("Value"), first);
        Flush();
        Assert.Same(first, picker.GetValue(Property<TPicker>("Value")));

        picker.SetValue(Property<TPicker>("Value"), second);
        Flush();
        Assert.Same(second, label.GetValue(Property<TLabel>("Value")));
    }

    [WpfFact]
    public void Mandatory_WithoutValue_IsInvalidWithTheRequiredText()
    {
        var label = new TLabel { IsMandatory = true };

        Assert.False(label.IsValid());
        Assert.Equal(Validations.DeviceIsRequired, label.ValidationText);
    }

    [WpfFact]
    public void NotMandatory_WithoutValue_IsValid()
    {
        var label = new TLabel();

        Assert.True(label.IsValid());
        Assert.Equal(string.Empty, label.ValidationText);
    }

    [WpfFact]
    public void Mandatory_WithAnAvailableValue_IsValid()
    {
        var label = new TLabel { IsMandatory = true };
        label.SetValue(Property<TLabel>("Value"), Available(1));

        Assert.True(label.IsValid());
    }

    [WpfFact]
    public void DisconnectedValue_IsInvalidWithTheNoLongerAvailableText()
    {
        var label = new TLabel();
        var item = Available(1);
        SetState(item, DeviceState.Disconnected);
        label.SetValue(Property<TLabel>("Value"), item);

        Assert.False(label.IsValid());
        Assert.Equal(Validations.DeviceNoLongerAvailable, label.ValidationText);
    }

    [WpfFact]
    public void InUseValue_IsValid_ButShowsTheInUseText()
    {
        var label = new TLabel();
        var item = Available(1);
        SetState(item, DeviceState.InUse);
        label.SetValue(Property<TLabel>("Value"), item);

        Assert.True(label.IsValid());
        Assert.Equal(Miscellaneous.DeviceInUse, label.ValidationText);
    }

    [WpfFact]
    public void SelectedDeviceDisconnects_ValidationTextUpdatesWithoutANewSelection()
    {
        var label = new TLabel();
        var item = Available(1);
        Flush();
        label.SetValue(Property<TLabel>("Value"), item);
        Flush();

        SetState(item, DeviceState.Disconnected);

        Assert.Equal(Validations.DeviceNoLongerAvailable, label.ValidationText);
    }

    [WpfFact]
    public void SelectingAnAvailableDevice_RaisesLostFocusValid()
    {
        var label = new TLabel();
        var item = Available(1);
        var valid = new List<ValueChangedEventArgs<TInfo?>>();
        var invalid = new List<ValueChangedEventArgs<TInfo?>>();
        Subscribe(label, "LostFocusValid", valid);
        Subscribe(label, "LostFocusInvalid", invalid);
        Flush();

        label.SetValue(Property<TLabel>("Value"), item);
        Flush();

        Assert.Same(item, Assert.Single(valid).NewValue);
        Assert.Empty(invalid);
    }

    [WpfFact]
    public void ClearingAMandatoryValue_RaisesLostFocusInvalid()
    {
        var label = new TLabel { IsMandatory = true };
        var invalid = new List<ValueChangedEventArgs<TInfo?>>();
        Subscribe(label, "LostFocusInvalid", invalid);
        Flush();
        label.SetValue(Property<TLabel>("Value"), Available(1));
        Flush();

        label.SetValue(Property<TLabel>("Value"), null);
        Flush();

        Assert.Null(Assert.Single(invalid).NewValue);
        Assert.Equal(Validations.DeviceIsRequired, label.ValidationText);
    }

    [WpfFact]
    public void DeviceErrorOfThePicker_ReachesTheWrapper()
    {
        var label = new TLabel();
        var picker = InnerPicker(label);
        var received = new List<DeviceErrorRoutedEventArgs>();
        var handler = new EventHandler<DeviceErrorRoutedEventArgs>((_, e) => received.Add(e));
        typeof(TLabel).GetEvent("DeviceError")!.AddEventHandler(label, handler);
        var args = new DeviceErrorRoutedEventArgs(
            DevicePickerEvents.DeviceErrorEvent,
            new InvalidOperationException("enumeration failed"),
            DevicePickerOperation.Refresh);

        picker.RaiseEvent(args);

        Assert.Same(args, Assert.Single(received));
    }

    protected abstract TInfo CreateItem(int id);

    protected abstract void SetState(
        TInfo item,
        DeviceState state);

    private static void Flush()
        => Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.ContextIdle, new Action(() => { }));

    private static TPicker InnerPicker(TLabel label)
        => FindLogical(label) ?? throw new InvalidOperationException($"{typeof(TLabel).Name} has no {typeof(TPicker).Name}.");

    private static TPicker? FindLogical(DependencyObject parent)
    {
        foreach (var child in LogicalTreeHelper.GetChildren(parent).OfType<DependencyObject>())
        {
            if (child is TPicker picker)
            {
                return picker;
            }

            if (FindLogical(child) is { } found)
            {
                return found;
            }
        }

        return null;
    }

    private static DependencyProperty Property<TOwner>(string name)
        => (DependencyProperty)(typeof(TOwner)
            .GetField(name + "Property", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            ?.GetValue(null) ?? throw new InvalidOperationException($"{typeof(TOwner).Name} has no {name} property."));

    private static void Subscribe(
        TLabel label,
        string eventName,
        List<ValueChangedEventArgs<TInfo?>> sink)
        => typeof(TLabel).GetEvent(eventName)!.AddEventHandler(
            label,
            new EventHandler<ValueChangedEventArgs<TInfo?>>((_, e) => sink.Add(e)));

    private TInfo Available(int id)
    {
        var item = CreateItem(id);
        SetState(item, DeviceState.Available);
        return item;
    }
}