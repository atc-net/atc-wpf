namespace Atc.Wpf.Sample.SamplesWpfControls.Inputs;

public partial class CurrencyBoxView
{
    public CurrencyBoxView()
    {
        InitializeComponent();
        DataContext = new CurrencyBoxDemoViewModel();
    }

    private void OnCurrencyChanged(
        object sender,
        SelectionChangedEventArgs e)
    {
        if (CbCurrency.SelectedItem is not ComboBoxItem { Tag: string cultureTag })
        {
            return;
        }

        var culture = new System.Globalization.CultureInfo(cultureTag);

        foreach (var box in CurrencyShowcasePanel.FindChildren<Atc.Wpf.Controls.Inputs.CurrencyBox>())
        {
            box.Culture = culture;
        }
    }
}