namespace Atc.Wpf.Controls.Progressing;

/// <summary>A content wrapper that shows placeholder loading content while data is loading.</summary>
[ContentProperty(nameof(Content))]
[DefaultStyleKey]
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
        OverrideDefaultStyleKey();
        StyleCheck.Register(typeof(Skeleton));
    }
}