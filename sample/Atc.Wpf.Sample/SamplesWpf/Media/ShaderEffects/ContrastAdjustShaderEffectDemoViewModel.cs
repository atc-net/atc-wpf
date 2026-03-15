namespace Atc.Wpf.Sample.SamplesWpf.Media.ShaderEffects;

public partial class ContrastAdjustShaderEffectDemoViewModel : ViewModelBase
{
    [PropertyDisplay("Brightness", "Effect", 1)]
    [PropertyRange(-1, 1, 0.01)]
    [PropertyEditorHint(EditorHint.Slider)]
    [ObservableProperty]
    private double brightness;

    [PropertyDisplay("Contrast", "Effect", 2)]
    [PropertyRange(-1, 1, 0.01)]
    [PropertyEditorHint(EditorHint.Slider)]
    [ObservableProperty]
    private double contrast;
}