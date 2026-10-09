namespace Atc.Wpf.Helpers;

/// <summary>Provides attached properties for a <see cref="ScrollViewer"/>, such as the content presenter margin and placing the vertical scroll bar on the left side.</summary>
[SuppressMessage("Design", "CA1062:Validate arguments of public methods", Justification = "OK.")]
public static class ScrollViewerHelper
{
    /// <summary>Identifies the ScrollContentPresenterMargin attached property.</summary>
    public static readonly DependencyProperty ScrollContentPresenterMarginProperty =
        DependencyProperty.RegisterAttached(
            "ScrollContentPresenterMargin",
            typeof(Thickness),
            typeof(ScrollViewerHelper));

    /// <summary>Gets the value of the ScrollContentPresenterMargin attached property.</summary>
    public static Thickness GetScrollContentPresenterMargin(
        ScrollViewer scrollViewer)
        => (Thickness)scrollViewer.GetValue(ScrollContentPresenterMarginProperty);

    /// <summary>Sets the value of the ScrollContentPresenterMargin attached property.</summary>
    public static void SetScrollContentPresenterMargin(
        ScrollViewer scrollViewer,
        Thickness value)
        => scrollViewer.SetValue(
            ScrollContentPresenterMarginProperty,
            value);

    /// <summary>Identifies the VerticalScrollBarOnLeftSide attached property.</summary>
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

    /// <summary>Gets the value of the VerticalScrollBarOnLeftSide attached property.</summary>
    public static bool GetVerticalScrollBarOnLeftSide(UIElement element)
        => (bool)element.GetValue(VerticalScrollBarOnLeftSideProperty);

    /// <summary>Sets the value of the VerticalScrollBarOnLeftSide attached property.</summary>
    public static void SetVerticalScrollBarOnLeftSide(
        UIElement element,
        bool value)
        => element.SetValue(
            VerticalScrollBarOnLeftSideProperty,
            BooleanBoxes.Box(value));
}