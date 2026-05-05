namespace Atc.Wpf.Hardware.Services;

public interface IPrinterService : IDisposable
{
    ObservableCollection<PrinterInfo> Printers { get; }

    TimeSpan PollingInterval { get; set; }

    void StartWatching();

    void StopWatching();

    Task RefreshAsync();
}