namespace Atc.Wpf.Sample.SamplesWpfNetwork.Onvif;

public class OnvifCameraSampleViewModel : ViewModelBase
{
    public OnvifCameraSampleViewModel()
        => OnvifCamera.StreamSelected += (_, e) => SelectedStreams.Insert(
            0,
            $"{DateTime.Now:HH:mm:ss}  {e.Manufacturer} {e.Model} - {e.Stream.Profile.Name}: {e.Stream.StreamUri} (user '{e.Credential.UserName}')");

    public OnvifCameraViewModel OnvifCamera { get; } = new();

    public ObservableCollection<string> SelectedStreams { get; } = [];
}