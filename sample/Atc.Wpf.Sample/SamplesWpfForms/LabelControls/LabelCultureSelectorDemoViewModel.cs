namespace Atc.Wpf.Sample.SamplesWpfForms.LabelControls;

public partial class LabelCultureSelectorDemoViewModel : LabelControlDemoViewModel
{
    [PropertyDisplay("Update UI Culture On Change", "Behavior", 2)]
    [ObservableProperty]
    private bool updateUiCultureOnChangeEvent;
}