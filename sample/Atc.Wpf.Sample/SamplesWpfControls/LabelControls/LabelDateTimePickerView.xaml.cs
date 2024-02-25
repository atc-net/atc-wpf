namespace Atc.Wpf.Sample.SamplesWpfControls.LabelControls;

/// <summary>
/// Interaction logic for LabelDateTimePickerView.
/// </summary>
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