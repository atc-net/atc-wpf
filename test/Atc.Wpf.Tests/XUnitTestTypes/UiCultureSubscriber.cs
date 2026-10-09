namespace Atc.Wpf.Tests.XUnitTestTypes;

/// <summary>
/// A <see cref="CultureManager.UiCultureChanged"/> subscriber that, like a control, may only be used on its own thread.
/// </summary>
public sealed class UiCultureSubscriber : DispatcherObject
{
    public int CallCount { get; private set; }

    public Thread? CalledOnThread { get; private set; }

    public void OnUiCultureChanged(
        object? sender,
        UiCultureEventArgs e)
    {
        VerifyAccess();
        CallCount++;
        CalledOnThread = Thread.CurrentThread;
    }
}