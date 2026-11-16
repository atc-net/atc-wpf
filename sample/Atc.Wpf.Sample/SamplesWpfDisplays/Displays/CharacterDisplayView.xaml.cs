namespace Atc.Wpf.Sample.SamplesWpfDisplays.Displays;

public partial class CharacterDisplayView
{
    public CharacterDisplayView()
    {
        InitializeComponent();

        var viewModel = new CharacterDisplayDemoViewModel();
        DataContext = viewModel;

        // A replay ticks on the dispatcher; stop it so it does no work and keeps nothing alive after the page is left.
        Unloaded += (_, _) => viewModel.StopReplay();
    }
}