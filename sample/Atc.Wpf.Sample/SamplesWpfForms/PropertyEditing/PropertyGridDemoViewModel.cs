namespace Atc.Wpf.Sample.SamplesWpfForms.PropertyEditing;

public partial class PropertyGridDemoViewModel : ViewModelBase
{
    [PropertyDisplay("Show Categories", "Behavior", 1)]
    [ObservableProperty]
    private bool showCategories;

    [PropertyDisplay("Show Descriptions", "Behavior", 2)]
    [ObservableProperty]
    private bool showDescriptions = true;
}
