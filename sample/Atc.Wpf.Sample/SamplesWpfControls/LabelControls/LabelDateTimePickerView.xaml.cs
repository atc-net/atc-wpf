namespace Atc.Wpf.Sample.SamplesWpfControls.LabelControls;

public partial class LabelDateTimePickerView
{
    public LabelDateTimePickerView()
    {
        InitializeComponent();

        DataContext = this;
    }

    public CultureInfo DanishCultureInfo => GlobalizationConstants.DanishCultureInfo;

    private void OnClickDtpMyLabel1(
        object sender,
        RoutedEventArgs e)
    {
        DtpMyLabel1.SelectedDate = null;
    }
}