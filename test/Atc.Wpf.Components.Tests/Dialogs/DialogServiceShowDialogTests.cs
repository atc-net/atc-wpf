namespace Atc.Wpf.Components.Tests.Dialogs;

/// <summary>
/// Drives the shared show-dialog routine behind every <see cref="DialogService"/> method with a plain
/// window (the real dialog boxes need theme resources that are not available in a test host).
/// </summary>
public sealed class DialogServiceShowDialogTests
{
    private static readonly TimeSpan TestTimeout = TimeSpan.FromSeconds(10);

    [WpfFact]
    public async Task ShowDialogAsync_DialogConfirmed_ReturnsMappedResult()
    {
        Window? shown = null;

        var task = DialogService.ShowDialogAsync(
            Dispatcher.CurrentDispatcher,
            () =>
            {
                shown = CreateHiddenWindow();
                shown.Loaded += (_, _) => _ = shown.Dispatcher.BeginInvoke(() => shown.DialogResult = true);
                return shown;
            },
            (_, dialogResult) => dialogResult == true,
            fallback: false,
            CancellationToken.None);

        var result = await task.WaitAsync(TestTimeout);

        Assert.True(result);
        Assert.NotNull(shown);
    }

    [WpfFact]
    public async Task ShowDialogAsync_CancelledWhileOpen_ClosesTheDialogAndCancels()
    {
        using var cts = new CancellationTokenSource();
        Window? shown = null;

        var task = DialogService.ShowDialogAsync(
            Dispatcher.CurrentDispatcher,
            () =>
            {
                shown = CreateHiddenWindow();
                shown.Loaded += (_, _) => _ = shown.Dispatcher.BeginInvoke(cts.Cancel);
                return shown;
            },
            (_, dialogResult) => dialogResult == true,
            fallback: false,
            cts.Token);

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => task.WaitAsync(TestTimeout));
        Assert.NotNull(shown);
        Assert.False(shown.IsVisible);
    }

    [WpfFact]
    public async Task ShowDialogAsync_AlreadyCancelled_NeverCreatesTheDialog()
    {
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();
        var created = false;

        var task = DialogService.ShowDialogAsync(
            Dispatcher.CurrentDispatcher,
            () =>
            {
                created = true;
                return CreateHiddenWindow();
            },
            (_, dialogResult) => dialogResult == true,
            fallback: false,
            cts.Token);

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => task.WaitAsync(TestTimeout));
        Assert.False(created);
    }

    [WpfFact]
    public async Task ShowDialogAsync_NoDispatcher_ReturnsFallback()
    {
        var result = await DialogService.ShowDialogAsync<string?>(
            dispatcher: null,
            () => throw new InvalidOperationException("must not be called"),
            (_, _) => "mapped",
            fallback: "fallback",
            CancellationToken.None);

        Assert.Equal("fallback", result);
    }

    private static Window CreateHiddenWindow()
        => new()
        {
            Width = 100,
            Height = 100,
            Left = -10000,
            Top = -10000,
            WindowStartupLocation = WindowStartupLocation.Manual,
            WindowStyle = WindowStyle.None,
            ShowInTaskbar = false,
            ShowActivated = false,
        };
}