namespace Atc.Wpf.Sample.SamplesWpfControls.Selectors;

public class CultureSelectorDemoViewModel : SelectorDemoViewModel
{
    private bool updateUiCultureOnChangeEvent;

    public CultureSelectorDemoViewModel(bool updateUiCultureOnChangeEvent)
        => this.updateUiCultureOnChangeEvent = updateUiCultureOnChangeEvent;

    [PropertyDisplay("Update UI Culture On Change", "Behavior", 2)]
    public bool UpdateUiCultureOnChangeEvent
    {
        get => updateUiCultureOnChangeEvent;
        set => Set(ref updateUiCultureOnChangeEvent, value);
    }
}