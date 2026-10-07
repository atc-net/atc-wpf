namespace Atc.Wpf.Hardware.Tests.Pickers;

/// <summary>
/// Service tuning is settable from XAML on the pickers. A tuning property left at null keeps the
/// (possibly injected and pre-configured) service's own setting; a set value is pushed to the service.
/// </summary>
public sealed class PickerTuningTests
{
    private static readonly TimeSpan FiveSeconds = TimeSpan.FromSeconds(5);

    [StaFact]
    public void ProcessPicker_TuningProperties_ArePushedToTheService()
    {
        var service = Substitute.For<IProcessService>();
        service.Processes.Returns([]);
        var picker = new ProcessPicker(service);

        picker.PollingInterval = FiveSeconds;
        picker.OnlyWithMainWindow = false;

        service.Received().PollingInterval = FiveSeconds;
        service.Received().OnlyWithMainWindow = false;
    }

    [StaFact]
    public void ProcessPicker_TuningPropertiesSetInXaml_AreConvertedAndPushedToTheService()
    {
        const string xaml = """
            <hw:ProcessPicker
                xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                xmlns:hw="clr-namespace:Atc.Wpf.Hardware.Pickers;assembly=Atc.Wpf.Hardware"
                PollingInterval="00:00:05"
                OnlyWithMainWindow="False" />
            """;

        var picker = (ProcessPicker)System.Windows.Markup.XamlReader.Parse(xaml);

        Assert.Equal(FiveSeconds, picker.PollingInterval);
        Assert.False(picker.OnlyWithMainWindow);
        Assert.Equal(FiveSeconds, picker.Service.PollingInterval);
        Assert.False(picker.Service.OnlyWithMainWindow);
    }

    [StaFact]
    public void ProcessPicker_TuningPropertiesLeftNull_DoNotOverrideTheService()
    {
        var service = Substitute.For<IProcessService>();
        service.Processes.Returns([]);

        var picker = new ProcessPicker(service);

        Assert.Null(picker.PollingInterval);
        Assert.Null(picker.OnlyWithMainWindow);
        service.DidNotReceiveWithAnyArgs().PollingInterval = default;
        service.DidNotReceiveWithAnyArgs().OnlyWithMainWindow = default;
    }

    [StaFact]
    public void WindowPicker_TuningProperties_ArePushedToTheService()
    {
        var service = Substitute.For<IWindowService>();
        service.Windows.Returns([]);
        var picker = new WindowPicker(service);

        picker.PollingInterval = FiveSeconds;
        picker.OnlyVisibleWithTitle = false;

        service.Received().PollingInterval = FiveSeconds;
        service.Received().OnlyVisibleWithTitle = false;
    }

    [StaFact]
    public void NetworkAdapterPicker_TuningProperties_ArePushedToTheService()
    {
        var service = Substitute.For<INetworkAdapterService>();
        service.Adapters.Returns([]);
        var picker = new NetworkAdapterPicker(service);

        picker.PollingInterval = FiveSeconds;
        picker.IncludeLoopback = true;

        service.Received().PollingInterval = FiveSeconds;
        service.Received().IncludeLoopback = true;
    }

    [StaFact]
    public void DrivePicker_PollingInterval_IsPushedToTheService()
    {
        var service = Substitute.For<IDriveService>();
        service.Drives.Returns([]);
        var picker = new DrivePicker(service);

        picker.PollingInterval = FiveSeconds;

        service.Received().PollingInterval = FiveSeconds;
    }

    [StaFact]
    public void PrinterPicker_PollingInterval_IsPushedToTheService()
    {
        var service = Substitute.For<IPrinterService>();
        service.Printers.Returns([]);
        var picker = new PrinterPicker(service);

        picker.PollingInterval = FiveSeconds;

        service.Received().PollingInterval = FiveSeconds;
    }

    [StaFact]
    public void DisplayPicker_PollingInterval_IsPushedToTheService()
    {
        var service = Substitute.For<IDisplayService>();
        service.Displays.Returns([]);
        var picker = new DisplayPicker(service);

        picker.PollingInterval = FiveSeconds;

        service.Received().PollingInterval = FiveSeconds;
    }

    [StaFact]
    public void AutoRefreshOnDeviceChange_TurnedOffWhileLoaded_StopsWatching()
    {
        var service = Substitute.For<IProcessService>();
        service.Processes.Returns([]);
        var picker = new ProcessPicker(service);
        picker.RaiseEvent(new RoutedEventArgs(FrameworkElement.LoadedEvent));
        service.ClearReceivedCalls();

        picker.AutoRefreshOnDeviceChange = false;

        service.Received(1).StopWatching();
        service.DidNotReceive().StartWatching();
    }
}