namespace Atc.Wpf.Hardware.Tests.Pickers;

/// <summary>
/// The behaviour every hardware picker shares, run against each picker with a substituted service:
/// the load/unload lifecycle, auto-select, selection, a selected device that disconnects, and the
/// <c>DeviceError</c> event when the service fails.
/// </summary>
/// <typeparam name="TPicker">The picker type.</typeparam>
/// <typeparam name="TInfo">The device type the picker selects.</typeparam>
/// <typeparam name="TService">The service interface the picker is built on.</typeparam>
[SuppressMessage("Design", "CA1033:Interface methods should be callable by child types", Justification = "Test base class.")]
public abstract class DevicePickerContractTests<TPicker, TInfo, TService>
    where TPicker : FrameworkElement
    where TInfo : class, IDeviceInfo, INotifyPropertyChanged
    where TService : class, IDisposable
{
    private readonly ObservableCollection<TInfo> items = [];
    private Func<Task> refresh = () => Task.CompletedTask;

    /// <summary>
    /// Gets a value indicating whether the picker watches while it is loaded (false for the pickers that
    /// watch only while their drop-down is open).
    /// </summary>
    protected virtual bool WatchesWhileLoaded => true;

    [WpfFact]
    public void Loaded_RefreshesTheList_AndWatchesUnlessItWatchesOnlyWhileOpen()
    {
        var (picker, service) = CreateSut();

        Load(picker);

        var calls = CallNames(service);
        Assert.Contains("RefreshAsync", calls, StringComparer.Ordinal);
        Assert.Equal(WatchesWhileLoaded, calls.Contains("StartWatching", StringComparer.Ordinal));
        Assert.DoesNotContain("StopWatching", calls, StringComparer.Ordinal);
    }

    [WpfFact]
    public void Unloaded_StopsOnlyAWatchItStarted()
    {
        var (picker, service) = CreateSut();
        Load(picker);

        picker.RaiseEvent(new RoutedEventArgs(FrameworkElement.UnloadedEvent));

        Assert.Equal(WatchesWhileLoaded, CallNames(service).Contains("StopWatching", StringComparer.Ordinal));
    }

    [WpfFact]
    public void AutoSelectFirstAvailable_SelectsADeviceThatAppears()
    {
        var (picker, _) = CreateSut();
        picker.SetValue(Property("AutoSelectFirstAvailable"), true);
        Load(picker);

        var item = CreateItem(1);
        SetState(item, DeviceState.Available);
        items.Add(item);

        Assert.Same(item, picker.GetValue(Property("Value")));
    }

    [WpfFact]
    public void WithoutAutoSelect_ADeviceThatAppearsIsNotSelected()
    {
        var (picker, _) = CreateSut();
        Load(picker);

        var item = CreateItem(1);
        SetState(item, DeviceState.Available);
        items.Add(item);

        Assert.Null(picker.GetValue(Property("Value")));
    }

    [WpfFact]
    public void SelectingADevice_RaisesValueChanged()
    {
        var (picker, _) = CreateSut();
        var item = CreateItem(1);
        SetState(item, DeviceState.Available);
        items.Add(item);
        var raised = new List<RoutedEventArgs>();
        picker.AddHandler(RoutedEventField("ValueChanged"), new RoutedPropertyChangedEventHandler<TInfo?>((_, e) => raised.Add(e)));

        picker.SetValue(Property("Value"), item);

        var args = Assert.IsType<RoutedPropertyChangedEventArgs<TInfo?>>(Assert.Single(raised));
        Assert.Null(args.OldValue);
        Assert.Same(item, args.NewValue);
    }

    [WpfFact]
    public void SelectedDeviceDisconnects_RaisesDeviceLost_KeepsTheValue_AndShowsTheStateMessage()
    {
        var (picker, _) = CreateSut();
        var item = CreateItem(1);
        SetState(item, DeviceState.Available);
        items.Add(item);
        Load(picker);
        picker.SetValue(Property("Value"), item);
        var lost = 0;
        picker.AddHandler(RoutedEventField("DeviceLost"), new RoutedPropertyChangedEventHandler<TInfo?>((_, _) => lost++));

        SetState(item, DeviceState.Disconnected);

        Assert.Equal(1, lost);
        Assert.Same(item, picker.GetValue(Property("Value")));
        Assert.Equal(Miscellaneous.DeviceDisconnected, picker.GetValue(Property("SelectedStateMessage")));
    }

    [WpfFact]
    public void RefreshFails_RaisesDeviceErrorOnThePickerAndItsAncestors_AndDoesNotThrow()
    {
        var failure = new InvalidOperationException("enumeration failed");
        refresh = () => Task.FromException(failure);
        var (picker, _) = CreateSut();
        var host = new System.Windows.Controls.Border { Child = picker };
        var onPicker = new List<DeviceErrorRoutedEventArgs>();
        var onHost = new List<DeviceErrorRoutedEventArgs>();
        picker.AddHandler(DevicePickerEvents.DeviceErrorEvent, new EventHandler<DeviceErrorRoutedEventArgs>((_, e) => onPicker.Add(e)));
        host.AddHandler(DevicePickerEvents.DeviceErrorEvent, new EventHandler<DeviceErrorRoutedEventArgs>((_, e) => onHost.Add(e)));

        var exception = Record.Exception(() => Load(picker));

        Assert.Null(exception);
        var error = Assert.Single(onPicker);
        Assert.Same(failure, error.Exception);
        Assert.Equal(DevicePickerOperation.Refresh, error.Operation);
        Assert.Same(error, Assert.Single(onHost));
    }

    [WpfFact]
    public void DeviceErrorEvent_IsTheSharedEvent()
        => Assert.Same(DevicePickerEvents.DeviceErrorEvent, RoutedEventField("DeviceError"));

    protected abstract TService CreateService(
        ObservableCollection<TInfo> devices,
        Func<Task> refreshAsync);

    protected abstract TPicker CreatePicker(TService service);

    protected abstract TInfo CreateItem(int id);

    protected abstract void SetState(
        TInfo item,
        DeviceState state);

    private static void Load(TPicker picker)
        => picker.RaiseEvent(new RoutedEventArgs(FrameworkElement.LoadedEvent));

    private static List<string> CallNames(TService service)
        => service.ReceivedCalls().Select(x => x.GetMethodInfo().Name).ToList();

    private static DependencyProperty Property(string name)
        => (DependencyProperty)typeof(TPicker)
            .GetField(name + "Property", BindingFlags.Public | BindingFlags.Static)!
            .GetValue(null)!;

    private static RoutedEvent RoutedEventField(string name)
        => (RoutedEvent)typeof(TPicker)
            .GetField(name + "Event", BindingFlags.Public | BindingFlags.Static)!
            .GetValue(null)!;

    private (TPicker Picker, TService Service) CreateSut()
    {
        var service = CreateService(items, () => refresh());
        return (CreatePicker(service), service);
    }
}