namespace Atc.Wpf.Helpers;

[SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "OK.")]
public static class ScrollViewerHelper
{
    public static readonly DependencyProperty ScrollContentPresenterMarginProperty =
        DependencyProperty.RegisterAttached(
            "ScrollContentPresenterMargin",
            typeof(Thickness),
            typeof(ScrollViewerHelper));

    public static Thickness GetScrollContentPresenterMargin(
        ScrollViewer scrollViewer)
        => (Thickness)scrollViewer.GetValue(ScrollContentPresenterMarginProperty);

    public static void SetScrollContentPresenterMargin(
        ScrollViewer scrollViewer,
        Thickness value)
        => scrollViewer.SetValue(
            ScrollContentPresenterMarginProperty,
            value);

    [SuppressMessage("", "SA1118:The parameter spans multiple lines", Justification = "OK")]
    public static readonly DependencyProperty VerticalScrollBarOnLeftSideProperty =
        DependencyProperty.RegisterAttached(
            "VerticalScrollBarOnLeftSide",
            typeof(bool),
            typeof(ScrollViewerHelper),
            new FrameworkPropertyMetadata(
                BooleanBoxes.FalseBox,
                FrameworkPropertyMetadataOptions.AffectsArrange |
                FrameworkPropertyMetadataOptions.Inherits));

    public static bool GetVerticalScrollBarOnLeftSide(UIElement element)
        => (bool)element.GetValue(VerticalScrollBarOnLeftSideProperty);

    public static void SetVerticalScrollBarOnLeftSide(
        UIElement element,
        bool value)
        => element.SetValue(
            VerticalScrollBarOnLeftSideProperty,
            BooleanBoxes.Box(value));
}