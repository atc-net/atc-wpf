namespace Atc.Wpf.Sample.SamplesWpfNetwork.Diagnostics;

public partial class NtpTimeSampleView
{
    public NtpTimeSampleView()
    {
        InitializeComponent();
        DataContext = new NtpTimeViewModel();
    }
}