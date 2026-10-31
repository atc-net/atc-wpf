namespace Atc.Wpf.Hardware.Services.Internal;

/// <summary>
/// Follows the Windows default audio endpoint through <see cref="Windows.Media.Devices.MediaDevice"/>.
/// </summary>
/// <remarks>
/// The <see cref="Windows.Media.Devices.MediaDevice"/> events are static, so a subscription roots its handler;
/// the static event is only subscribed while this source has subscribers of its own.
/// </remarks>
internal sealed class MediaDeviceDefaultAudioSource : IDefaultAudioDeviceSource
{
    private readonly AudioDeviceKind kind;
    private readonly Lock gate = new();
    private EventHandler? defaultDeviceChanged;

    public MediaDeviceDefaultAudioSource(AudioDeviceKind kind)
        => this.kind = kind;

    public event EventHandler? DefaultDeviceChanged
    {
        add
        {
            lock (gate)
            {
                var wasEmpty = defaultDeviceChanged is null;
                defaultDeviceChanged += value;
                if (wasEmpty && defaultDeviceChanged is not null)
                {
                    Subscribe();
                }
            }
        }

        remove
        {
            lock (gate)
            {
                var hadSubscribers = defaultDeviceChanged is not null;
                defaultDeviceChanged -= value;
                if (hadSubscribers && defaultDeviceChanged is null)
                {
                    Unsubscribe();
                }
            }
        }
    }

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "No audio stack (e.g. a server without audio) must not break the picker.")]
    public string? GetDefaultDeviceId()
    {
        try
        {
            var id = kind is AudioDeviceKind.Input
                ? Windows.Media.Devices.MediaDevice.GetDefaultAudioCaptureId(Windows.Media.Devices.AudioDeviceRole.Default)
                : Windows.Media.Devices.MediaDevice.GetDefaultAudioRenderId(Windows.Media.Devices.AudioDeviceRole.Default);
            return string.IsNullOrEmpty(id) ? null : id;
        }
        catch (Exception)
        {
            return null;
        }
    }

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "No audio stack must not break the picker.")]
    private void Subscribe()
    {
        try
        {
            if (kind is AudioDeviceKind.Input)
            {
                Windows.Media.Devices.MediaDevice.DefaultAudioCaptureDeviceChanged += OnDefaultCaptureDeviceChanged;
            }
            else
            {
                Windows.Media.Devices.MediaDevice.DefaultAudioRenderDeviceChanged += OnDefaultRenderDeviceChanged;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Could not follow the default audio device: {ex.Message}");
        }
    }

    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Best-effort cleanup.")]
    private void Unsubscribe()
    {
        try
        {
            if (kind is AudioDeviceKind.Input)
            {
                Windows.Media.Devices.MediaDevice.DefaultAudioCaptureDeviceChanged -= OnDefaultCaptureDeviceChanged;
            }
            else
            {
                Windows.Media.Devices.MediaDevice.DefaultAudioRenderDeviceChanged -= OnDefaultRenderDeviceChanged;
            }
        }
        catch (Exception)
        {
            // Nothing to release.
        }
    }

    private void OnDefaultCaptureDeviceChanged(
        object sender,
        Windows.Media.Devices.DefaultAudioCaptureDeviceChangedEventArgs args)
    {
        if (args.Role is Windows.Media.Devices.AudioDeviceRole.Default)
        {
            Raise();
        }
    }

    private void OnDefaultRenderDeviceChanged(
        object sender,
        Windows.Media.Devices.DefaultAudioRenderDeviceChangedEventArgs args)
    {
        if (args.Role is Windows.Media.Devices.AudioDeviceRole.Default)
        {
            Raise();
        }
    }

    private void Raise()
    {
        EventHandler? handler;
        lock (gate)
        {
            handler = defaultDeviceChanged;
        }

        handler?.Invoke(this, EventArgs.Empty);
    }
}