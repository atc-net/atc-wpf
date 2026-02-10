namespace Atc.Wpf.Sample.SamplesWpf.Animation;

public partial class AnimationView
{
    public AnimationView()
    {
        InitializeComponent();
    }

    private async void OnFadeInClick(
        object sender,
        RoutedEventArgs e)
        => await FadeTarget.FadeInAsync().ConfigureAwait(true);

    private async void OnFadeOutClick(
        object sender,
        RoutedEventArgs e)
        => await FadeTarget.FadeOutAsync().ConfigureAwait(true);

    private async void OnSlideInLeftClick(
        object sender,
        RoutedEventArgs e)
        => await SlideTarget.SlideInFromAsync(SlideDirection.Left).ConfigureAwait(true);

    private async void OnSlideInRightClick(
        object sender,
        RoutedEventArgs e)
        => await SlideTarget.SlideInFromAsync(SlideDirection.Right).ConfigureAwait(true);

    private async void OnSlideInTopClick(
        object sender,
        RoutedEventArgs e)
        => await SlideTarget.SlideInFromAsync(SlideDirection.Top).ConfigureAwait(true);

    private async void OnSlideInBottomClick(
        object sender,
        RoutedEventArgs e)
        => await SlideTarget.SlideInFromAsync(SlideDirection.Bottom).ConfigureAwait(true);

    private async void OnSlideOutLeftClick(
        object sender,
        RoutedEventArgs e)
        => await SlideTarget.SlideOutToAsync(SlideDirection.Left).ConfigureAwait(true);

    private async void OnSlideOutRightClick(
        object sender,
        RoutedEventArgs e)
        => await SlideTarget.SlideOutToAsync(SlideDirection.Right).ConfigureAwait(true);

    private async void OnSlideOutTopClick(
        object sender,
        RoutedEventArgs e)
        => await SlideTarget.SlideOutToAsync(SlideDirection.Top).ConfigureAwait(true);

    private async void OnSlideOutBottomClick(
        object sender,
        RoutedEventArgs e)
        => await SlideTarget.SlideOutToAsync(SlideDirection.Bottom).ConfigureAwait(true);

    private async void OnScaleInClick(
        object sender,
        RoutedEventArgs e)
        => await ScaleTarget.ScaleInAsync().ConfigureAwait(true);

    private async void OnScaleOutClick(
        object sender,
        RoutedEventArgs e)
        => await ScaleTarget.ScaleOutAsync().ConfigureAwait(true);

    private async void OnFastPresetClick(
        object sender,
        RoutedEventArgs e)
        => await PresetTarget.FadeOutAsync(AnimationParameters.Fast).ConfigureAwait(true);

    private async void OnDefaultPresetClick(
        object sender,
        RoutedEventArgs e)
        => await PresetTarget.FadeInAsync(AnimationParameters.Default).ConfigureAwait(true);

    private async void OnSlowPresetClick(
        object sender,
        RoutedEventArgs e)
        => await PresetTarget.FadeInAsync(AnimationParameters.Slow).ConfigureAwait(true);
}