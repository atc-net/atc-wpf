namespace Atc.Wpf.Network.Onvif;

/// <summary>
/// Reads one ONVIF camera: enter its device service URL and ONVIF account, read it, and choose one of its streams.
/// </summary>
/// <remarks>
/// This control expects an <see cref="OnvifCameraViewModel"/> as its DataContext. The control never stores the
/// credentials; <see cref="OnvifCameraViewModel.StreamSelected"/> hands them to the app with the chosen stream.
/// </remarks>
public partial class OnvifCameraView
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OnvifCameraView"/> class.
    /// </summary>
    public OnvifCameraView()
        => InitializeComponent();
}