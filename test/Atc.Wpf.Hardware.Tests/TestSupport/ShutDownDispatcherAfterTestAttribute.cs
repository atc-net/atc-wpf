namespace Atc.Wpf.Hardware.Tests.TestSupport;

/// <summary>
/// Shuts down a WPF dispatcher that a test created on its thread, so the test host does not wait
/// for STA threads to exit ("Waiting 10 seconds for foreground threads to exit").
/// </summary>
/// <remarks>
/// A dispatcher that already existed when the test started (the one a [WpfFact] runs on) belongs to
/// Xunit.StaFact and is left alone.
/// </remarks>
[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method)]
public sealed class ShutDownDispatcherAfterTestAttribute : Xunit.v3.BeforeAfterTestAttribute
{
    [ThreadStatic]
    private static bool hadDispatcherBeforeTest;

    public override void Before(
        MethodInfo methodUnderTest,
        Xunit.v3.IXunitTest test)
        => hadDispatcherBeforeTest = Dispatcher.FromThread(Thread.CurrentThread) is not null;

    public override void After(
        MethodInfo methodUnderTest,
        Xunit.v3.IXunitTest test)
    {
        if (hadDispatcherBeforeTest)
        {
            return;
        }

        Dispatcher.FromThread(Thread.CurrentThread)?.InvokeShutdown();
    }
}