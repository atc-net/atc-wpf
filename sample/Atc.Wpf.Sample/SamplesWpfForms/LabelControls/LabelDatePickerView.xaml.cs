namespace Atc.Wpf.Sample.SamplesWpfForms.LabelControls;

public partial class LabelDatePickerView
{
    public LabelDatePickerView()
    {
        InitializeComponent();

        DataContext = this;
    }

    public CultureInfo DanishCultureInfo
        => GlobalizationConstants.DanishCultureInfo;

    private void OnClickDpMyLabel1(
        object sender,
        RoutedEventArgs e)
        => DpMyLabel1.SelectedDate = null;
}