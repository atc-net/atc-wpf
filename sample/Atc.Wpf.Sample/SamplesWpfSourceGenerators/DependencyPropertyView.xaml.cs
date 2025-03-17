namespace Atc.Wpf.Sample.SamplesWpfSourceGenerators;

[DependencyProperty<bool>("IsRunning")]
[DependencyProperty<decimal>("MyDecimal")]
[DependencyProperty<double>("MyDouble")]
public partial class DependencyPropertyView
{
    public DependencyPropertyView()
    {
        InitializeComponent();
    }
}