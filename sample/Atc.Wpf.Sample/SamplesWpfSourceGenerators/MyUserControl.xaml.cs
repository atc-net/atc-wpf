namespace Atc.Wpf.Sample.SamplesWpfSourceGenerators;

/// <summary>
/// Interaction logic for MyUserControl.
/// </summary>
public partial class MyUserControl
{
    public MyUserControl()
    {
        InitializeComponent();

        // Dummy: to see that the RelayCommand 'DoCommand' exist
        DoCommand.RaiseCanExecuteChanged();
    }

    [RelayCommand]
    public void DoCommandHandler()
    {
        // Dummy
    }
}