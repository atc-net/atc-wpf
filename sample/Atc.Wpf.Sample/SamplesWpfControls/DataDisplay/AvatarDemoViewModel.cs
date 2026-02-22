namespace Atc.Wpf.Sample.SamplesWpfControls.DataDisplay;

public class AvatarDemoViewModel : ViewModelBase
{
    private AvatarSize size = AvatarSize.Medium;
    private AvatarStatus status = AvatarStatus.None;
    private string displayName = "John Doe";
    private string? initials;

    [PropertyDisplay("Size", "Appearance", 1)]
    public AvatarSize Size
    {
        get => size;
        set => Set(ref size, value);
    }

    [PropertyDisplay("Status", "Appearance", 2)]
    public AvatarStatus Status
    {
        get => status;
        set => Set(ref status, value);
    }

    [PropertyDisplay("Display Name", "Content", 1)]
    public string DisplayName
    {
        get => displayName;
        set => Set(ref displayName, value);
    }

    [PropertyDisplay("Initials", "Content", 2)]
    public string? Initials
    {
        get => initials;
        set => Set(ref initials, value);
    }
}