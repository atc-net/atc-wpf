namespace Atc.Wpf.Sample.SamplesWpfControls.DataDisplay;

public partial class AvatarView
{
    public AvatarView()
    {
        InitializeComponent();
        DataContext = new AvatarDemoViewModel();
    }
}