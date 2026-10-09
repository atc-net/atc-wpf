namespace Atc.Wpf.Tests.Translation;

[Collection(nameof(UiCultureTestGroup))]
public sealed class CultureManagerThreadTests : IDisposable
{
    private readonly CultureInfo originalUiCulture = CultureManager.UiCulture;

    public void Dispose()
        => CultureManager.UiCulture = originalUiCulture;

    [StaFact]
    public void UiCultureChanged_SubscriberOnThisThread_IsCalledRightAway()
    {
        var subscriber = new UiCultureSubscriber();
        CultureManager.UiCultureChanged += subscriber.OnUiCultureChanged;

        try
        {
            CultureManager.UiCulture = new CultureInfo("da-DK");

            Assert.Equal(1, subscriber.CallCount);
        }
        finally
        {
            CultureManager.UiCultureChanged -= subscriber.OnUiCultureChanged;
        }
    }

    [StaFact]
    public void UiCultureChanged_SubscriberOnAnotherRunningThread_IsCalledOnItsOwnThread()
    {
        using var ready = new ManualResetEventSlim();
        UiCultureSubscriber? subscriber = null;
        Dispatcher? dispatcher = null;
        var thread = new Thread(() =>
        {
            subscriber = new UiCultureSubscriber();
            dispatcher = Dispatcher.CurrentDispatcher;
            CultureManager.UiCultureChanged += subscriber.OnUiCultureChanged;
            ready.Set();
            Dispatcher.Run();
        });
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        ready.Wait();

        try
        {
            CultureManager.UiCulture = new CultureInfo("da-DK");
            dispatcher!.Invoke(() => { }, DispatcherPriority.ContextIdle);

            Assert.Equal(1, subscriber!.CallCount);
            Assert.Same(thread, subscriber.CalledOnThread);
        }
        finally
        {
            CultureManager.UiCultureChanged -= subscriber!.OnUiCultureChanged;
            dispatcher!.InvokeShutdown();
            thread.Join();
        }
    }

    [StaFact]
    public void UiCultureChanged_SubscriberOnAFinishedThread_IsSkipped()
    {
        UiCultureSubscriber? subscriber = null;
        var thread = new Thread(() =>
        {
            subscriber = new UiCultureSubscriber();
            CultureManager.UiCultureChanged += subscriber.OnUiCultureChanged;
            Dispatcher.CurrentDispatcher.InvokeShutdown();
        });
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        thread.Join();

        try
        {
            CultureManager.UiCulture = new CultureInfo("da-DK");

            Assert.Equal(0, subscriber!.CallCount);
        }
        finally
        {
            CultureManager.UiCultureChanged -= subscriber!.OnUiCultureChanged;
        }
    }
}