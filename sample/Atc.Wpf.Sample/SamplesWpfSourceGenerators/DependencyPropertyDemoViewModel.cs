namespace Atc.Wpf.Sample.SamplesWpfSourceGenerators;

public partial class DependencyPropertyDemoViewModel : ViewModelBase
{
    [PropertyDisplay("Is Running", "Behavior", 1)]
    [ObservableProperty]
    private bool isRunning;
}
