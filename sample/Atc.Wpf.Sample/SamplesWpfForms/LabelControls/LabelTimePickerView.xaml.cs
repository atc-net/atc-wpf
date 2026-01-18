namespace Atc.Wpf.Sample.SamplesWpfForms.LabelControls;

public partial class LabelTimePickerView
{
    public LabelTimePickerView()
    {
        InitializeComponent();

        DataContext = this;
    }

    public CultureInfo DanishCultureInfo
        => GlobalizationConstants.DanishCultureInfo;

    private void OnClickTpMyLabel1(
        object sender,
        RoutedEventArgs e)
        => TpMyLabel1.SelectedTime = null;
}