namespace Atc.Wpf.Components.Progressing;

/// <summary>
/// Default implementation of <see cref="IBusyIndicatorService"/> that wraps
/// the <see cref="BusyIndicatorManager"/> for MVVM-friendly busy overlay management.
/// </summary>
public class BusyIndicatorService : IBusyIndicatorService
{
    private readonly Dispatcher dispatcher;

    /// <summary>
    /// Initializes a new instance of the <see cref="BusyIndicatorService"/> class.
    /// </summary>
    public BusyIndicatorService()
        : this(dispatcher: null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BusyIndicatorService"/> class.
    /// </summary>
    /// <param name="dispatcher">Optional dispatcher for thread marshalling.</param>
    public BusyIndicatorService(Dispatcher? dispatcher)
    {
        dispatcher ??= Application.Current?.Dispatcher ?? Dispatcher.CurrentDispatcher;
        this.dispatcher = dispatcher;
    }

    /// <inheritdoc />
    public BusyToken Show(
        string message = "",
        string regionName = "",
        bool allowCancellation = false)
        => BusyIndicatorManager.ShowBusy(
            dispatcher,
            message,
            regionName,
            allowCancellation);

    /// <inheritdoc />
    public void Hide(BusyToken token)
    {
        ArgumentNullException.ThrowIfNull(token);
        BusyIndicatorManager.HideBusy(
            dispatcher,
            token);
    }

    /// <inheritdoc />
    public void HideAll(string regionName = "")
        => BusyIndicatorManager.HideAllBusy(
            dispatcher,
            regionName);

    /// <inheritdoc />
    public void Report(
        BusyToken token,
        BusyInfo info)
    {
        ArgumentNullException.ThrowIfNull(token);
        ArgumentNullException.ThrowIfNull(info);
        BusyIndicatorManager.ReportProgress(
            dispatcher,
            token,
            info);
    }

    /// <inheritdoc />
    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "RunAsync must always hide the overlay.")]
    public async Task RunAsync(
        Func<IProgress<BusyInfo>, CancellationToken, Task> operation,
        string message = "",
        string regionName = "",
        bool allowCancellation = false)
    {
        ArgumentNullException.ThrowIfNull(operation);

        var token = Show(
            message,
            regionName,
            allowCancellation);
        var ct = BusyIndicatorManager.GetCancellationToken(token);
        var progress = new Progress<BusyInfo>(info => Report(
            token,
            info));

        try
        {
            await operation(
                progress,
                ct).ConfigureAwait(true);
        }
        catch (OperationCanceledException)
        {
            BusyIndicatorManager.MarkCancelled(token);
        }
        finally
        {
            Hide(token);
        }
    }

    /// <inheritdoc />
    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "RunAsync must always hide the overlay.")]
    public async Task<T> RunAsync<T>(
        Func<IProgress<BusyInfo>, CancellationToken, Task<T>> operation,
        string message = "",
        string regionName = "",
        bool allowCancellation = false)
    {
        ArgumentNullException.ThrowIfNull(operation);

        var token = Show(
            message,
            regionName,
            allowCancellation);
        var ct = BusyIndicatorManager.GetCancellationToken(token);
        var progress = new Progress<BusyInfo>(info => Report(
            token,
            info));

        try
        {
            return await operation(
                progress,
                ct).ConfigureAwait(true);
        }
        catch (OperationCanceledException)
        {
            BusyIndicatorManager.MarkCancelled(token);
            return default!;
        }
        finally
        {
            Hide(token);
        }
    }
}