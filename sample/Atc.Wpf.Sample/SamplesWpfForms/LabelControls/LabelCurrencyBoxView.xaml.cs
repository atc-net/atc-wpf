namespace Atc.Wpf.Sample.SamplesWpfForms.LabelControls;

public partial class LabelCurrencyBoxView
{
    public LabelCurrencyBoxView()
    {
        InitializeComponent();
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

        foreach (var box in UsageGrid.FindChildren<Atc.Wpf.Controls.Inputs.CurrencyBox>())
        {
            box.Culture = culture;
        }
    }
}