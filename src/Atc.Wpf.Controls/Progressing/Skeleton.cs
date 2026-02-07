namespace Atc.Wpf.Controls.Progressing;

[ContentProperty(nameof(Content))]
public sealed partial class Skeleton : ContentControl
{
    [DependencyProperty(DefaultValue = false)]
    private bool isLoading;

    [DependencyProperty]
    private object? loadingContent;

    [DependencyProperty]
    private DataTemplate? loadingContentTemplate;

    static Skeleton()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(Skeleton),
            new FrameworkPropertyMetadata(typeof(Skeleton)));
    }
}