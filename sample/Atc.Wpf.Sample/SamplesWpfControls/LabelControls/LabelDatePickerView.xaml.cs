namespace Atc.Wpf.Sample.SamplesWpfControls.LabelControls;

/// <summary>
/// Interaction logic for LabelDatePickerView.
/// </summary>
public partial class LabelDatePickerView
{
    public LabelDatePickerView()
    {
        InitializeComponent();

        DataContext = this;
    }

    public CultureInfo DanishCultureInfo => GlobalizationConstants.DanishCultureInfo;

    private void OnClickDpMyLabel1(
        object sender,
        RoutedEventArgs e)
    {
        DpMyLabel1.SelectedDate = null;
    }
}