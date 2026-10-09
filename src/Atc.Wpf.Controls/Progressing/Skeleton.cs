namespace Atc.Wpf.Controls.Progressing;

/// <summary>A content wrapper that shows placeholder loading content while data is loading.</summary>
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