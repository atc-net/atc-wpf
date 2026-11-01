namespace Atc.Wpf.Hardware.Tests.TestSupport;

internal sealed class FakeDefaultAudioDeviceSource : IDefaultAudioDeviceSource
{
    private EventHandler? defaultDeviceChanged;

    public event EventHandler? DefaultDeviceChanged
    {
        add
        {
            defaultDeviceChanged += value;
            SubscriberCount++;
        }

        remove
        {
            defaultDeviceChanged -= value;
            SubscriberCount--;
        }
    }

    public int SubscriberCount { get; private set; }

    public string? DefaultDeviceId { get; set; }

    public string? GetDefaultDeviceId()
        => DefaultDeviceId;

    public void ChangeDefault(string? deviceId)
    {
        DefaultDeviceId = deviceId;
        defaultDeviceChanged?.Invoke(this, EventArgs.Empty);
    }
}