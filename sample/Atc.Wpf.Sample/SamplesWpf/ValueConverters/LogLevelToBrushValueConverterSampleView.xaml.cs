namespace Atc.Wpf.Sample.SamplesWpf.ValueConverters;

public partial class LogLevelToBrushValueConverterSampleView
{
    public LogLevelToBrushValueConverterSampleView()
    {
        InitializeComponent();

        DataContext = new LogLevelToBrushValueConverterSampleViewModel();
    }
}