namespace Atc.Wpf.Sample.SamplesWpfSourceGenerators;

[DependencyProperty<decimal>("MyDecimal", DefaultValue = 0.01)]
[DependencyProperty<double>("MyDouble", DefaultValue = 0.01)]
[DependencyProperty<float>("MyFloat", DefaultValue = 0.01)]
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