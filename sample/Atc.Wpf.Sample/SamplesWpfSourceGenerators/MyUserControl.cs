namespace Atc.Wpf.Sample.SamplesWpfSourceGenerators;

public partial class MyUserControl : UserControl
{
    public MyUserControl()
    {
        // Dummy: to see that the RelayCommand 'DoCommand' exist
        DoCommand.RaiseCanExecuteChanged();
    }

    [RelayCommand]
    public void DoCommandHandler()
    {
        // Dummy
    }
}