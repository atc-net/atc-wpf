namespace Atc.Wpf.Sample.SamplesWpfControls.Buttons;

public partial class AuthenticationButtonView
{
    public AuthenticationButtonView()
    {
        InitializeComponent();
        DataContext = new AuthenticationButtonDemoViewModel();
    }
}