namespace Atc.Wpf.Sample.SamplesWpf.Media.ShaderEffects;

public partial class FadeShaderEffectDemoViewModel : ViewModelBase
{
    [PropertyDisplay("Strength", "Effect", 1)]
    [PropertyRange(0, 1, 0.01)]
    [PropertyEditorHint(EditorHint.Slider)]
    [ObservableProperty]
    private double strength;

    [PropertyDisplay("To Color", "Effect", 2)]
    [ObservableProperty]
    private Color toColor = Colors.Gray;
}