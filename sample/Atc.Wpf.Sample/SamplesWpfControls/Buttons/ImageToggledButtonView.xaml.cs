namespace Atc.Wpf.Sample.SamplesWpfControls.Buttons;

public partial class ImageToggledButtonView
{
    public ImageToggledButtonView()
    {
        InitializeComponent();
        DataContext = new ImageToggledButtonDemoViewModel();
    }
}