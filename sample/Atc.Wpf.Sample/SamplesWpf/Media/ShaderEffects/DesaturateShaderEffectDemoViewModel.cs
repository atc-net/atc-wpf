namespace Atc.Wpf.Sample.SamplesWpf.Media.ShaderEffects;

public partial class DesaturateShaderEffectDemoViewModel : ViewModelBase
{
    [PropertyDisplay("Strength", "Effect", 1)]
    [PropertyRange(0, 1, 0.01)]
    [PropertyEditorHint(EditorHint.Slider)]
    [ObservableProperty]
    private double strength;
}