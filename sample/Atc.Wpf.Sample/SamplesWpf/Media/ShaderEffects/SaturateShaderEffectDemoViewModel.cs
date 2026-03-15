namespace Atc.Wpf.Sample.SamplesWpf.Media.ShaderEffects;

public partial class SaturateShaderEffectDemoViewModel : ViewModelBase
{
    [PropertyDisplay("Progress", "Effect", 1)]
    [PropertyRange(-1, 2, 0.01)]
    [PropertyEditorHint(EditorHint.Slider)]
    [ObservableProperty]
    private double progress;
}