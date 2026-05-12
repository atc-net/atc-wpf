namespace Atc.Wpf.Sample.SamplesWpf.ValueConverters;

public partial class LogLevelToColorValueConverterSampleView
{
    public LogLevelToColorValueConverterSampleView()
    {
        InitializeComponent();

        DataContext = new LogLevelToColorValueConverterSampleViewModel();
    }
}