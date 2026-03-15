namespace Atc.Wpf.Sample.SamplesWpfControls.DataDisplay;

public partial class AvatarDemoViewModel : ViewModelBase
{
    [PropertyDisplay("Size", "Appearance", 1)]
    [ObservableProperty]
    private AvatarSize size = AvatarSize.Medium;

    [PropertyDisplay("Status", "Appearance", 2)]
    [ObservableProperty]
    private AvatarStatus status = AvatarStatus.None;

    [PropertyDisplay("Display Name", "Content", 1)]
    [ObservableProperty]
    private string displayName = "John Doe";

    [PropertyDisplay("Initials", "Content", 2)]
    [ObservableProperty]
    private string? initials;
}