namespace Atc.Wpf.Sample.SamplesWpfControls.Buttons;

public partial class ConnectivityButtonView
{
    public ConnectivityButtonView()
    {
        InitializeComponent();
        DataContext = new ConnectivityButtonDemoViewModel();
    }
}